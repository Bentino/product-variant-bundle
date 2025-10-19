using Microsoft.EntityFrameworkCore;
using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ProductMaster> ProductMasters { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<VariantOption> VariantOptions { get; set; }
    public DbSet<VariantOptionValue> VariantOptionValues { get; set; }
    public DbSet<ProductBundle> ProductBundles { get; set; }
    public DbSet<BundleItem> BundleItems { get; set; }
    public DbSet<SellableItem> SellableItems { get; set; }
    public DbSet<InventoryRecord> InventoryRecords { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<BatchOperation> BatchOperations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}