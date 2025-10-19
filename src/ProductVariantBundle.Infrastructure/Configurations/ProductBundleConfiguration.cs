using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class ProductBundleConfiguration : BaseEntityConfiguration<ProductBundle>
{
    public override void Configure(EntityTypeBuilder<ProductBundle> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("ProductBundles");
        
        // Override Status to use integer conversion for ProductBundles table
        builder.Property(e => e.Status)
            .HasConversion<int>()
            .HasDefaultValue(EntityStatus.Active);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(e => e.Description)
            .HasMaxLength(2000);
            
        builder.Property(e => e.Price)
            .HasPrecision(18, 2);
            
        builder.Property(e => e.Metadata)
            .HasColumnType("jsonb");
            
        // GIN index for JSONB metadata
        builder.HasIndex(e => e.Metadata)
            .HasMethod("gin")
            .HasDatabaseName("idx_bundle_metadata");
            
        // Relationships
        builder.HasOne(e => e.SellableItem)
            .WithOne(e => e.Bundle)
            .HasForeignKey<SellableItem>(e => e.BundleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Items)
            .WithOne(e => e.Bundle)
            .HasForeignKey(e => e.BundleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}