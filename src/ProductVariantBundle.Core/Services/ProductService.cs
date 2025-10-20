using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Helpers;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Core.Validators;

namespace ProductVariantBundle.Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ISellableItemService _sellableItemService;
    private readonly ProductValidator _validator;
    private readonly BatchOperationService _batchOperationService;

    public ProductService(
        IProductRepository productRepository, 
        ISellableItemService sellableItemService, 
        ProductValidator validator,
        BatchOperationService batchOperationService)
    {
        _productRepository = productRepository;
        _sellableItemService = sellableItemService;
        _validator = validator;
        _batchOperationService = batchOperationService;
    }

    public async Task<ProductMaster> CreateProductMasterAsync(ProductMaster productMaster)
    {
        // Validate using validator
        await _validator.ValidateProductMasterAsync(productMaster, false);

        // Set audit fields
        productMaster.Id = Guid.NewGuid();
        productMaster.CreatedAt = DateTime.UtcNow;
        productMaster.UpdatedAt = DateTime.UtcNow;
        productMaster.Status = EntityStatus.Active;

        return await _productRepository.AddAsync(productMaster);
    }

    public async Task<ProductVariant> CreateVariantAsync(ProductVariant variant)
    {
        // Validate using validator
        await _validator.ValidateProductVariantAsync(variant, false);

        // Set audit fields (only if not already set)
        if (variant.Id == Guid.Empty)
        {
            variant.Id = Guid.NewGuid();
        }
        variant.CreatedAt = DateTime.UtcNow;
        variant.UpdatedAt = DateTime.UtcNow;
        variant.Status = EntityStatus.Active;

        // Generate combination key
        if (variant.OptionValues != null && variant.OptionValues.Any())
        {
            variant.CombinationKey = await GenerateVariantCombinationKeyAsync(variant.OptionValues);
        }
        else
        {
            variant.CombinationKey = string.Empty;
        }

        var createdVariant = await _productRepository.AddVariantAsync(variant);
        return createdVariant;
    }

    public async Task<ProductMaster?> GetProductMasterAsync(Guid id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<ProductVariant?> GetVariantAsync(Guid id)
    {
        return await _productRepository.GetVariantByIdAsync(id);
    }

    public async Task<IEnumerable<ProductMaster>> GetProductMastersAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<PagedResult<ProductMaster>> GetProductMastersAsync(ProductFilter filter)
    {
        return await _productRepository.GetAllAsync(filter);
    }



    public async Task<ProductMaster> UpdateProductMasterAsync(ProductMaster productMaster)
    {
        var existing = await _productRepository.GetByIdAsync(productMaster.Id);
        if (existing == null)
        {
            throw new EntityNotFoundException("ProductMaster", productMaster.Id);
        }

        // Validate using validator
        await _validator.ValidateProductMasterAsync(productMaster, true);

        // Update audit fields
        productMaster.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(productMaster);
        return productMaster;
    }

    public async Task<ProductVariant> UpdateVariantAsync(ProductVariant variant)
    {
        var existing = await _productRepository.GetVariantByIdAsync(variant.Id);
        if (existing == null)
        {
            throw new EntityNotFoundException("ProductVariant", variant.Id);
        }

        // Validate using validator
        await _validator.ValidateProductVariantAsync(variant, true);

        // Update audit fields
        variant.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateVariantAsync(variant);
        return variant;
    }

    public async Task DeleteProductMasterAsync(Guid id)
    {
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new EntityNotFoundException("ProductMaster", id);
        }

        await _productRepository.DeleteAsync(id);
    }

    public async Task DeleteVariantAsync(Guid id)
    {
        var existing = await _productRepository.GetVariantByIdAsync(id);
        if (existing == null)
        {
            throw new EntityNotFoundException("ProductVariant", id);
        }

        // Delete the variant using the correct method
        await _productRepository.DeleteVariantAsync(id);
    }

    public async Task<bool> ValidateSlugUniquenessAsync(string slug, Guid? excludeId = null)
    {
        var normalizedSlug = SlugHelper.Normalize(slug);
        return !await _productRepository.SlugExistsAsync(normalizedSlug, excludeId);
    }



    public async Task<BatchOperationResult<ProductVariant>> CreateVariantsBatchAsync(BatchCreateVariantRequest request)
    {
        // Skip idempotency check for now to debug the main issue
        // var existingResult = await _batchOperationService.GetExistingOperationAsync<ProductVariant>(request.IdempotencyKey);
        // if (existingResult != null)
        // {
        //     return existingResult;
        // }

        var results = new List<BatchItemResult<ProductVariant>>();
        var index = 0;

        foreach (var item in request.Items)
        {
            var itemResult = new BatchItemResult<ProductVariant> { Index = index };
            
            try
            {
                // Check if variant with same combination already exists
                var productMaster = await _productRepository.GetByIdAsync(item.ProductMasterId);
                if (productMaster == null)
                {
                    itemResult.Success = false;
                    itemResult.Errors = new[] { "Product master not found" };
                    results.Add(itemResult);
                    index++;
                    continue;
                }

                // Generate variant ID first
                var variantId = Guid.NewGuid();
                
                // Create variant option values from batch item with proper ProductVariantId
                var optionValues = item.OptionValues.Select(ov => new VariantOptionValue
                {
                    Id = Guid.NewGuid(),
                    VariantOptionId = ov.VariantOptionId,
                    Value = ov.Value,
                    ProductVariantId = variantId,
                    Status = EntityStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                var combinationKey = ProductValidator.GenerateVariantCombinationKey(optionValues);
                var existingVariant = await _productRepository.VariantCombinationExistsAsync(item.ProductMasterId, combinationKey);

                if (existingVariant)
                {
                    switch (request.OnConflict)
                    {
                        case ConflictResolutionStrategy.Fail:
                            itemResult.Success = false;
                            itemResult.Errors = new[] { "Variant combination already exists" };
                            break;
                        case ConflictResolutionStrategy.Skip:
                            itemResult.Success = true;
                            itemResult.Data = null; // Skipped
                            break;
                        case ConflictResolutionStrategy.Update:
                            // Find existing variant and update it
                            // This would require additional repository method to get by combination key
                            itemResult.Success = false;
                            itemResult.Errors = new[] { "Update strategy not implemented for variants" };
                            break;
                    }
                }
                else
                {
                    // Create new variant with pre-set ID
                    var variant = new ProductVariant
                    {
                        Id = variantId,
                        ProductMasterId = item.ProductMasterId,
                        Price = item.Price,
                        CombinationKey = combinationKey,
                        OptionValues = optionValues
                    };

                    var createdVariant = await CreateVariantAsync(variant);
                    itemResult.Success = true;
                    itemResult.Data = createdVariant;
                }
            }
            catch (ValidationException ex)
            {
                itemResult.Success = false;
                itemResult.Errors = ex.Errors.SelectMany(kvp => kvp.Value).ToArray();
            }
            catch (Exception ex)
            {
                itemResult.Success = false;
                itemResult.Errors = new[] { ex.Message };
            }

            results.Add(itemResult);
            index++;
        }

        var batchResult = BatchOperationService.CreateResult(request.IdempotencyKey, request.OnConflict, results);
        
        // Save the operation result for idempotency
        await _batchOperationService.SaveOperationResultAsync(request.IdempotencyKey, "CreateVariants", batchResult);
        
        return batchResult;
    }

    public async Task<BatchOperationResult<ProductVariant>> UpdateVariantsBatchAsync(BatchUpdateVariantRequest request)
    {
        // Check for existing operation with same idempotency key
        var existingResult = await _batchOperationService.GetExistingOperationAsync<ProductVariant>(request.IdempotencyKey);
        if (existingResult != null)
        {
            return existingResult;
        }

        var results = new List<BatchItemResult<ProductVariant>>();
        var index = 0;

        foreach (var item in request.Items)
        {
            var itemResult = new BatchItemResult<ProductVariant> { Index = index };
            
            try
            {
                var existingVariant = await _productRepository.GetVariantByIdAsync(item.Id);
                if (existingVariant == null)
                {
                    switch (request.OnConflict)
                    {
                        case ConflictResolutionStrategy.Fail:
                            itemResult.Success = false;
                            itemResult.Errors = new[] { "Variant not found" };
                            break;
                        case ConflictResolutionStrategy.Skip:
                            itemResult.Success = true;
                            itemResult.Data = null; // Skipped
                            break;
                        default:
                            itemResult.Success = false;
                            itemResult.Errors = new[] { "Variant not found" };
                            break;
                    }
                }
                else
                {
                    // Update variant properties
                    if (item.Price.HasValue)
                        existingVariant.Price = item.Price.Value;

                    if (item.OptionValues != null)
                    {
                        existingVariant.OptionValues = item.OptionValues.Select(ov => new VariantOptionValue
                        {
                            VariantOptionId = ov.VariantOptionId,
                            Value = ov.Value
                        }).ToList();
                    }

                    // SKU updates will be handled separately through SellableItem service

                    var updatedVariant = await UpdateVariantAsync(existingVariant);
                    itemResult.Success = true;
                    itemResult.Data = updatedVariant;
                }
            }
            catch (ValidationException ex)
            {
                itemResult.Success = false;
                itemResult.Errors = ex.Errors.SelectMany(kvp => kvp.Value).ToArray();
            }
            catch (Exception ex)
            {
                itemResult.Success = false;
                itemResult.Errors = new[] { ex.Message };
            }

            results.Add(itemResult);
            index++;
        }

        var batchResult = BatchOperationService.CreateResult(request.IdempotencyKey, request.OnConflict, results);
        
        // Save the operation result for idempotency
        await _batchOperationService.SaveOperationResultAsync(request.IdempotencyKey, "UpdateVariants", batchResult);
        
        return batchResult;
    }

    public async Task<VariantOption> CreateVariantOptionAsync(VariantOption option)
    {
        // Set audit fields
        option.Id = Guid.NewGuid();
        option.CreatedAt = DateTime.UtcNow;
        option.UpdatedAt = DateTime.UtcNow;
        option.Status = EntityStatus.Active;

        return await _productRepository.AddVariantOptionAsync(option);
    }

    public async Task<VariantOption?> GetVariantOptionAsync(Guid id)
    {
        return await _productRepository.GetVariantOptionAsync(id);
    }

    public async Task<VariantOption?> GetVariantOptionByNameAsync(Guid productMasterId, string name)
    {
        var options = await _productRepository.GetVariantOptionsAsync(productMasterId);
        return options.FirstOrDefault(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<VariantOption>> GetVariantOptionsAsync(Guid productMasterId)
    {
        return await _productRepository.GetVariantOptionsAsync(productMasterId);
    }

    public async Task<VariantOption> UpdateVariantOptionAsync(VariantOption option)
    {
        option.UpdatedAt = DateTime.UtcNow;
        return await _productRepository.UpdateVariantOptionAsync(option);
    }

    public async Task DeleteVariantOptionAsync(Guid id)
    {
        await _productRepository.DeleteVariantOptionAsync(id);
    }

    public async Task<string> GenerateVariantCombinationKeyAsync(IEnumerable<VariantOptionValue> optionValues)
    {
        if (optionValues == null || !optionValues.Any())
        {
            return string.Empty;
        }

        // Sort by option name to ensure consistent key generation
        var sortedValues = optionValues
            .OrderBy(ov => ov.VariantOptionId)
            .Select(ov => $"{ov.VariantOptionId}:{ov.Value}")
            .ToList();

        return string.Join("|", sortedValues);
    }
}