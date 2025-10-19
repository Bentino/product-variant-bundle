using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class BatchOperationConfiguration : IEntityTypeConfiguration<BatchOperation>
{
    public void Configure(EntityTypeBuilder<BatchOperation> builder)
    {
        builder.ToTable("BatchOperations");

        // Base entity properties
        builder.Property(e => e.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnName("status");

        // Properties
        builder.Property(e => e.IdempotencyKey)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("idempotency_key");

        builder.Property(e => e.OperationType)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("operation_type");

        builder.Property(e => e.TotalItems)
            .IsRequired()
            .HasColumnName("total_items");

        builder.Property(e => e.SuccessCount)
            .IsRequired()
            .HasColumnName("success_count");

        builder.Property(e => e.FailureCount)
            .IsRequired()
            .HasColumnName("failure_count");

        builder.Property(e => e.ResultData)
            .HasColumnType("jsonb")
            .HasColumnName("result_data");

        builder.Property(e => e.ExpiresAt)
            .IsRequired()
            .HasColumnName("expires_at");

        // Indexes
        builder.HasIndex(e => e.IdempotencyKey)
            .IsUnique()
            .HasDatabaseName("ix_batch_operations_idempotency_key");

        builder.HasIndex(e => e.ExpiresAt)
            .HasDatabaseName("ix_batch_operations_expires_at");

        builder.HasIndex(e => e.OperationType)
            .HasDatabaseName("ix_batch_operations_operation_type");
    }
}