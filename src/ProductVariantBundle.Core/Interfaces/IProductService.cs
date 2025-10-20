using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Models;

namespace ProductVariantBundle.Core.Interfaces;

public interface IProductService
{
    Task<ProductMaster> CreateProductMasterAsync(ProductMaster productMaster);
    Task<ProductVariant> CreateVariantAsync(ProductVariant variant);
    Task<ProductMaster?> GetProductMasterAsync(Guid id);
    Task<ProductVariant?> GetVariantAsync(Guid id);
    Task<IEnumerable<ProductMaster>> GetProductMastersAsync();
    Task<PagedResult<ProductMaster>> GetProductMastersAsync(ProductFilter filter);
    Task<ProductMaster> UpdateProductMasterAsync(ProductMaster productMaster);
    Task<ProductVariant> UpdateVariantAsync(ProductVariant variant);
    Task DeleteProductMasterAsync(Guid id);
    Task DeleteVariantAsync(Guid id);
    Task<bool> ValidateSlugUniquenessAsync(string slug, Guid? excludeId = null);
    Task<string> GenerateVariantCombinationKeyAsync(IEnumerable<VariantOptionValue> optionValues);
    
    // Variant Options
    Task<VariantOption> CreateVariantOptionAsync(VariantOption option);
    Task<VariantOption?> GetVariantOptionAsync(Guid id);
    Task<VariantOption?> GetVariantOptionByNameAsync(Guid productMasterId, string name);
    Task<IEnumerable<VariantOption>> GetVariantOptionsAsync(Guid productMasterId);
    Task<VariantOption> UpdateVariantOptionAsync(VariantOption option);
    Task DeleteVariantOptionAsync(Guid id);
    
    // Batch operations
    Task<BatchOperationResult<ProductVariant>> CreateVariantsBatchAsync(BatchCreateVariantRequest request);
    Task<BatchOperationResult<ProductVariant>> UpdateVariantsBatchAsync(BatchUpdateVariantRequest request);
}