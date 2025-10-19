using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class SellableItemConfiguration : BaseEntityConfiguration<SellableItem>
{
    public override void Configure(EntityTypeBuilder<SellableItem> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("SellableItems");
        
        builder.Property(e => e.SKU)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.Type)
            .HasConversion<string>()
            .IsRequired();
            
        // Global SKU uniqueness constraint
        builder.HasIndex(e => e.SKU)
            .IsUnique()
            .HasDatabaseName("uk_sellable_item_sku");
            
        // One-of constraint: either VariantId or BundleId must be set, but not both
        builder.ToTable(t => t.HasCheckConstraint("ck_sellable_item_one_of",
            "(\"VariantId\" IS NOT NULL AND \"BundleId\" IS NULL) OR (\"VariantId\" IS NULL AND \"BundleId\" IS NOT NULL)"));
            
        // Relationships
        builder.HasOne(e => e.Variant)
            .WithOne(e => e.SellableItem)
            .HasForeignKey<SellableItem>(e => e.VariantId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Bundle)
            .WithOne(e => e.SellableItem)
            .HasForeignKey<SellableItem>(e => e.BundleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.InventoryRecords)
            .WithOne(e => e.SellableItem)
            .HasForeignKey(e => e.SellableItemId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(e => e.BundleItems)
            .WithOne(e => e.SellableItem)
            .HasForeignKey(e => e.SellableItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}