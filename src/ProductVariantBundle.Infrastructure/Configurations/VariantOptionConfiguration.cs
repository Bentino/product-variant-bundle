using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class VariantOptionConfiguration : BaseEntityConfiguration<VariantOption>
{
    public override void Configure(EntityTypeBuilder<VariantOption> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("VariantOptions");
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(100);
            
        // Unique constraint on slug per product master
        builder.HasIndex(e => new { e.ProductMasterId, e.Slug })
            .IsUnique()
            .HasDatabaseName("uk_variant_option_slug_per_product");
            
        // Relationships
        builder.HasOne(e => e.ProductMaster)
            .WithMany()
            .HasForeignKey(e => e.ProductMasterId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.Values)
            .WithOne(e => e.VariantOption)
            .HasForeignKey(e => e.VariantOptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}