using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.DTOs.Products;

public class ProductMasterDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public JsonDocument? Attributes { get; set; }
    public ICollection<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
}

public class CreateProductMasterDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
    public string? Slug { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string Category { get; set; } = string.Empty;

    public JsonDocument? Attributes { get; set; }
}

public class UpdateProductMasterDto
{
    [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
    public string? Name { get; set; }

    [StringLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
    public string? Slug { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }

    public JsonDocument? Attributes { get; set; }
}