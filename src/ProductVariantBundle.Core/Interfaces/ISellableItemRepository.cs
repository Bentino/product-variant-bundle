using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Core.Interfaces;

public interface ISellableItemRepository
{
    Task<SellableItem?> GetByIdAsync(Guid id);
    Task<SellableItem?> GetBySKUAsync(string sku);
    Task<SellableItem?> GetByVariantIdAsync(Guid variantId);
    Task<SellableItem?> GetByBundleIdAsync(Guid bundleId);
    Task<IEnumerable<SellableItem>> GetAllAsync();
    Task<SellableItem> AddAsync(SellableItem sellableItem);
    Task UpdateAsync(SellableItem sellableItem);
    Task DeleteAsync(Guid id);
    Task<bool> SKUExistsAsync(string sku, Guid? excludeId = null);
    Task<IEnumerable<SellableItem>> GetByTypeAsync(SellableItemType type);
}