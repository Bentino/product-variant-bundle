using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ProductVariantBundle.Infrastructure.Data;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IProductService _productService;
    private readonly IInventoryService _inventoryService;

    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ApplicationDbContext context,
        IProductService productService,
        IInventoryService inventoryService,
        ILogger<AdminController> logger)
    {
        _context = context;
        _productService = productService;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpPost("reset-data")]
    public async Task<IActionResult> ResetData()
    {
        try
        {
            _logger.LogInformation("Starting data reset process (purge + create sample data)");
            
            // First purge all data
            var purgeResult = await PurgeAllDataAsync();
            
            // Then create sample data
            var sampleResult = await CreateSampleDataAsync();
            
            _logger.LogInformation("Data reset completed successfully");
            
            return Ok(new
            {
                success = true,
                message = "Data reset completed with sample data",
                purged = purgeResult,
                created = sampleResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while resetting data");
            
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to reset data",
                error = ex.Message
            });
        }
    }

    [HttpPost("purge-data")]
    public async Task<IActionResult> PurgeData()
    {
        try
        {
            _logger.LogInformation("Starting data purge process (clear all data)");
            
            var result = await PurgeAllDataAsync();
            
            _logger.LogInformation("Data purge completed successfully");
            
            return Ok(new
            {
                success = true,
                message = "All data purged successfully",
                summary = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while purging data");
            
            return StatusCode(500, new
            {
                success = false,
                message = "Failed to purge data",
                error = ex.Message
            });
        }
    }

    private async Task<object> PurgeAllDataAsync()
    {
        _logger.LogInformation("Starting data purge via database truncate");

        // Use database truncate for fast cleanup - order matters due to foreign key constraints
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"BundleItems\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"ProductBundles\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"InventoryRecords\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"SellableItems\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"VariantOptionValues\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"ProductVariants\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"VariantOptions\" CASCADE");
        await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"ProductMasters\" CASCADE");

        _logger.LogInformation("Data purged successfully - all tables truncated");

        return new
        {
            productsDeleted = 0,
            variantsDeleted = 0,
            bundlesDeleted = 0,
            inventoryRecordsDeleted = 0
        };
    }

    private async Task<SampleDataResult> CreateSampleDataAsync()
    {
        _logger.LogInformation("Creating sample data");

        var result = new SampleDataResult();

        try
        {
            // Create default warehouse first
            var existingWarehouse = await _context.Warehouses
                .FirstOrDefaultAsync(w => w.Code == "MAIN");

            if (existingWarehouse == null)
            {
                var warehouse = new Warehouse
                {
                    Code = "MAIN",
                    Name = "Main Warehouse",
                    Address = "123 Main Street, City, Country"
                };

                _context.Warehouses.Add(warehouse);
                await _context.SaveChangesAsync();
                result.WarehousesCreated++;

                _logger.LogInformation("Created warehouse: {Code} - {Name}", warehouse.Code, warehouse.Name);
            }

            // Create sample products
            await CreateSampleProductsAsync(result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sample data");
            throw;
        }
    }

    private async Task CreateSampleProductsAsync(SampleDataResult result)
    {
        try
        {
            // T-Shirt Product
            var tshirt = new ProductMaster
            {
                Name = "T-Shirt",
                Slug = "t-shirt",
                Description = "Comfortable cotton t-shirt",
                Category = "Clothing",
                Attributes = JsonSerializer.SerializeToDocument(new { material = "cotton", care = "machine wash" })
            };

            var createdTshirt = await _productService.CreateProductMasterAsync(tshirt);
            result.ProductsCreated++;
            _logger.LogInformation("Created product: {Name}", tshirt.Name);

            // T-Shirt Variants
            await CreateTshirtVariantsAsync(createdTshirt.Id, result);

            // Gaming Keyboard Product
            var keyboard = new ProductMaster
            {
                Name = "Gaming Mechanical Keyboard",
                Slug = "gaming-mechanical-keyboard", 
                Description = "RGB backlit mechanical gaming keyboard with Cherry MX switches",
                Category = "Computer Accessories",
                Attributes = JsonSerializer.SerializeToDocument(new { 
                    brand = "TechGear", 
                    switchType = "Cherry MX Blue", 
                    connectivity = "USB-C",
                    backlight = "RGB",
                    layout = "Full Size",
                    warranty = "2 years"
                })
            };

            var createdKeyboard = await _productService.CreateProductMasterAsync(keyboard);
            result.ProductsCreated++;
            _logger.LogInformation("Created product: {Name}", keyboard.Name);

            // Keyboard Variants
            await CreateKeyboardVariantsAsync(createdKeyboard.Id, result);

            // Gaming Mouse Product
            var mouse = new ProductMaster
            {
                Name = "Gaming Mouse",
                Slug = "gaming-mouse",
                Description = "High-precision optical gaming mouse with customizable RGB lighting",
                Category = "Computer Accessories", 
                Attributes = JsonSerializer.SerializeToDocument(new {
                    brand = "ProGamer",
                    sensor = "Optical",
                    dpi = "16000",
                    buttons = "8",
                    connectivity = "Wireless/USB",
                    battery = "Rechargeable Li-ion",
                    warranty = "1 year"
                })
            };

            var createdMouse = await _productService.CreateProductMasterAsync(mouse);
            result.ProductsCreated++;
            _logger.LogInformation("Created product: {Name}", mouse.Name);

            // Mouse Variants
            await CreateMouseVariantsAsync(createdMouse.Id, result);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sample products");
            throw;
        }
    }

    private async Task CreateTshirtVariantsAsync(Guid productId, SampleDataResult result)
    {
        await CreateVariantAsync(productId, "TSHIRT-RED-S", 19.99m, 
            new[] { ("Color", "Red"), ("Size", "S") }, result);
        await CreateVariantAsync(productId, "TSHIRT-BLUE-M", 19.99m, 
            new[] { ("Color", "Blue"), ("Size", "M") }, result);
    }

    private async Task CreateKeyboardVariantsAsync(Guid productId, SampleDataResult result)
    {
        await CreateVariantAsync(productId, "KEYBOARD-BLACK-MX-BLUE", 129.99m, 
            new[] { ("Color", "Black"), ("Switch Type", "Cherry MX Blue") }, result);
        await CreateVariantAsync(productId, "KEYBOARD-WHITE-MX-RED", 139.99m, 
            new[] { ("Color", "White"), ("Switch Type", "Cherry MX Red") }, result);
    }

    private async Task CreateMouseVariantsAsync(Guid productId, SampleDataResult result)
    {
        await CreateVariantAsync(productId, "MOUSE-BLACK-WIRELESS", 89.99m, 
            new[] { ("Color", "Black"), ("Connection Type", "Wireless") }, result);
        await CreateVariantAsync(productId, "MOUSE-WHITE-WIRED", 79.99m, 
            new[] { ("Color", "White"), ("Connection Type", "Wired") }, result);
    }

    private async Task CreateVariantAsync(Guid productId, string sku, decimal price, 
        (string optionName, string value)[] options, SampleDataResult result)
    {
        try
        {
            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductMasterId = productId,
                Price = price
            };

            var optionValues = new List<VariantOptionValue>();
            
            foreach (var (optionName, value) in options)
            {
                var existingOption = await _productService.GetVariantOptionByNameAsync(productId, optionName);
                
                Guid variantOptionId;
                if (existingOption != null)
                {
                    variantOptionId = existingOption.Id;
                }
                else
                {
                    var newOption = new VariantOption
                    {
                        Name = optionName,
                        Slug = optionName.ToLowerInvariant().Replace(" ", "-"),
                        ProductMasterId = productId
                    };
                    
                    var createdOption = await _productService.CreateVariantOptionAsync(newOption);
                    variantOptionId = createdOption.Id;
                }
                
                optionValues.Add(new VariantOptionValue
                {
                    Id = Guid.NewGuid(),
                    VariantOptionId = variantOptionId,
                    Value = value,
                    ProductVariantId = variant.Id,
                    Status = EntityStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            
            variant.OptionValues = optionValues;
            
            // Create the variant
            var createdVariant = await _productService.CreateVariantAsync(variant);
            result.VariantsCreated++;

            // Create SellableItem directly
            var sellableItem = new SellableItem
            {
                Id = Guid.NewGuid(),
                SKU = sku,
                Type = SellableItemType.Variant,
                VariantId = createdVariant.Id,
                Status = EntityStatus.Active
            };

            _context.SellableItems.Add(sellableItem);
            await _context.SaveChangesAsync();

            // Create inventory record
            var warehouse = await _context.Warehouses.FirstAsync(w => w.Code == "MAIN");
                
            await _inventoryService.CreateInventoryRecordAsync(
                sellableItem.Id,
                warehouse.Id,
                onHand: 100,
                reserved: 0
            );

            _logger.LogInformation("Created variant: {SKU}", sku);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating variant {SKU}: {Error}", sku, ex.Message);
            throw;
        }
    }
}

public class SampleDataResult
{
    public int WarehousesCreated { get; set; } = 0;
    public int ProductsCreated { get; set; } = 0;
    public int VariantsCreated { get; set; } = 0;
}