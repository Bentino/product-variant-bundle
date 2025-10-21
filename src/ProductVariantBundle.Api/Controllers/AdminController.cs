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

            // Create products by category
            var mobileProducts = await CreateMobileProductsAsync(result);
            var computerProducts = await CreateComputerComponentsAsync(result);
            var accessoryProducts = await CreateAccessoryProductsAsync(result);

            // Create bundles
            await CreateProductBundlesAsync(mobileProducts, computerProducts, accessoryProducts, result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sample data");
            throw;
        }
    }

    private async Task<List<Guid>> CreateMobileProductsAsync(SampleDataResult result)
    {
        var productIds = new List<Guid>();

        // iPhone 15 Pro
        var iphone = new ProductMaster
        {
            Name = "iPhone 15 Pro",
            Slug = "iphone-15-pro",
            Description = "Latest iPhone with titanium design and advanced camera system",
            Category = "Mobile Phone",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Apple", 
                os = "iOS 17", 
                display = "6.1-inch Super Retina XDR",
                camera = "48MP Main + 12MP Ultra Wide + 12MP Telephoto",
                chip = "A17 Pro",
                warranty = "1 year"
            })
        };
        var createdIphone = await _productService.CreateProductMasterAsync(iphone);
        productIds.Add(createdIphone.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdIphone.Id, "IPHONE15PRO-TITANIUM-128", 35900m, 
            new[] { ("Color", "Natural Titanium"), ("Storage", "128GB") }, 120, result);
        await CreateVariantWithStock(createdIphone.Id, "IPHONE15PRO-BLUE-256", 41900m, 
            new[] { ("Color", "Blue Titanium"), ("Storage", "256GB") }, 85, result);
        await CreateVariantWithStock(createdIphone.Id, "IPHONE15PRO-WHITE-512", 49900m, 
            new[] { ("Color", "White Titanium"), ("Storage", "512GB") }, 45, result);

        // Samsung Galaxy S24 Ultra
        var samsung = new ProductMaster
        {
            Name = "Samsung Galaxy S24 Ultra",
            Slug = "samsung-galaxy-s24-ultra",
            Description = "Premium Android flagship with S Pen and advanced AI features",
            Category = "Mobile Phone",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Samsung", 
                os = "Android 14", 
                display = "6.8-inch Dynamic AMOLED 2X",
                camera = "200MP Main + 50MP Periscope + 12MP Ultra Wide + 10MP Telephoto",
                chip = "Snapdragon 8 Gen 3",
                spen = "Yes",
                warranty = "1 year"
            })
        };
        var createdSamsung = await _productService.CreateProductMasterAsync(samsung);
        productIds.Add(createdSamsung.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdSamsung.Id, "S24ULTRA-BLACK-256", 42900m, 
            new[] { ("Color", "Titanium Black"), ("Storage", "256GB") }, 12, result);
        await CreateVariantWithStock(createdSamsung.Id, "S24ULTRA-GRAY-512", 49900m, 
            new[] { ("Color", "Titanium Gray"), ("Storage", "512GB") }, 8, result);
        await CreateVariantWithStock(createdSamsung.Id, "S24ULTRA-VIOLET-1TB", 56900m, 
            new[] { ("Color", "Titanium Violet"), ("Storage", "1TB") }, 5, result);

        // Google Pixel 8
        var pixel = new ProductMaster
        {
            Name = "Google Pixel 8",
            Slug = "google-pixel-8",
            Description = "Pure Android experience with advanced computational photography",
            Category = "Mobile Phone",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Google", 
                os = "Android 14", 
                display = "6.2-inch Actua Display",
                camera = "50MP Main + 12MP Ultra Wide",
                chip = "Google Tensor G3",
                warranty = "1 year"
            })
        };
        var createdPixel = await _productService.CreateProductMasterAsync(pixel);
        productIds.Add(createdPixel.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdPixel.Id, "PIXEL8-OBSIDIAN-128", 24900m, 
            new[] { ("Color", "Obsidian"), ("Storage", "128GB") }, 150, result);
        await CreateVariantWithStock(createdPixel.Id, "PIXEL8-HAZEL-256", 28900m, 
            new[] { ("Color", "Hazel"), ("Storage", "256GB") }, 95, result);
        await CreateVariantWithStock(createdPixel.Id, "PIXEL8-ROSE-256", 28900m, 
            new[] { ("Color", "Rose"), ("Storage", "256GB") }, 75, result);

        // OnePlus 12
        var oneplus = new ProductMaster
        {
            Name = "OnePlus 12",
            Slug = "oneplus-12",
            Description = "Flagship killer with fast charging and premium design",
            Category = "Mobile Phone",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "OnePlus", 
                os = "OxygenOS 14", 
                display = "6.82-inch LTPO AMOLED",
                camera = "50MP Main + 64MP Periscope + 48MP Ultra Wide",
                chip = "Snapdragon 8 Gen 3",
                charging = "100W SuperVOOC",
                warranty = "1 year"
            })
        };
        var createdOneplus = await _productService.CreateProductMasterAsync(oneplus);
        productIds.Add(createdOneplus.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdOneplus.Id, "ONEPLUS12-BLACK-256", 28900m, 
            new[] { ("Color", "Silky Black"), ("Storage", "256GB") }, 65, result);
        await CreateVariantWithStock(createdOneplus.Id, "ONEPLUS12-EMERALD-512", 32900m, 
            new[] { ("Color", "Flowy Emerald"), ("Storage", "512GB") }, 0, result); // Out of stock

        // Xiaomi 14
        var xiaomi = new ProductMaster
        {
            Name = "Xiaomi 14",
            Slug = "xiaomi-14",
            Description = "Premium smartphone with Leica camera partnership",
            Category = "Mobile Phone",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Xiaomi", 
                os = "MIUI 15", 
                display = "6.36-inch LTPO AMOLED",
                camera = "50MP Leica Main + 50MP Ultra Wide + 50MP Telephoto",
                chip = "Snapdragon 8 Gen 3",
                charging = "90W HyperCharge",
                warranty = "1 year"
            })
        };
        var createdXiaomi = await _productService.CreateProductMasterAsync(xiaomi);
        productIds.Add(createdXiaomi.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdXiaomi.Id, "XIAOMI14-BLACK-256", 22900m, 
            new[] { ("Color", "Black"), ("Storage", "256GB") }, 110, result);
        await CreateVariantWithStock(createdXiaomi.Id, "XIAOMI14-WHITE-512", 26900m, 
            new[] { ("Color", "White"), ("Storage", "512GB") }, 88, result);
        await CreateVariantWithStock(createdXiaomi.Id, "XIAOMI14-GREEN-512", 26900m, 
            new[] { ("Color", "Green"), ("Storage", "512GB") }, 42, result);

        return productIds;
    }

    private async Task<List<Guid>> CreateComputerComponentsAsync(SampleDataResult result)
    {
        var productIds = new List<Guid>();

        // NVIDIA RTX 4070
        var rtx4070 = new ProductMaster
        {
            Name = "NVIDIA RTX 4070 Graphics Card",
            Slug = "nvidia-rtx-4070",
            Description = "High-performance graphics card for gaming and content creation",
            Category = "Computer Hardware",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "NVIDIA", 
                memory = "12GB GDDR6X", 
                architecture = "Ada Lovelace",
                rayTracing = "Yes",
                dlss = "3.0",
                warranty = "3 years"
            })
        };
        var createdRtx = await _productService.CreateProductMasterAsync(rtx4070);
        productIds.Add(createdRtx.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdRtx.Id, "RTX4070-ASUS-12GB", 25900m, 
            new[] { ("Brand", "ASUS"), ("Memory", "12GB GDDR6X") }, 125, result);
        await CreateVariantWithStock(createdRtx.Id, "RTX4070-MSI-12GB", 24900m, 
            new[] { ("Brand", "MSI"), ("Memory", "12GB GDDR6X") }, 98, result);
        await CreateVariantWithStock(createdRtx.Id, "RTX4070-GIGABYTE-12GB", 22900m, 
            new[] { ("Brand", "Gigabyte"), ("Memory", "12GB GDDR6X") }, 156, result);

        // AMD Ryzen 7 7700X
        var ryzen = new ProductMaster
        {
            Name = "AMD Ryzen 7 7700X Processor",
            Slug = "amd-ryzen-7-7700x",
            Description = "8-core, 16-thread processor for high-performance computing",
            Category = "Computer Hardware",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "AMD", 
                cores = "8", 
                threads = "16",
                baseClock = "4.5 GHz",
                boostClock = "5.4 GHz",
                socket = "AM5",
                warranty = "3 years"
            })
        };
        var createdRyzen = await _productService.CreateProductMasterAsync(ryzen);
        productIds.Add(createdRyzen.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdRyzen.Id, "RYZEN7700X-BOX", 13900m, 
            new[] { ("Package", "Box"), ("Type", "Standard") }, 145, result);
        await CreateVariantWithStock(createdRyzen.Id, "RYZEN7700X-TRAY", 12900m, 
            new[] { ("Package", "Tray"), ("Type", "OEM") }, 89, result);

        // Corsair DDR5 RAM
        var corsairRam = new ProductMaster
        {
            Name = "Corsair Vengeance DDR5 RAM",
            Slug = "corsair-vengeance-ddr5",
            Description = "High-performance DDR5 memory for gaming and productivity",
            Category = "Computer Hardware",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Corsair", 
                type = "DDR5", 
                timing = "CL36",
                voltage = "1.35V",
                warranty = "Lifetime"
            })
        };
        var createdRam = await _productService.CreateProductMasterAsync(corsairRam);
        productIds.Add(createdRam.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdRam.Id, "CORSAIR-DDR5-16GB-5600-BLACK", 3900m, 
            new[] { ("Capacity", "16GB"), ("Speed", "5600MHz"), ("Color", "Black") }, 200, result);
        await CreateVariantWithStock(createdRam.Id, "CORSAIR-DDR5-32GB-6000-BLACK", 7900m, 
            new[] { ("Capacity", "32GB"), ("Speed", "6000MHz"), ("Color", "Black") }, 95, result);
        await CreateVariantWithStock(createdRam.Id, "CORSAIR-DDR5-32GB-6000-WHITE", 8900m, 
            new[] { ("Capacity", "32GB"), ("Speed", "6000MHz"), ("Color", "White") }, 0, result); // Out of stock

        // Samsung 980 PRO SSD
        var samsungSsd = new ProductMaster
        {
            Name = "Samsung 980 PRO NVMe SSD",
            Slug = "samsung-980-pro-nvme",
            Description = "High-speed NVMe SSD for ultimate performance",
            Category = "Computer Hardware",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Samsung", 
                interfaceType = "PCIe 4.0 x4", 
                formFactor = "M.2 2280",
                readSpeed = "7,000 MB/s",
                writeSpeed = "5,100 MB/s",
                warranty = "5 years"
            })
        };
        var createdSsd = await _productService.CreateProductMasterAsync(samsungSsd);
        productIds.Add(createdSsd.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdSsd.Id, "SAMSUNG980PRO-500GB", 2490m, 
            new[] { ("Capacity", "500GB"), ("Type", "Standard") }, 180, result);
        await CreateVariantWithStock(createdSsd.Id, "SAMSUNG980PRO-1TB-HEATSINK", 4990m, 
            new[] { ("Capacity", "1TB"), ("Type", "Heatsink") }, 125, result);
        await CreateVariantWithStock(createdSsd.Id, "SAMSUNG980PRO-2TB", 8990m, 
            new[] { ("Capacity", "2TB"), ("Type", "Standard") }, 67, result);

        // ASUS ROG Strix Motherboard
        var asusBoard = new ProductMaster
        {
            Name = "ASUS ROG Strix B650-E Motherboard",
            Slug = "asus-rog-strix-b650e",
            Description = "Premium AM5 motherboard for gaming and content creation",
            Category = "Computer Hardware",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "ASUS", 
                socket = "AM5", 
                chipset = "B650E",
                memorySlots = "4 x DDR5",
                pciSlots = "2 x PCIe 5.0 x16",
                warranty = "3 years"
            })
        };
        var createdBoard = await _productService.CreateProductMasterAsync(asusBoard);
        productIds.Add(createdBoard.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdBoard.Id, "ASUS-B650E-BLACK-GAMING", 8900m, 
            new[] { ("Color", "Black"), ("Edition", "Gaming") }, 78, result);
        await CreateVariantWithStock(createdBoard.Id, "ASUS-B650E-WHITE-CREATOR", 9900m, 
            new[] { ("Color", "White"), ("Edition", "Creator") }, 45, result);

        // Corsair Power Supply
        var corsairPsu = new ProductMaster
        {
            Name = "Corsair RM850x Power Supply",
            Slug = "corsair-rm850x-psu",
            Description = "80 PLUS Gold certified modular power supply",
            Category = "Computer Hardware",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Corsair", 
                efficiency = "80 PLUS Gold", 
                modular = "Fully Modular",
                fan = "135mm Magnetic Levitation",
                warranty = "10 years"
            })
        };
        var createdPsu = await _productService.CreateProductMasterAsync(corsairPsu);
        productIds.Add(createdPsu.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdPsu.Id, "CORSAIR-RM750X-BLACK", 4900m, 
            new[] { ("Wattage", "750W"), ("Color", "Black") }, 134, result);
        await CreateVariantWithStock(createdPsu.Id, "CORSAIR-RM850X-BLACK", 5900m, 
            new[] { ("Wattage", "850W"), ("Color", "Black") }, 98, result);
        await CreateVariantWithStock(createdPsu.Id, "CORSAIR-RM1000X-WHITE", 7900m, 
            new[] { ("Wattage", "1000W"), ("Color", "White") }, 56, result);

        return productIds;
    }

    private async Task<List<Guid>> CreateAccessoryProductsAsync(SampleDataResult result)
    {
        var productIds = new List<Guid>();

        // Logitech MX Master 3S
        var logitechMouse = new ProductMaster
        {
            Name = "Logitech MX Master 3S Mouse",
            Slug = "logitech-mx-master-3s",
            Description = "Advanced wireless mouse for productivity and precision",
            Category = "Accessory",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Logitech", 
                sensor = "Darkfield High Precision", 
                dpi = "8000",
                battery = "70 days",
                connectivity = "Bluetooth/USB Receiver",
                warranty = "1 year"
            })
        };
        var createdLogitechMouse = await _productService.CreateProductMasterAsync(logitechMouse);
        productIds.Add(createdLogitechMouse.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdLogitechMouse.Id, "MXMASTER3S-GRAPHITE-WIRELESS", 3590m, 
            new[] { ("Color", "Graphite"), ("Connection", "Wireless") }, 145, result);
        await CreateVariantWithStock(createdLogitechMouse.Id, "MXMASTER3S-GRAY-BLUETOOTH", 3290m, 
            new[] { ("Color", "Pale Gray"), ("Connection", "Bluetooth") }, 89, result);

        // Keychron K8 Keyboard
        var keychron = new ProductMaster
        {
            Name = "Keychron K8 Mechanical Keyboard",
            Slug = "keychron-k8-mechanical",
            Description = "Wireless mechanical keyboard for Mac and Windows",
            Category = "Accessory",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Keychron", 
                layout = "Tenkeyless", 
                connectivity = "Wireless/USB-C",
                battery = "4000mAh",
                hotSwappable = "Yes",
                warranty = "1 year"
            })
        };
        var createdKeychron = await _productService.CreateProductMasterAsync(keychron);
        productIds.Add(createdKeychron.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdKeychron.Id, "K8-GATERON-BLUE-RGB", 3890m, 
            new[] { ("Switch", "Gateron Blue"), ("Layout", "ANSI"), ("Backlight", "RGB") }, 156, result);
        await CreateVariantWithStock(createdKeychron.Id, "K8-GATERON-RED-WHITE", 3490m, 
            new[] { ("Switch", "Gateron Red"), ("Layout", "ISO"), ("Backlight", "White") }, 78, result);
        await CreateVariantWithStock(createdKeychron.Id, "K8-GATERON-BROWN-RGB", 3690m, 
            new[] { ("Switch", "Gateron Brown"), ("Layout", "ANSI"), ("Backlight", "RGB") }, 123, result);

        // Sony WH-1000XM5
        var sonyHeadphones = new ProductMaster
        {
            Name = "Sony WH-1000XM5 Headphones",
            Slug = "sony-wh-1000xm5",
            Description = "Industry-leading noise canceling wireless headphones",
            Category = "Accessory",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Sony", 
                noiseCanceling = "Industry Leading", 
                battery = "30 hours",
                driver = "30mm",
                codec = "LDAC, Hi-Res Audio",
                warranty = "1 year"
            })
        };
        var createdSony = await _productService.CreateProductMasterAsync(sonyHeadphones);
        productIds.Add(createdSony.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdSony.Id, "WH1000XM5-BLACK-STANDARD", 12990m, 
            new[] { ("Color", "Black"), ("Edition", "Standard") }, 12, result); // Low stock
        await CreateVariantWithStock(createdSony.Id, "WH1000XM5-SILVER-LIMITED", 13990m, 
            new[] { ("Color", "Silver"), ("Edition", "Limited Edition") }, 8, result); // Low stock

        // Razer DeathAdder V3
        var razerMouse = new ProductMaster
        {
            Name = "Razer DeathAdder V3 Gaming Mouse",
            Slug = "razer-deathadder-v3",
            Description = "Ergonomic gaming mouse with Focus Pro sensor",
            Category = "Accessory",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Razer", 
                sensor = "Focus Pro 30K", 
                dpi = "30000",
                switches = "Razer Optical Gen-3",
                weight = "59g",
                warranty = "2 years"
            })
        };
        var createdRazer = await _productService.CreateProductMasterAsync(razerMouse);
        productIds.Add(createdRazer.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdRazer.Id, "DEATHADDER-V3-BLACK-WIRED", 2290m, 
            new[] { ("Color", "Black"), ("Connection", "Wired") }, 189, result);
        await CreateVariantWithStock(createdRazer.Id, "DEATHADDER-V3-WHITE-WIRELESS", 3290m, 
            new[] { ("Color", "White"), ("Connection", "Wireless") }, 134, result);

        // HyperX Cloud III
        var hyperxHeadset = new ProductMaster
        {
            Name = "HyperX Cloud III Gaming Headset",
            Slug = "hyperx-cloud-iii",
            Description = "Comfortable gaming headset with crystal clear audio",
            Category = "Accessory",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "HyperX", 
                driver = "53mm Dynamic", 
                microphone = "Detachable",
                weight = "308g",
                compatibility = "Multi-platform",
                warranty = "2 years"
            })
        };
        var createdHyperx = await _productService.CreateProductMasterAsync(hyperxHeadset);
        productIds.Add(createdHyperx.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdHyperx.Id, "CLOUDIII-BLACK-3.5MM", 2990m, 
            new[] { ("Color", "Black"), ("Connection", "3.5mm") }, 167, result);
        await CreateVariantWithStock(createdHyperx.Id, "CLOUDIII-RED-USBC", 3490m, 
            new[] { ("Color", "Red"), ("Connection", "USB-C") }, 98, result);
        await CreateVariantWithStock(createdHyperx.Id, "CLOUDIII-WHITE-WIRELESS", 4290m, 
            new[] { ("Color", "White"), ("Connection", "Wireless") }, 76, result);

        // Anker PowerCore
        var ankerPowerbank = new ProductMaster
        {
            Name = "Anker PowerCore 20000mAh Power Bank",
            Slug = "anker-powercore-20000",
            Description = "High-capacity portable charger with fast charging",
            Category = "Accessory",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Anker", 
                technology = "PowerIQ 3.0", 
                input = "USB-C PD",
                output = "22.5W",
                safety = "MultiProtect",
                warranty = "18 months"
            })
        };
        var createdAnker = await _productService.CreateProductMasterAsync(ankerPowerbank);
        productIds.Add(createdAnker.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdAnker.Id, "POWERCORE-10000-USBA", 1290m, 
            new[] { ("Capacity", "10000mAh"), ("Port", "USB-A") }, 245, result);
        await CreateVariantWithStock(createdAnker.Id, "POWERCORE-20000-USBC", 1990m, 
            new[] { ("Capacity", "20000mAh"), ("Port", "USB-C") }, 178, result);
        await CreateVariantWithStock(createdAnker.Id, "POWERCORE-20000-LIGHTNING", 2290m, 
            new[] { ("Capacity", "20000mAh"), ("Port", "Lightning") }, 134, result);

        // Belkin Wireless Charger
        var belkinCharger = new ProductMaster
        {
            Name = "Belkin 3-in-1 Wireless Charger",
            Slug = "belkin-3in1-wireless-charger",
            Description = "Charge iPhone, AirPods, and Apple Watch simultaneously",
            Category = "Accessory",
            Attributes = JsonSerializer.SerializeToDocument(new { 
                brand = "Belkin", 
                compatibility = "iPhone, AirPods, Apple Watch", 
                charging = "Qi Wireless",
                design = "Foldable Stand",
                certification = "MFi Certified",
                warranty = "2 years"
            })
        };
        var createdBelkin = await _productService.CreateProductMasterAsync(belkinCharger);
        productIds.Add(createdBelkin.Id);
        result.ProductsCreated++;

        await CreateVariantWithStock(createdBelkin.Id, "BELKIN-3IN1-BLACK-7.5W", 2990m, 
            new[] { ("Color", "Black"), ("Type", "Standard"), ("Power", "7.5W") }, 89, result);
        await CreateVariantWithStock(createdBelkin.Id, "BELKIN-3IN1-WHITE-MAGSAFE-15W", 4290m, 
            new[] { ("Color", "White"), ("Type", "MagSafe"), ("Power", "15W") }, 67, result);

        return productIds;
    }
    
    private async Task CreateVariantWithStock(Guid productId, string sku, decimal price, 
        (string optionName, string value)[] options, int stockQuantity, SampleDataResult result)
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
                var existingOption = await _context.VariantOptions
                    .FirstOrDefaultAsync(vo => vo.ProductMasterId == productId && vo.Name == optionName);
                
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
                    
                    _context.VariantOptions.Add(newOption);
                    await _context.SaveChangesAsync();
                    variantOptionId = newOption.Id;
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

            // Create inventory record with specified stock
            var warehouse = await _context.Warehouses.FirstAsync(w => w.Code == "MAIN");
                
            await _inventoryService.CreateInventoryRecordAsync(
                sellableItem.Id,
                warehouse.Id,
                onHand: stockQuantity,
                reserved: 0
            );

            _logger.LogInformation("Created variant: {SKU} with stock: {Stock}", sku, stockQuantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating variant {SKU}: {Error}", sku, ex.Message);
            throw;
        }
    }

    private async Task CreateProductBundlesAsync(List<Guid> mobileProducts, List<Guid> computerProducts, 
        List<Guid> accessoryProducts, SampleDataResult result)
    {
        try
        {
            var warehouse = await _context.Warehouses.FirstAsync(w => w.Code == "MAIN");

            // Gaming Setup Pro Bundle
            var gamingBundle = new ProductBundle
            {
                Name = "Gaming Setup Pro Bundle",
                Description = "Complete gaming setup with high-end components",
                Price = 52900m,
                Status = EntityStatus.Active
            };

            _context.ProductBundles.Add(gamingBundle);
            await _context.SaveChangesAsync();

            // Add bundle items - RTX 4070, Ryzen 7700X, DDR5 32GB, Keychron K8, Razer Mouse
            var rtx4070Variant = await _context.SellableItems.FirstAsync(s => s.SKU == "RTX4070-GIGABYTE-12GB");
            var ryzenVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "RYZEN7700X-BOX");
            var ramVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "CORSAIR-DDR5-32GB-6000-BLACK");
            var keyboardVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "K8-GATERON-BLUE-RGB");
            var mouseVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "DEATHADDER-V3-BLACK-WIRED");

            var gamingBundleItems = new[]
            {
                new BundleItem { BundleId = gamingBundle.Id, SellableItemId = rtx4070Variant.Id, Quantity = 1 },
                new BundleItem { BundleId = gamingBundle.Id, SellableItemId = ryzenVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = gamingBundle.Id, SellableItemId = ramVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = gamingBundle.Id, SellableItemId = keyboardVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = gamingBundle.Id, SellableItemId = mouseVariant.Id, Quantity = 1 }
            };

            _context.BundleItems.AddRange(gamingBundleItems);

            // Create bundle sellable item
            var gamingBundleSellable = new SellableItem
            {
                SKU = "BUNDLE-GAMING-PRO",
                Type = SellableItemType.Bundle,
                BundleId = gamingBundle.Id,
                Status = EntityStatus.Active
            };
            _context.SellableItems.Add(gamingBundleSellable);
            await _context.SaveChangesAsync();

            await _inventoryService.CreateInventoryRecordAsync(gamingBundleSellable.Id, warehouse.Id, 25, 0);

            // Mobile Power User Bundle
            var mobileBundle = new ProductBundle
            {
                Name = "Mobile Power User Bundle",
                Description = "iPhone with premium accessories for power users",
                Price = 42900m,
                Status = EntityStatus.Active
            };

            _context.ProductBundles.Add(mobileBundle);
            await _context.SaveChangesAsync();

            var iphoneVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "IPHONE15PRO-BLUE-256");
            var powerbankVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "POWERCORE-20000-USBC");
            var chargerVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "BELKIN-3IN1-WHITE-MAGSAFE-15W");

            var mobileBundleItems = new[]
            {
                new BundleItem { BundleId = mobileBundle.Id, SellableItemId = iphoneVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = mobileBundle.Id, SellableItemId = powerbankVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = mobileBundle.Id, SellableItemId = chargerVariant.Id, Quantity = 1 }
            };

            _context.BundleItems.AddRange(mobileBundleItems);

            var mobileBundleSellable = new SellableItem
            {
                SKU = "BUNDLE-MOBILE-POWER",
                Type = SellableItemType.Bundle,
                BundleId = mobileBundle.Id,
                Status = EntityStatus.Active
            };
            _context.SellableItems.Add(mobileBundleSellable);
            await _context.SaveChangesAsync();

            await _inventoryService.CreateInventoryRecordAsync(mobileBundleSellable.Id, warehouse.Id, 15, 0);

            // Content Creator Bundle
            var creatorBundle = new ProductBundle
            {
                Name = "Content Creator Bundle",
                Description = "Samsung flagship with premium audio and productivity tools",
                Price = 58900m,
                Status = EntityStatus.Active
            };

            _context.ProductBundles.Add(creatorBundle);
            await _context.SaveChangesAsync();

            var samsungVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "S24ULTRA-GRAY-512");
            var sonyHeadphonesVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "WH1000XM5-BLACK-STANDARD");
            var logitechMouseVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "MXMASTER3S-GRAPHITE-WIRELESS");

            var creatorBundleItems = new[]
            {
                new BundleItem { BundleId = creatorBundle.Id, SellableItemId = samsungVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = creatorBundle.Id, SellableItemId = sonyHeadphonesVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = creatorBundle.Id, SellableItemId = logitechMouseVariant.Id, Quantity = 1 }
            };

            _context.BundleItems.AddRange(creatorBundleItems);

            var creatorBundleSellable = new SellableItem
            {
                SKU = "BUNDLE-CREATOR",
                Type = SellableItemType.Bundle,
                BundleId = creatorBundle.Id,
                Status = EntityStatus.Active
            };
            _context.SellableItems.Add(creatorBundleSellable);
            await _context.SaveChangesAsync();

            await _inventoryService.CreateInventoryRecordAsync(creatorBundleSellable.Id, warehouse.Id, 8, 0);

            // PC Builder Starter Bundle
            var pcBuilderBundle = new ProductBundle
            {
                Name = "PC Builder Starter Bundle",
                Description = "Essential components to start your PC build",
                Price = 19900m,
                Status = EntityStatus.Active
            };

            _context.ProductBundles.Add(pcBuilderBundle);
            await _context.SaveChangesAsync();

            var motherboardVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "ASUS-B650E-BLACK-GAMING");
            var ram16Variant = await _context.SellableItems.FirstAsync(s => s.SKU == "CORSAIR-DDR5-16GB-5600-BLACK");
            var ssdVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "SAMSUNG980PRO-1TB-HEATSINK");
            var psuVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "CORSAIR-RM850X-BLACK");

            var pcBuilderBundleItems = new[]
            {
                new BundleItem { BundleId = pcBuilderBundle.Id, SellableItemId = motherboardVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = pcBuilderBundle.Id, SellableItemId = ram16Variant.Id, Quantity = 1 },
                new BundleItem { BundleId = pcBuilderBundle.Id, SellableItemId = ssdVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = pcBuilderBundle.Id, SellableItemId = psuVariant.Id, Quantity = 1 }
            };

            _context.BundleItems.AddRange(pcBuilderBundleItems);

            var pcBuilderBundleSellable = new SellableItem
            {
                SKU = "BUNDLE-PC-STARTER",
                Type = SellableItemType.Bundle,
                BundleId = pcBuilderBundle.Id,
                Status = EntityStatus.Active
            };
            _context.SellableItems.Add(pcBuilderBundleSellable);
            await _context.SaveChangesAsync();

            await _inventoryService.CreateInventoryRecordAsync(pcBuilderBundleSellable.Id, warehouse.Id, 20, 0);

            // Work From Home Bundle
            var wfhBundle = new ProductBundle
            {
                Name = "Work From Home Bundle",
                Description = "Complete setup for productive remote work",
                Price = 33900m,
                Status = EntityStatus.Active
            };

            _context.ProductBundles.Add(wfhBundle);
            await _context.SaveChangesAsync();

            var pixelVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "PIXEL8-OBSIDIAN-128");
            var keychron2Variant = await _context.SellableItems.FirstAsync(s => s.SKU == "K8-GATERON-RED-WHITE");
            var logitech2Variant = await _context.SellableItems.FirstAsync(s => s.SKU == "MXMASTER3S-GRAY-BLUETOOTH");
            var hyperxVariant = await _context.SellableItems.FirstAsync(s => s.SKU == "CLOUDIII-BLACK-3.5MM");

            var wfhBundleItems = new[]
            {
                new BundleItem { BundleId = wfhBundle.Id, SellableItemId = pixelVariant.Id, Quantity = 1 },
                new BundleItem { BundleId = wfhBundle.Id, SellableItemId = keychron2Variant.Id, Quantity = 1 },
                new BundleItem { BundleId = wfhBundle.Id, SellableItemId = logitech2Variant.Id, Quantity = 1 },
                new BundleItem { BundleId = wfhBundle.Id, SellableItemId = hyperxVariant.Id, Quantity = 1 }
            };

            _context.BundleItems.AddRange(wfhBundleItems);

            var wfhBundleSellable = new SellableItem
            {
                SKU = "BUNDLE-WFH",
                Type = SellableItemType.Bundle,
                BundleId = wfhBundle.Id,
                Status = EntityStatus.Active
            };
            _context.SellableItems.Add(wfhBundleSellable);
            await _context.SaveChangesAsync();

            await _inventoryService.CreateInventoryRecordAsync(wfhBundleSellable.Id, warehouse.Id, 12, 0);

            result.BundlesCreated = 5;
            _logger.LogInformation("Created 5 product bundles successfully");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product bundles");
            throw;
        }
    }
}

public class SampleDataResult
{
    public int WarehousesCreated { get; set; } = 0;
    public int ProductsCreated { get; set; } = 0;
    public int VariantsCreated { get; set; } = 0;
    public int BundlesCreated { get; set; } = 0;
}