using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class BundleItemConfiguration : BaseEntityConfiguration<BundleItem>
{
    public override void Configure(EntityTypeBuilder<BundleItem> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("BundleItems");
        
        builder.Property(e => e.Quantity)
            .IsRequired();
            
        // Unique constraint: one sellable item per bundle
        builder.HasIndex(e => new { e.BundleId, e.SellableItemId })
            .IsUnique()
            .HasDatabaseName("uk_bundle_sellable_item");
            
        // Positive quantity constraint
        builder.ToTable(t => t.HasCheckConstraint("ck_quantity_positive", "quantity > 0"));
        
        // Relationships
        builder.HasOne(e => e.Bundle)
            .WithMany(e => e.Items)
            .HasForeignKey(e => e.BundleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.SellableItem)
            .WithMany(e => e.BundleItems)
            .HasForeignKey(e => e.SellableItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}