using System.Text.Json;

namespace ProductVariantBundle.Core.Entities;

public class BatchOperation : BaseEntity
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public JsonDocument? ResultData { get; set; }
    public DateTime ExpiresAt { get; set; }
}