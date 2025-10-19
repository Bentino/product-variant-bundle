using System.Text.Json;

namespace ProductVariantBundle.Core.Entities;

public class ProductBundle : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public JsonDocument? Metadata { get; set; }
    public SellableItem? SellableItem { get; set; }
    public ICollection<BundleItem> Items { get; set; } = new List<BundleItem>();
}