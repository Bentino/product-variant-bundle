using ProductVariantBundle.Api.DTOs.Products;
using ProductVariantBundle.Api.DTOs.Bundles;

namespace ProductVariantBundle.Api.DTOs.Batch;

public class BatchOperationResultDto<T>
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int TotalProcessed { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public ConflictResolutionStrategy OnConflict { get; set; }
    public IEnumerable<BatchItemResultDto<T>> Results { get; set; } = new List<BatchItemResultDto<T>>();
}

public class BatchItemResultDto<T>
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

// Batch Create Variants
public class BatchCreateVariantRequestDto
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public ConflictResolutionStrategy OnConflict { get; set; } = ConflictResolutionStrategy.Fail;
    public IEnumerable<CreateProductVariantDto> Items { get; set; } = new List<CreateProductVariantDto>();
}

// Batch Update Variants
public class BatchUpdateVariantRequestDto
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public ConflictResolutionStrategy OnConflict { get; set; } = ConflictResolutionStrategy.Fail;
    public IEnumerable<BatchUpdateVariantItemDto> Items { get; set; } = new List<BatchUpdateVariantItemDto>();
}

public class BatchUpdateVariantItemDto : UpdateProductVariantDto
{
    public Guid Id { get; set; }
}

// Batch Create Bundles
public class BatchCreateBundleRequestDto
{
    public string IdempotencyKey { get; set; } = string.Empty;
    public ConflictResolutionStrategy OnConflict { get; set; } = ConflictResolutionStrategy.Fail;
    public IEnumerable<CreateProductBundleDto> Items { get; set; } = new List<CreateProductBundleDto>();
}