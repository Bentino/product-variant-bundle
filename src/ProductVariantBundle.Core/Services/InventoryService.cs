using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models; // Added this line
using System.Data;

namespace ProductVariantBundle.Core.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ISellableItemRepository _sellableItemRepository;

    public InventoryService(IInventoryRepository inventoryRepository, ISellableItemRepository sellableItemRepository)
    {
        _inventoryRepository = inventoryRepository;
        _sellableItemRepository = sellableItemRepository;
    }

    public async Task<int> GetAvailableStockAsync(string sku, string warehouseCode = "MAIN")
    {
        var inventoryRecord = await _inventoryRepository.GetBySKUAndWarehouseCodeAsync(sku, warehouseCode);
        if (inventoryRecord == null)
        {
            return 0;
        }

        return Math.Max(0, inventoryRecord.OnHand - inventoryRecord.Reserved);
    }

    public async Task UpdateStockAsync(string sku, int onHand, int reserved, string warehouseCode = "MAIN")
    {
        var inventoryRecord = await _inventoryRepository.GetBySKUAndWarehouseCodeAsync(sku, warehouseCode);
        if (inventoryRecord == null)
        {
            throw new EntityNotFoundException("InventoryRecord", $"SKU: {sku}, Warehouse: {warehouseCode}");
        }

        if (onHand < 0)
        {
            throw new ValidationException("OnHand", "On hand quantity cannot be negative");
        }

        if (reserved < 0)
        {
            throw new ValidationException("Reserved", "Reserved quantity cannot be negative");
        }

        if (reserved > onHand)
        {
            throw new ValidationException("Reserved", "Reserved quantity cannot exceed on hand quantity");
        }

        inventoryRecord.OnHand = onHand;
        inventoryRecord.Reserved = reserved;
        inventoryRecord.UpdatedAt = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventoryRecord);
    }

    public async Task ReserveStockAsync(string sku, int quantity, string warehouseCode = "MAIN")
    {
        if (quantity <= 0)
        {
            throw new ValidationException("Quantity", "Quantity must be positive");
        }

        // Use row locking for reservation to prevent race conditions
        var inventoryRecord = await _inventoryRepository.GetBySKUAndWarehouseCodeWithLockAsync(sku, warehouseCode);
        if (inventoryRecord == null)
        {
            throw new EntityNotFoundException("InventoryRecord", $"SKU: {sku}, Warehouse: {warehouseCode}");
        }

        var availableStock = inventoryRecord.OnHand - inventoryRecord.Reserved;
        if (availableStock < quantity)
        {
            throw new ValidationException("Quantity", $"Insufficient stock. Available: {availableStock}, Requested: {quantity}");
        }

        inventoryRecord.Reserved += quantity;
        inventoryRecord.UpdatedAt = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventoryRecord);
    }

    public async Task ReleaseReservedStockAsync(string sku, int quantity, string warehouseCode = "MAIN")
    {
        if (quantity <= 0)
        {
            throw new ValidationException("Quantity", "Quantity must be positive");
        }

        // Use row locking for release to prevent race conditions
        var inventoryRecord = await _inventoryRepository.GetBySKUAndWarehouseCodeWithLockAsync(sku, warehouseCode);
        if (inventoryRecord == null)
        {
            throw new EntityNotFoundException("InventoryRecord", $"SKU: {sku}, Warehouse: {warehouseCode}");
        }

        if (inventoryRecord.Reserved < quantity)
        {
            throw new ValidationException("Quantity", $"Cannot release more than reserved. Reserved: {inventoryRecord.Reserved}, Requested: {quantity}");
        }

        inventoryRecord.Reserved -= quantity;
        inventoryRecord.UpdatedAt = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventoryRecord);
    }

    public async Task<InventoryRecord?> GetInventoryRecordAsync(string sku, string warehouseCode = "MAIN")
    {
        return await _inventoryRepository.GetBySKUAndWarehouseCodeAsync(sku, warehouseCode);
    }

    public async Task<InventoryRecord?> GetInventoryRecordWithLockAsync(string sku, string warehouseCode = "MAIN")
    {
        return await _inventoryRepository.GetBySKUAndWarehouseCodeWithLockAsync(sku, warehouseCode);
    }

    public async Task<IEnumerable<InventoryRecord>> GetWarehouseInventoryAsync(string warehouseCode)
    {
        // First, get the warehouse by code to get its ID
        var warehouse = await _inventoryRepository.GetWarehouseByCodeAsync(warehouseCode);
        if (warehouse == null)
        {
            throw new EntityNotFoundException("Warehouse", warehouseCode);
        }

        return await _inventoryRepository.GetByWarehouseIdAsync(warehouse.Id);
    }

    public async Task<InventoryRecord> CreateInventoryRecordAsync(Guid sellableItemId, Guid warehouseId, int onHand = 0, int reserved = 0)
    {
        if (onHand < 0)
        {
            throw new ValidationException("OnHand", "On hand quantity cannot be negative");
        }

        if (reserved < 0)
        {
            throw new ValidationException("Reserved", "Reserved quantity cannot be negative");
        }

        if (reserved > onHand)
        {
            throw new ValidationException("Reserved", "Reserved quantity cannot exceed on hand quantity");
        }

        // Check if inventory record already exists
        var existing = await _inventoryRepository.GetBySellableItemAndWarehouseAsync(sellableItemId, warehouseId);
        if (existing != null)
        {
            throw new DuplicateEntityException("InventoryRecord", "SellableItem-Warehouse combination", $"{sellableItemId}-{warehouseId}");
        }

        var inventoryRecord = new InventoryRecord
        {
            Id = Guid.NewGuid(),
            SellableItemId = sellableItemId,
            WarehouseId = warehouseId,
            OnHand = onHand,
            Reserved = reserved,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = EntityStatus.Active
        };

        return await _inventoryRepository.AddAsync(inventoryRecord);
    }

    public Task<PagedResult<InventoryRecord>> GetInventoryRecordsAsync(int page = 1, int pageSize = 10, string? search = null, string? warehouseCode = null, string sortBy = "SKU", string sortDirection = "asc")
    {
        return _inventoryRepository.GetPagedAsync(page, pageSize, search, warehouseCode, sortBy, sortDirection);
    }

    public Task<InventoryStats> GetInventoryStatsAsync(string warehouseCode = "MAIN", int lowStockThreshold = 10)
    {
        throw new NotImplementedException();
    }
}