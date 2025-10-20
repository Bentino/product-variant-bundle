using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Core.Interfaces;

public interface ISellableItemService
{
    Task<bool> ValidateSkuUniquenessAsync(string sku, Guid? excludeId = null);
    Task<SellableItem?> GetBySKUAsync(string sku);
    Task<SellableItem?> GetByVariantIdAsync(Guid variantId);
    Task<SellableItem?> GetByBundleIdAsync(Guid bundleId);
    Task<SellableItem> CreateSellableItemAsync(SellableItemType type, Guid entityId, string sku);
    Task<SellableItem> UpdateSellableItemAsync(SellableItem sellableItem);
    Task DeleteSellableItemAsync(Guid id);
}