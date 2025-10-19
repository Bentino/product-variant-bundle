using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Core.Interfaces;

public interface IBatchOperationRepository
{
    Task<BatchOperation?> GetByIdempotencyKeyAsync(string idempotencyKey);
    Task<BatchOperation> AddAsync(BatchOperation batchOperation);
    Task UpdateAsync(BatchOperation batchOperation);
    Task DeleteExpiredAsync(DateTime cutoffDate);
}