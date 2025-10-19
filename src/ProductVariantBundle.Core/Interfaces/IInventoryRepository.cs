using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Core.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryRecord?> GetByIdAsync(Guid id);
    Task<InventoryRecord?> GetBySellableItemAndWarehouseAsync(Guid sellableItemId, Guid warehouseId);
    Task<InventoryRecord?> GetBySKUAndWarehouseCodeAsync(string sku, string warehouseCode);
    Task<InventoryRecord?> GetBySKUAndWarehouseCodeWithLockAsync(string sku, string warehouseCode);
    Task<IEnumerable<InventoryRecord>> GetBySellableItemIdAsync(Guid sellableItemId);
    Task<IEnumerable<InventoryRecord>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<InventoryRecord> AddAsync(InventoryRecord inventoryRecord);
    Task UpdateAsync(InventoryRecord inventoryRecord);
    Task DeleteAsync(Guid id);
    Task<Warehouse?> GetWarehouseByCodeAsync(string warehouseCode);
    Task<int> GetAvailableStockAsync(Guid sellableItemId, Guid warehouseId);
    Task UpdateStockAsync(Guid sellableItemId, Guid warehouseId, int onHand, int reserved);
    Task<PagedResult<InventoryRecord>> GetPagedAsync(int page, int pageSize, string? search, string? warehouseCode, string sortBy, string sortDirection);
}