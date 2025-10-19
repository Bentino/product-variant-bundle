using System.ComponentModel.DataAnnotations;

namespace ProductVariantBundle.Api.DTOs.Products;

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
public class CreateVariantOptionValueDto
{
    /// <summary>
    /// ID of the variant option (e.g., Size, Color)
    /// </summary>
    [Required]
    public Guid VariantOptionId { get; set; }

    /// <summary>
    /// Value for this option (e.g., "Large", "Red")
    /// </summary>
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Value { get; set; } = string.Empty;
}