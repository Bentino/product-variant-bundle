using Microsoft.EntityFrameworkCore;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Infrastructure.Data;

namespace ProductVariantBundle.Infrastructure.Repositories;

public class BatchOperationRepository : IBatchOperationRepository
{
    private readonly ApplicationDbContext _context;

    public BatchOperationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BatchOperation?> GetByIdempotencyKeyAsync(string idempotencyKey)
    {
        return await _context.BatchOperations
            .Where(bo => bo.Status == EntityStatus.Active)
            .FirstOrDefaultAsync(bo => bo.IdempotencyKey == idempotencyKey);
    }

    public async Task<BatchOperation> AddAsync(BatchOperation batchOperation)
    {
        _context.BatchOperations.Add(batchOperation);
        await _context.SaveChangesAsync();
        return batchOperation;
    }

    public async Task UpdateAsync(BatchOperation batchOperation)
    {
        _context.BatchOperations.Update(batchOperation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpiredAsync(DateTime cutoffDate)
    {
        var expiredOperations = await _context.BatchOperations
            .Where(bo => bo.ExpiresAt < cutoffDate)
            .ToListAsync();

        foreach (var operation in expiredOperations)
        {
            operation.Status = EntityStatus.Archived;
            operation.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}