namespace ProductVariantBundle.Core.Models;

public class BatchOperationResult<T>
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int TotalProcessed { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public ConflictResolutionStrategy OnConflict { get; set; }
    public IEnumerable<BatchItemResult<T>> Results { get; set; } = new List<BatchItemResult<T>>();
}

public class BatchItemResult<T>
{
    public int Index { get; set; }
    public bool Success { get; set; }
    public T? Data { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();
}

public enum ConflictResolutionStrategy
{
    Fail,
    Skip,
    Update
}

public class BatchCreateVariantRequest
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public ConflictResolutionStrategy OnConflict { get; set; } = ConflictResolutionStrategy.Fail;
    public IEnumerable<ProductVariantBatchItem> Items { get; set; } = new List<ProductVariantBatchItem>();
}

public class BatchUpdateVariantRequest
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public ConflictResolutionStrategy OnConflict { get; set; } = ConflictResolutionStrategy.Fail;
    public IEnumerable<ProductVariantUpdateBatchItem> Items { get; set; } = new List<ProductVariantUpdateBatchItem>();
}

public class BatchCreateBundleRequest
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public ConflictResolutionStrategy OnConflict { get; set; } = ConflictResolutionStrategy.Fail;
    public IEnumerable<ProductBundleBatchItem> Items { get; set; } = new List<ProductBundleBatchItem>();
}

public class ProductVariantBatchItem
{
    public Guid ProductMasterId { get; set; }
    public decimal Price { get; set; }
    public string SKU { get; set; } = string.Empty;
    public IEnumerable<VariantOptionValueBatchItem> OptionValues { get; set; } = new List<VariantOptionValueBatchItem>();
}

public class ProductVariantUpdateBatchItem
{
    public Guid Id { get; set; }
    public decimal? Price { get; set; }
    public string? SKU { get; set; }
    public IEnumerable<VariantOptionValueBatchItem>? OptionValues { get; set; }
}

public class ProductBundleBatchItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SKU { get; set; } = string.Empty;
    public IEnumerable<BundleItemBatchItem> Items { get; set; } = new List<BundleItemBatchItem>();
}

public class VariantOptionValueBatchItem
{
    public Guid VariantOptionId { get; set; }
    public string Value { get; set; } = string.Empty;
}

public class BundleItemBatchItem
{
    public Guid SellableItemId { get; set; }
    public int Quantity { get; set; }
}