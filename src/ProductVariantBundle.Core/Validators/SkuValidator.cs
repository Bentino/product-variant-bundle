using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using System.Text.RegularExpressions;

namespace ProductVariantBundle.Core.Validators;

public class SkuValidator
{
    private readonly ISellableItemRepository _sellableItemRepository;
    private static readonly Regex SkuPattern = new Regex(@"^[A-Z0-9\-_]{3,50}$", RegexOptions.Compiled);

    public SkuValidator(ISellableItemRepository sellableItemRepository)
    {
        _sellableItemRepository = sellableItemRepository;
    }

    public async Task ValidateSkuAsync(string sku, Guid? excludeId = null)
    {
        var errors = new Dictionary<string, List<string>>();

        // Validate format
        if (string.IsNullOrWhiteSpace(sku))
        {
            errors.AddError("SKU", "SKU is required");
        }
        else
        {
            if (!SkuPattern.IsMatch(sku))
            {
                errors.AddError("SKU", "SKU must be 3-50 characters long and contain only uppercase letters, numbers, hyphens, and underscores");
            }

            // Validate uniqueness
            if (await _sellableItemRepository.SKUExistsAsync(sku, excludeId))
            {
                errors.AddError("SKU", $"SKU '{sku}' already exists");
            }
        }

        if (errors.Any())
        {
            throw new ValidationException(errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
        }
    }

    public static string NormalizeSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return string.Empty;

        return sku.Trim().ToUpperInvariant();
    }
}