using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.DTOs.Inventory;

public class InventoryRecordDto : BaseDto
{
    public Guid SellableItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public int Available => OnHand - Reserved;
}

public class UpdateInventoryDto
{
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string WarehouseCode { get; set; } = "MAIN";
}

public class ReserveInventoryDto
{
    public string SKU { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string WarehouseCode { get; set; } = "MAIN";
}

public class UpdateStockDto
{
    public string SKU { get; set; } = string.Empty;
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public string WarehouseCode { get; set; } = "MAIN";
}

public class StockInquiryDto
{
    public string SKU { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = "MAIN";
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public int Available { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class CreateInventoryRecordDto
{
    public Guid SellableItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public int OnHand { get; set; } = 0;
    public int Reserved { get; set; } = 0;
}

public class CreateInventoryBySKUDto
{
    public string SKU { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = "MAIN";
    public int OnHand { get; set; } = 0;
    public int Reserved { get; set; } = 0;
}