using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.DTOs.Products;

public class ProductVariantDto : BaseDto
{
    public decimal Price { get; set; }
    public string CombinationKey { get; set; } = string.Empty;
    public Guid ProductMasterId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public JsonDocument? Attributes { get; set; }
    public ICollection<VariantOptionValueDto> OptionValues { get; set; } = new List<VariantOptionValueDto>();
}

