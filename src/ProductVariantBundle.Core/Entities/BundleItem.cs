namespace ProductVariantBundle.Core.Entities;

public class BundleItem : BaseEntity
{
    public Guid SellableItemId { get; set; }
    public int Quantity { get; set; }
    public Guid BundleId { get; set; }
    public ProductBundle Bundle { get; set; } = null!;
    public SellableItem SellableItem { get; set; } = null!;
}