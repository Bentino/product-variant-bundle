namespace ProductVariantBundle.Core.Models;

public class BundleAvailability
{
    public Guid BundleId { get; set; }
    public int AvailableQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public string WarehouseCode { get; set; } = "MAIN";
    public ICollection<ComponentAvailability> Components { get; set; } = new List<ComponentAvailability>();
}

public class ComponentAvailability
{
    public Guid SellableItemId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int Required { get; set; }
    public int Available { get; set; }
    public int CanFulfill { get; set; }
}