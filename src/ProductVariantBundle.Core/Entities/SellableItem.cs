using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Core.Entities;

public class SellableItem : BaseEntity
{
    public string SKU { get; set; } = string.Empty;
    public SellableItemType Type { get; set; }
    public Guid? VariantId { get; set; }
    public Guid? BundleId { get; set; }
    public ProductVariant? Variant { get; set; }
    public ProductBundle? Bundle { get; set; }
    public ICollection<InventoryRecord> InventoryRecords { get; set; } = new List<InventoryRecord>();
    public ICollection<BundleItem> BundleItems { get; set; } = new List<BundleItem>();
}