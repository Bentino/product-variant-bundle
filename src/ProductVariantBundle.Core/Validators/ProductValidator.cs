using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Helpers;
using ProductVariantBundle.Core.Interfaces;

namespace ProductVariantBundle.Core.Validators;

public class ProductValidator
{
    private readonly IProductRepository _productRepository;

    public ProductValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task ValidateProductMasterAsync(ProductMaster productMaster, bool isUpdate = false)
    {
        var errors = new Dictionary<string, List<string>>();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(productMaster.Name))
        {
            errors.AddError("Name", "Name is required");
        }

        if (string.IsNullOrWhiteSpace(productMaster.Slug))
        {
            errors.AddError("Slug", "Slug is required");
        }
        else
        {
            // Normalize slug
            var normalizedSlug = SlugHelper.Normalize(productMaster.Slug);
            if (string.IsNullOrEmpty(normalizedSlug))
            {
                errors.AddError("Slug", "Slug cannot be empty after normalization");
            }
            else
            {
                productMaster.Slug = normalizedSlug;
                
                // Check uniqueness
                var excludeId = isUpdate ? productMaster.Id : (Guid?)null;
                if (await _productRepository.SlugExistsAsync(normalizedSlug, excludeId))
                {
                    errors.AddError("Slug", $"Slug '{normalizedSlug}' already exists");
                }
            }
        }

        if (errors.Any())
        {
            throw new ValidationException(errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
        }
    }

    public async Task ValidateProductVariantAsync(ProductVariant variant, bool isUpdate = false)
    {
        var errors = new Dictionary<string, List<string>>();

        // Validate price
        if (variant.Price < 0)
        {
            errors.AddError("Price", "Price cannot be negative");
        }

        // Validate product master exists
        var productMaster = await _productRepository.GetByIdAsync(variant.ProductMasterId);
        if (productMaster == null)
        {
            errors.AddError("ProductMasterId", "Product master not found");
        }

        // Validate option values
        if (!variant.OptionValues.Any())
        {
            errors.AddError("OptionValues", "Variant must have at least one option value");
        }

        // Generate and validate combination key
        if (variant.OptionValues.Any())
        {
            variant.CombinationKey = GenerateVariantCombinationKey(variant.OptionValues);
            
            var excludeId = isUpdate ? variant.Id : (Guid?)null;
            if (await _productRepository.VariantCombinationExistsAsync(variant.ProductMasterId, variant.CombinationKey, excludeId))
            {
                errors.AddError("OptionValues", "This combination of option values already exists for this product");
            }
        }
        else
        {
            // Set a default combination key if no option values
            if (string.IsNullOrEmpty(variant.CombinationKey))
            {
                variant.CombinationKey = $"default-{Guid.NewGuid()}";
            }
        }

        if (errors.Any())
        {
            throw new ValidationException(errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
        }
    }

    public static string GenerateVariantCombinationKey(IEnumerable<VariantOptionValue> optionValues)
    {
        if (!optionValues.Any())
        {
            return string.Empty;
        }

        // Sort by option slug to ensure consistent ordering
        var sortedValues = optionValues
            .OrderBy(ov => ov.VariantOption.Slug)
            .Select(ov => $"{ov.VariantOption.Slug}:{ov.Value.Trim().ToLowerInvariant()}");

        return string.Join("|", sortedValues);
    }
}

public static class ValidationExtensions
{
    public static void AddError(this Dictionary<string, List<string>> errors, string key, string message)
    {
        if (!errors.ContainsKey(key))
        {
            errors[key] = new List<string>();
        }
        errors[key].Add(message);
    }
}