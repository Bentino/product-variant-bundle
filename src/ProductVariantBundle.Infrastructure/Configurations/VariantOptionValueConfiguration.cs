using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Configurations;

public class VariantOptionValueConfiguration : BaseEntityConfiguration<VariantOptionValue>
{
    public override void Configure(EntityTypeBuilder<VariantOptionValue> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("VariantOptionValues");
        
        builder.Property(e => e.Value)
            .IsRequired()
            .HasMaxLength(100);
            
        // Unique constraint on value per variant option
        builder.HasIndex(e => new { e.VariantOptionId, e.Value })
            .IsUnique()
            .HasDatabaseName("uk_option_value_per_option");
            
        // Relationships
        builder.HasOne(e => e.VariantOption)
            .WithMany(e => e.Values)
            .HasForeignKey(e => e.VariantOptionId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(e => e.ProductVariant)
            .WithMany(e => e.OptionValues)
            .HasForeignKey(e => e.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}