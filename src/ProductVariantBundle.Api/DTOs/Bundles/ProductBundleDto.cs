using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.DTOs.Bundles;

public class ProductBundleDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SKU { get; set; } = string.Empty;
    public JsonDocument? Metadata { get; set; }
    public ICollection<BundleItemDto> Items { get; set; } = new List<BundleItemDto>();
}

public class CreateProductBundleDto
{
    [Required(ErrorMessage = "Bundle name is required")]
    [StringLength(200, ErrorMessage = "Bundle name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    [JsonPropertyName("sku")]
    public string SKU { get; set; } = string.Empty;

    public JsonDocument? Metadata { get; set; }

    [Required(ErrorMessage = "At least one bundle item is required")]
    [MinLength(1, ErrorMessage = "At least one bundle item is required")]
    [JsonPropertyName("items")]
    public ICollection<CreateBundleItemDto> Items { get; set; } = new List<CreateBundleItemDto>();
}

public class UpdateProductBundleDto
{
    [StringLength(200, ErrorMessage = "Bundle name cannot exceed 200 characters")]
    public string? Name { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }

    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    public string? SKU { get; set; }

    public JsonDocument? Metadata { get; set; }

    public ICollection<CreateBundleItemDto>? Items { get; set; }
}