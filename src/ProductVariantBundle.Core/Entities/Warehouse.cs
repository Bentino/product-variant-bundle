using System.Text.Json;

namespace ProductVariantBundle.Core.Entities;

public class Warehouse : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public JsonDocument? Metadata { get; set; }
    public ICollection<InventoryRecord> InventoryRecords { get; set; } = new List<InventoryRecord>();
}