using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Core.Interfaces;

public interface IProductRepository
{
    Task<ProductMaster?> GetByIdAsync(Guid id);
    Task<ProductMaster?> GetBySlugAsync(string slug);
    Task<IEnumerable<ProductMaster>> GetAllAsync();
    Task<PagedResult<ProductMaster>> GetAllAsync(ProductFilter filter);
    Task<ProductMaster> AddAsync(ProductMaster product);
    Task UpdateAsync(ProductMaster product);
    Task DeleteAsync(Guid id);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null);
    Task<ProductVariant?> GetVariantByIdAsync(Guid id);
    Task<ProductVariant> AddVariantAsync(ProductVariant variant);
    Task UpdateVariantAsync(ProductVariant variant);
    Task<bool> VariantCombinationExistsAsync(Guid productMasterId, string combinationKey, Guid? excludeId = null);
    
    // Variant Options
    Task<VariantOption?> GetVariantOptionAsync(Guid id);
    Task<IEnumerable<VariantOption>> GetVariantOptionsAsync(Guid productMasterId);
    Task<VariantOption> AddVariantOptionAsync(VariantOption option);
    Task<VariantOption> UpdateVariantOptionAsync(VariantOption option);
    Task DeleteVariantOptionAsync(Guid id);
}