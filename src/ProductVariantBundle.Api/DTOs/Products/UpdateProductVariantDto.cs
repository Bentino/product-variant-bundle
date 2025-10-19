using System.ComponentModel.DataAnnotations;

namespace ProductVariantBundle.Api.DTOs.Products;

/// <summary>
/// DTO for updating an existing product variant
/// </summary>
public class UpdateProductVariantDto
{
    /// <summary>
    /// Updated SKU for this variant (must be unique)
    /// </summary>
    [StringLength(100, MinimumLength = 3)]
    public string? SKU { get; set; }

    /// <summary>
    /// Updated price for this variant
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Updated attributes for this variant (stored as JSONB)
    /// </summary>
    public Dictionary<string, object>? Attributes { get; set; }

    /// <summary>
    /// Updated option values that define this variant
    /// Note: Changing option values may affect variant uniqueness
    /// </summary>
    public List<UpdateVariantOptionValueDto>? OptionValues { get; set; }
}

/// <summary>
/// DTO for updating variant option values
/// </summary>
public class UpdateVariantOptionValueDto
{
    /// <summary>
    /// ID of the variant option value to update
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Updated value for this option
    /// </summary>
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Value { get; set; } = string.Empty;
}