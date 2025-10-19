namespace ProductVariantBundle.Core.Entities;

public class InventoryRecord : BaseEntity
{
    public Guid SellableItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public SellableItem SellableItem { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}