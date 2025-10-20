namespace ProductVariantBundle.Core.Models;

public class InventoryStats
{
    public int TotalItems { get; set; }
    public int InStock { get; set; }
    public int LowStock { get; set; }
    public int OutOfStock { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public int LowStockThreshold { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}