using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class ProductVariantConfiguration : BaseEntityConfiguration<ProductVariant>
{
    public override void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("ProductVariants");
        
        builder.Property(e => e.Price)
            .HasPrecision(18, 2);
            
        builder.Property(e => e.CombinationKey)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(e => e.Attributes)
            .HasColumnType("jsonb");
            
        // Unique constraint on variant combination per product master
        builder.HasIndex(e => new { e.ProductMasterId, e.CombinationKey })
            .IsUnique()
            .HasDatabaseName("uk_variant_combination");
            
        // GIN index for JSONB attributes
        builder.HasIndex(e => e.Attributes)
            .HasMethod("gin")
            .HasDatabaseName("idx_product_variant_attributes");
            
        // Relationships
        builder.HasOne(e => e.ProductMaster)
            .WithMany(e => e.Variants)
            .HasForeignKey(e => e.ProductMasterId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // OptionValues relationship
        builder.HasMany(e => e.OptionValues)
            .WithOne(e => e.ProductVariant)
            .HasForeignKey(e => e.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.SellableItem)
            .WithOne(e => e.Variant)
            .HasForeignKey<SellableItem>(e => e.VariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}