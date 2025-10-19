using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Infrastructure.Configurations;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()");
            
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW()");
            
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("NOW()");
            
        builder.Property(e => e.Status)
            .HasConversion<int>()
            .HasDefaultValue(EntityStatus.Active);
    }
}