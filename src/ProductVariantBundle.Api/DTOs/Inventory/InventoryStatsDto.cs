namespace ProductVariantBundle.Api.DTOs.Inventory;

public class InventoryStatsDto
{
    public int TotalItems { get; set; }
    public int InStock { get; set; }
    public int LowStock { get; set; }
    public int OutOfStock { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public int LowStockThreshold { get; set; } = 10;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}