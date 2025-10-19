using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.DTOs.Products;

public class VariantOptionValueDto : BaseDto
{
    public string Value { get; set; } = string.Empty;
    public Guid VariantOptionId { get; set; }
    public string OptionName { get; set; } = string.Empty;
    public string OptionSlug { get; set; } = string.Empty;
    public Guid ProductVariantId { get; set; }
}