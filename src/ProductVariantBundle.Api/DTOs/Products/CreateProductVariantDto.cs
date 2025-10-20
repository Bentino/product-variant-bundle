using System.ComponentModel.DataAnnotations;

namespace ProductVariantBundle.Api.DTOs.Products;

/// <summary>
/// Custom validation attribute for CreateVariantOptionValueDto
/// </summary>
public class VariantOptionValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is CreateVariantOptionValueDto dto)
        {
            // Either VariantOptionId or OptionName must be provided
            return dto.VariantOptionId.HasValue || !string.IsNullOrWhiteSpace(dto.OptionName);
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return "Either VariantOptionId or OptionName must be provided";
    }
}

/// <summary>
/// DTO for creating a new product variant
/// </summary>
public class CreateProductVariantDto
{
    /// <summary>
    /// Product master ID that this variant belongs to
    /// </summary>
    [Required]
    public Guid ProductMasterId { get; set; }

    /// <summary>
    /// Unique SKU for this variant
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string SKU { get; set; } = string.Empty;

    /// <summary>
    /// Price for this variant
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// Additional attributes for this variant (stored as JSONB)
    /// </summary>
    public Dictionary<string, object>? Attributes { get; set; }

    /// <summary>
    /// Option values that define this variant (e.g., Size: Large, Color: Red)
    /// </summary>
    [Required]
    [MinLength(1, ErrorMessage = "At least one option value is required")]
    public List<CreateVariantOptionValueDto> OptionValues { get; set; } = new();
}

/// <summary>
/// DTO for creating variant option values
/// </summary>
[VariantOptionValidation]
public class CreateVariantOptionValueDto
{
    /// <summary>
    /// ID of the variant option (e.g., Size, Color) - Optional if OptionName is provided
    /// </summary>
    public Guid? VariantOptionId { get; set; }

    /// <summary>
    /// Name of the variant option (e.g., "Size", "Color") - Used to find or create VariantOption
    /// </summary>
    [StringLength(100, MinimumLength = 1)]
    public string? OptionName { get; set; }

    /// <summary>
    /// Value for this option (e.g., "Large", "Red")
    /// </summary>
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Value { get; set; } = string.Empty;
}