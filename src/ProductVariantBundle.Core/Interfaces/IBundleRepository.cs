using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Core.Interfaces;

public interface IBundleRepository
{
    Task<ProductBundle?> GetByIdAsync(Guid id);
    Task<ProductBundle?> GetBySKUAsync(string sku);
    Task<IEnumerable<ProductBundle>> GetAllAsync();
    Task<PagedResult<ProductBundle>> GetAllAsync(BundleFilter filter);
    Task<ProductBundle> AddAsync(ProductBundle bundle);
    Task UpdateAsync(ProductBundle bundle);
    Task DeleteAsync(Guid id);
    Task<ProductBundle?> GetByIdWithItemsAsync(Guid id);
}