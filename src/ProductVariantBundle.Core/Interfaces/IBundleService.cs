using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Core.Interfaces;

public interface IBundleService
{
    Task<ProductBundle> CreateBundleAsync(ProductBundle bundle);
    Task<ProductBundle?> GetBundleAsync(Guid id);
    Task<ProductBundle?> GetBundleWithItemsAsync(Guid id);
    Task<IEnumerable<ProductBundle>> GetBundlesAsync();
    Task<PagedResult<ProductBundle>> GetBundlesAsync(BundleFilter filter);
    Task<ProductBundle> UpdateBundleAsync(ProductBundle bundle);
    Task DeleteBundleAsync(Guid id);
    Task<BundleAvailability> CalculateAvailabilityAsync(Guid bundleId, string warehouseCode = "MAIN");
    Task<BundleAvailability> CalculateAvailabilityWithLockAsync(Guid bundleId, string warehouseCode = "MAIN");
    Task ValidateBundleItemsAsync(IEnumerable<BundleItem> items);
    
    // Batch operations
    Task<BatchOperationResult<ProductBundle>> CreateBundlesBatchAsync(BatchCreateBundleRequest request);
}