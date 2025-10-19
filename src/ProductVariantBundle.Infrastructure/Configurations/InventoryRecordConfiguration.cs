using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class InventoryRecordConfiguration : BaseEntityConfiguration<InventoryRecord>
{
    public override void Configure(EntityTypeBuilder<InventoryRecord> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("InventoryRecords");
        
        builder.Property(e => e.OnHand)
            .HasDefaultValue(0);
            
        builder.Property(e => e.Reserved)
            .HasDefaultValue(0);
            
        // Warehouse is required (NOT NULL)
        builder.Property(e => e.WarehouseId)
            .IsRequired();
            
        // Unique constraint: one inventory record per sellable item per warehouse
        builder.HasIndex(e => new { e.SellableItemId, e.WarehouseId })
            .IsUnique()
            .HasDatabaseName("uk_inventory_sellable_warehouse");
            
        // Relationships
        builder.HasOne(e => e.SellableItem)
            .WithMany(e => e.InventoryRecords)
            .HasForeignKey(e => e.SellableItemId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.Warehouse)
            .WithMany(e => e.InventoryRecords)
            .HasForeignKey(e => e.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}