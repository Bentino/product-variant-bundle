using System.ComponentModel.DataAnnotations;
using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.DTOs.Products;

public class VariantOptionDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid ProductMasterId { get; set; }
    public ICollection<VariantOptionValueDto> Values { get; set; } = new List<VariantOptionValueDto>();
}

public class CreateVariantOptionDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required")]
    [StringLength(100, ErrorMessage = "Slug cannot exceed 100 characters")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug must contain only lowercase letters, numbers, and hyphens")]
    public string Slug { get; set; } = string.Empty;
}

public class UpdateVariantOptionDto
{
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string? Name { get; set; }

    [StringLength(100, ErrorMessage = "Slug cannot exceed 100 characters")]
    [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug must contain only lowercase letters, numbers, and hyphens")]
    public string? Slug { get; set; }
}