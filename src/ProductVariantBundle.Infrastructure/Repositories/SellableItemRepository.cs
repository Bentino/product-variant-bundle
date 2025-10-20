using Microsoft.EntityFrameworkCore;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Infrastructure.Data;

namespace ProductVariantBundle.Infrastructure.Repositories;

public class SellableItemRepository : ISellableItemRepository
{
    private readonly ApplicationDbContext _context;

    public SellableItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SellableItem?> GetByIdAsync(Guid id)
    {
        return await _context.SellableItems
            .Include(si => si.Variant)
                .ThenInclude(v => v!.ProductMaster)
            .Include(si => si.Bundle)
            .Include(si => si.InventoryRecords)
                .ThenInclude(ir => ir.Warehouse)
            .FirstOrDefaultAsync(si => si.Id == id && si.Status == EntityStatus.Active);
    }

    public async Task<SellableItem?> GetBySKUAsync(string sku)
    {
        return await _context.SellableItems
            .Include(si => si.Variant)
                .ThenInclude(v => v!.ProductMaster)
            .Include(si => si.Bundle)
            .Include(si => si.InventoryRecords)
                .ThenInclude(ir => ir.Warehouse)
            .FirstOrDefaultAsync(si => si.SKU == sku && si.Status == EntityStatus.Active);
    }

    public async Task<SellableItem?> GetByVariantIdAsync(Guid variantId)
    {
        return await _context.SellableItems
            .FirstOrDefaultAsync(si => si.VariantId == variantId && si.Status == EntityStatus.Active);
    }

    public async Task<SellableItem?> GetByBundleIdAsync(Guid bundleId)
    {
        return await _context.SellableItems
            .FirstOrDefaultAsync(si => si.BundleId == bundleId && si.Status == EntityStatus.Active);
    }

    public async Task<IEnumerable<SellableItem>> GetAllAsync()
    {
        return await _context.SellableItems
            .Include(si => si.Variant)
            .Include(si => si.Bundle)
            .Where(si => si.Status == EntityStatus.Active)
            .ToListAsync();
    }

    public async Task<SellableItem> AddAsync(SellableItem sellableItem)
    {
        sellableItem.CreatedAt = DateTime.UtcNow;
        sellableItem.UpdatedAt = DateTime.UtcNow;
        sellableItem.Status = EntityStatus.Active;

        _context.SellableItems.Add(sellableItem);
        await _context.SaveChangesAsync();
        return sellableItem;
    }

    public async Task UpdateAsync(SellableItem sellableItem)
    {
        sellableItem.UpdatedAt = DateTime.UtcNow;
        _context.SellableItems.Update(sellableItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var sellableItem = await _context.SellableItems.FindAsync(id);
        if (sellableItem != null)
        {
            sellableItem.Status = EntityStatus.Archived;
            sellableItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> SKUExistsAsync(string sku, Guid? excludeId = null)
    {
        var query = _context.SellableItems
            .Where(si => si.SKU == sku && si.Status == EntityStatus.Active);

        if (excludeId.HasValue)
        {
            query = query.Where(si => si.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<SellableItem>> GetByTypeAsync(SellableItemType type)
    {
        return await _context.SellableItems
            .Include(si => si.Variant)
            .Include(si => si.Bundle)
            .Where(si => si.Type == type && si.Status == EntityStatus.Active)
            .ToListAsync();
    }
}