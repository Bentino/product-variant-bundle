using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using System.Text.Json;

namespace ProductVariantBundle.Core.Services;

public class BatchOperationService
{
    private readonly IBatchOperationRepository _batchOperationRepository;
    private static readonly TimeSpan DefaultExpiryTime = TimeSpan.FromHours(24);

    public BatchOperationService(IBatchOperationRepository batchOperationRepository)
    {
        _batchOperationRepository = batchOperationRepository;
    }

    public async Task<BatchOperationResult<T>?> GetExistingOperationAsync<T>(string idempotencyKey)
    {
        var existing = await _batchOperationRepository.GetByIdempotencyKeyAsync(idempotencyKey);
        if (existing == null || existing.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        // Deserialize the result data
        if (existing.ResultData != null)
        {
            var resultData = JsonSerializer.Deserialize<BatchOperationResult<T>>(existing.ResultData.RootElement.GetRawText());
            return resultData;
        }

        return null;
    }

    public async Task<BatchOperationResult<T>> SaveOperationResultAsync<T>(
        string idempotencyKey, 
        string operationType, 
        BatchOperationResult<T> result)
    {
        var batchOperation = new BatchOperation
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = idempotencyKey,
            OperationType = operationType,
            TotalItems = result.TotalProcessed,
            SuccessCount = result.SuccessCount,
            FailureCount = result.FailureCount,
            ResultData = JsonDocument.Parse(JsonSerializer.Serialize(result)),
            ExpiresAt = DateTime.UtcNow.Add(DefaultExpiryTime),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = EntityStatus.Active
        };

        await _batchOperationRepository.AddAsync(batchOperation);
        return result;
    }

    public async Task CleanupExpiredOperationsAsync()
    {
        await _batchOperationRepository.DeleteExpiredAsync(DateTime.UtcNow);
    }

    public static BatchOperationResult<T> CreateResult<T>(
        string idempotencyKey, 
        ConflictResolutionStrategy onConflict,
        IEnumerable<BatchItemResult<T>> results)
    {
        var resultList = results.ToList();
        return new BatchOperationResult<T>
        {
            IdempotencyKey = idempotencyKey,
            OnConflict = onConflict,
            TotalProcessed = resultList.Count,
            SuccessCount = resultList.Count(r => r.Success),
            FailureCount = resultList.Count(r => !r.Success),
            Results = resultList
        };
    }
}