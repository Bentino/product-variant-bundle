using System.Text.Json;

namespace ProductVariantBundle.Core.Entities;

public class ProductVariant : BaseEntity
{
    public decimal Price { get; set; }
    public string CombinationKey { get; set; } = string.Empty;
    public Guid ProductMasterId { get; set; }
    public ProductMaster ProductMaster { get; set; } = null!;
    public JsonDocument? Attributes { get; set; }
    public ICollection<VariantOptionValue> OptionValues { get; set; } = new List<VariantOptionValue>();
    public SellableItem? SellableItem { get; set; }
}