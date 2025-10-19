using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class ProductMasterConfiguration : BaseEntityConfiguration<ProductMaster>
{
    public override void Configure(EntityTypeBuilder<ProductMaster> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("ProductMasters");
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(e => e.Description)
            .HasMaxLength(2000);
            
        builder.Property(e => e.Category)
            .HasMaxLength(100);
            
        builder.Property(e => e.Attributes)
            .HasColumnType("jsonb");
            
        // Unique constraint on slug
        builder.HasIndex(e => e.Slug)
            .IsUnique()
            .HasDatabaseName("uk_product_master_slug");
            
        // GIN index for JSONB attributes
        builder.HasIndex(e => e.Attributes)
            .HasMethod("gin")
            .HasDatabaseName("idx_product_master_attributes");
            
        // Relationships
        builder.HasMany(e => e.Variants)
            .WithOne(e => e.ProductMaster)
            .HasForeignKey(e => e.ProductMasterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}