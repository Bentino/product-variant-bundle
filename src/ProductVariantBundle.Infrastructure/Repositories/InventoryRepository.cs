using Microsoft.EntityFrameworkCore;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Infrastructure.Data;

namespace ProductVariantBundle.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryRecord?> GetByIdAsync(Guid id)
    {
        return await _context.InventoryRecords
            .Include(ir => ir.SellableItem)
            .Include(ir => ir.Warehouse)
            .FirstOrDefaultAsync(ir => ir.Id == id && ir.Status == EntityStatus.Active);
    }

    public async Task<InventoryRecord?> GetBySellableItemAndWarehouseAsync(Guid sellableItemId, Guid warehouseId)
    {
        return await _context.InventoryRecords
            .Include(ir => ir.SellableItem)
            .Include(ir => ir.Warehouse)
            .FirstOrDefaultAsync(ir => ir.SellableItemId == sellableItemId && 
                                     ir.WarehouseId == warehouseId && 
                                     ir.Status == EntityStatus.Active);
    }

    public async Task<InventoryRecord?> GetBySKUAndWarehouseCodeAsync(string sku, string warehouseCode)
    {
        return await _context.InventoryRecords
            .Include(ir => ir.SellableItem)
            .Include(ir => ir.Warehouse)
            .FirstOrDefaultAsync(ir => ir.SellableItem.SKU == sku && 
                                     ir.Warehouse.Code == warehouseCode && 
                                     ir.Status == EntityStatus.Active &&
                                     ir.SellableItem.Status == EntityStatus.Active);
    }

    public async Task<InventoryRecord?> GetBySKUAndWarehouseCodeWithLockAsync(string sku, string warehouseCode)
    {
        return await _context.InventoryRecords
            .FromSqlRaw(@"
                SELECT ir.* FROM inventory_records ir
                INNER JOIN sellable_items si ON ir.sellable_item_id = si.id
                INNER JOIN warehouses w ON ir.warehouse_id = w.id
                WHERE si.sku = {0} AND w.code = {1} 
                AND ir.status = 0 AND si.status = 0
                FOR UPDATE", sku, warehouseCode)
            .Include(ir => ir.SellableItem)
            .Include(ir => ir.Warehouse)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<InventoryRecord>> GetBySellableItemIdAsync(Guid sellableItemId)
    {
        return await _context.InventoryRecords
            .Include(ir => ir.Warehouse)
            .Where(ir => ir.SellableItemId == sellableItemId && ir.Status == EntityStatus.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryRecord>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _context.InventoryRecords
            .Include(ir => ir.SellableItem)
            .Where(ir => ir.WarehouseId == warehouseId && ir.Status == EntityStatus.Active)
            .ToListAsync();
    }

    public async Task<InventoryRecord> AddAsync(InventoryRecord inventoryRecord)
    {
        inventoryRecord.CreatedAt = DateTime.UtcNow;
        inventoryRecord.UpdatedAt = DateTime.UtcNow;
        inventoryRecord.Status = EntityStatus.Active;

        _context.InventoryRecords.Add(inventoryRecord);
        await _context.SaveChangesAsync();
        return inventoryRecord;
    }

    public async Task UpdateAsync(InventoryRecord inventoryRecord)
    {
        inventoryRecord.UpdatedAt = DateTime.UtcNow;
        _context.InventoryRecords.Update(inventoryRecord);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var inventoryRecord = await _context.InventoryRecords.FindAsync(id);
        if (inventoryRecord != null)
        {
            inventoryRecord.Status = EntityStatus.Archived;
            inventoryRecord.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetAvailableStockAsync(Guid sellableItemId, Guid warehouseId)
    {
        var inventoryRecord = await GetBySellableItemAndWarehouseAsync(sellableItemId, warehouseId);
        return inventoryRecord != null ? Math.Max(0, inventoryRecord.OnHand - inventoryRecord.Reserved) : 0;
    }

    public async Task UpdateStockAsync(Guid sellableItemId, Guid warehouseId, int onHand, int reserved)
    {
        var inventoryRecord = await GetBySellableItemAndWarehouseAsync(sellableItemId, warehouseId);
        
        if (inventoryRecord != null)
        {
            inventoryRecord.OnHand = onHand;
            inventoryRecord.Reserved = reserved;
            await UpdateAsync(inventoryRecord);
        }
        else
        {
            // Create new inventory record if it doesn't exist
            var newRecord = new InventoryRecord
            {
                Id = Guid.NewGuid(),
                SellableItemId = sellableItemId,
                WarehouseId = warehouseId,
                OnHand = onHand,
                Reserved = reserved
            };
            await AddAsync(newRecord);
        }
    }

    public async Task<Warehouse?> GetWarehouseByCodeAsync(string warehouseCode)
    {
        return await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Code == warehouseCode && w.Status == EntityStatus.Active);
    }

    public async Task<PagedResult<InventoryRecord>> GetPagedAsync(int page, int pageSize, string? search, string? warehouseCode, string sortBy, string sortDirection)
    {
        var query = _context.InventoryRecords
            .Include(ir => ir.SellableItem)
            .Include(ir => ir.Warehouse)
            .Where(ir => ir.Status == EntityStatus.Active);

        // Apply search filter
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(ir => ir.SellableItem.SKU.Contains(search));
        }

        // Apply warehouse filter
        if (!string.IsNullOrEmpty(warehouseCode))
        {
            query = query.Where(ir => ir.Warehouse.Code == warehouseCode);
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "sku" => sortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(ir => ir.SellableItem.SKU)
                : query.OrderBy(ir => ir.SellableItem.SKU),
            "onhand" => sortDirection.ToLower() == "desc"
                ? query.OrderByDescending(ir => ir.OnHand)
                : query.OrderBy(ir => ir.OnHand),
            "available" => sortDirection.ToLower() == "desc"
                ? query.OrderByDescending(ir => ir.OnHand - ir.Reserved)
                : query.OrderBy(ir => ir.OnHand - ir.Reserved),
            _ => query.OrderBy(ir => ir.SellableItem.SKU)
        };

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<InventoryRecord>
        {
            Data = items,
            Meta = new PaginationMeta
            {
                Page = page,
                PageSize = pageSize,
                Total = totalCount,
                TotalPages = totalPages,
                HasNext = page < totalPages,
                HasPrevious = page > 1
            }
        };
    }
}