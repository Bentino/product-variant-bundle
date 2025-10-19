using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class WarehouseConfiguration : BaseEntityConfiguration<Warehouse>
{
    public override void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Warehouses");
        
        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(e => e.Address)
            .HasMaxLength(500);
            
        builder.Property(e => e.Metadata)
            .HasColumnType("jsonb");
            
        // Unique constraint on warehouse code
        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("uk_warehouse_code");
            
        // Relationships
        builder.HasMany(e => e.InventoryRecords)
            .WithOne(e => e.Warehouse)
            .HasForeignKey(e => e.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Seed default warehouse data
        builder.HasData(new Warehouse
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Code = "MAIN",
            Name = "Main Warehouse",
            Address = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = Core.Enums.EntityStatus.Active
        });
    }
}