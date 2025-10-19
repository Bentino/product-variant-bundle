using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Core.Interfaces;

public interface IInventoryService
{
    Task<int> GetAvailableStockAsync(string sku, string warehouseCode = "MAIN");
    Task UpdateStockAsync(string sku, int onHand, int reserved, string warehouseCode = "MAIN");
    Task ReserveStockAsync(string sku, int quantity, string warehouseCode = "MAIN");
    Task ReleaseReservedStockAsync(string sku, int quantity, string warehouseCode = "MAIN");
    Task<InventoryRecord?> GetInventoryRecordAsync(string sku, string warehouseCode = "MAIN");
    Task<InventoryRecord?> GetInventoryRecordWithLockAsync(string sku, string warehouseCode = "MAIN");
    Task<IEnumerable<InventoryRecord>> GetWarehouseInventoryAsync(string warehouseCode);
    Task<InventoryRecord> CreateInventoryRecordAsync(Guid sellableItemId, Guid warehouseId, int onHand = 0, int reserved = 0);
    Task<PagedResult<InventoryRecord>> GetInventoryRecordsAsync(int page = 1, int pageSize = 10, string? search = null, string? warehouseCode = null, string sortBy = "SKU", string sortDirection = "asc");
}