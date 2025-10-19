using System.Text.Json;

namespace ProductVariantBundle.Core.Entities;

public class ProductMaster : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public JsonDocument? Attributes { get; set; }
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}