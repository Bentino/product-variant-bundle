using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using System.Data;

namespace ProductVariantBundle.Core.Services;

public class BundleAvailabilityCalculator
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IWarehouseService _warehouseService;

    public BundleAvailabilityCalculator(
        IInventoryRepository inventoryRepository,
        IWarehouseService warehouseService)
    {
        _inventoryRepository = inventoryRepository;
        _warehouseService = warehouseService;
    }

    /// <summary>
    /// Calculate bundle availability based on component stock levels
    /// Formula: min(floor((on_hand - reserved) / required_quantity)) for each component
    /// </summary>
    public async Task<BundleAvailability> CalculateAvailabilityAsync(ProductBundle bundle, string warehouseCode = "MAIN")
    {
        if (bundle?.Items == null || !bundle.Items.Any())
        {
            return new BundleAvailability
            {
                BundleId = bundle?.Id ?? Guid.Empty,
                AvailableQuantity = 0,
                IsAvailable = false,
                WarehouseCode = warehouseCode,
                Components = new List<ComponentAvailability>()
            };
        }

        // Get warehouse
        var warehouse = await _warehouseService.GetByCodeAsync(warehouseCode);
        if (warehouse == null)
        {
            throw new EntityNotFoundException("Warehouse", warehouseCode);
        }

        var availability = int.MaxValue;
        var componentAvailabilities = new List<ComponentAvailability>();

        foreach (var item in bundle.Items)
        {
            // Get inventory record for this component
            var inventoryRecord = await _inventoryRepository.GetBySellableItemAndWarehouseAsync(item.SellableItemId, warehouse.Id);

            if (inventoryRecord == null)
            {
                // Component not found or no stock record
                availability = 0;
                componentAvailabilities.Add(new ComponentAvailability
                {
                    SellableItemId = item.SellableItemId,
                    SKU = item.SellableItem?.SKU ?? string.Empty,
                    Required = item.Quantity,
                    Available = 0,
                    CanFulfill = 0
                });
                break;
            }

            var available = inventoryRecord.OnHand - inventoryRecord.Reserved;
            var componentAvailability = available >= item.Quantity ? 
                Math.Floor((double)available / item.Quantity) : 0;

            componentAvailabilities.Add(new ComponentAvailability
            {
                SellableItemId = item.SellableItemId,
                SKU = inventoryRecord.SellableItem?.SKU ?? string.Empty,
                Required = item.Quantity,
                Available = available,
                CanFulfill = (int)componentAvailability
            });

            availability = Math.Min(availability, (int)componentAvailability);
        }

        return new BundleAvailability
        {
            BundleId = bundle.Id,
            AvailableQuantity = availability == int.MaxValue ? 0 : availability,
            IsAvailable = availability > 0,
            WarehouseCode = warehouseCode,
            Components = componentAvailabilities
        };
    }

    /// <summary>
    /// Calculate bundle availability with transaction-safe stock checking using row locking
    /// This method should be used when preparing for actual reservations
    /// </summary>
    public async Task<BundleAvailability> CalculateAvailabilityWithLockAsync(ProductBundle bundle, string warehouseCode = "MAIN")
    {
        if (bundle?.Items == null || !bundle.Items.Any())
        {
            return new BundleAvailability
            {
                BundleId = bundle?.Id ?? Guid.Empty,
                AvailableQuantity = 0,
                IsAvailable = false,
                WarehouseCode = warehouseCode,
                Components = new List<ComponentAvailability>()
            };
        }

        var availability = int.MaxValue;
        var componentAvailabilities = new List<ComponentAvailability>();

        foreach (var item in bundle.Items)
        {
            // Use row locking to get consistent stock levels for reservation
            var inventoryRecord = await _inventoryRepository.GetBySKUAndWarehouseCodeWithLockAsync(
                item.SellableItem?.SKU ?? string.Empty, warehouseCode);

            if (inventoryRecord == null)
            {
                // Component not found or no stock record
                availability = 0;
                componentAvailabilities.Add(new ComponentAvailability
                {
                    SellableItemId = item.SellableItemId,
                    SKU = item.SellableItem?.SKU ?? string.Empty,
                    Required = item.Quantity,
                    Available = 0,
                    CanFulfill = 0
                });
                break;
            }

            var available = inventoryRecord.OnHand - inventoryRecord.Reserved;
            var componentAvailability = available >= item.Quantity ? 
                Math.Floor((double)available / item.Quantity) : 0;

            componentAvailabilities.Add(new ComponentAvailability
            {
                SellableItemId = item.SellableItemId,
                SKU = inventoryRecord.SellableItem?.SKU ?? string.Empty,
                Required = item.Quantity,
                Available = available,
                CanFulfill = (int)componentAvailability
            });

            availability = Math.Min(availability, (int)componentAvailability);
        }

        return new BundleAvailability
        {
            BundleId = bundle.Id,
            AvailableQuantity = availability == int.MaxValue ? 0 : availability,
            IsAvailable = availability > 0,
            WarehouseCode = warehouseCode,
            Components = componentAvailabilities
        };
    }

    /// <summary>
    /// Reserve stock for bundle components with transaction safety and row locking
    /// </summary>
    public async Task<bool> ReserveStockAsync(ProductBundle bundle, int quantity, string warehouseCode = "MAIN")
    {
        if (quantity <= 0)
        {
            throw new ValidationException("Quantity", "Quantity must be positive");
        }

        // Get warehouse
        var warehouse = await _warehouseService.GetByCodeAsync(warehouseCode);
        if (warehouse == null)
        {
            throw new EntityNotFoundException("Warehouse", warehouseCode);
        }

        // Use transaction-safe availability check with row locking
        var availability = await CalculateAvailabilityWithLockAsync(bundle, warehouseCode);
        if (availability.AvailableQuantity < quantity)
        {
            return false;
        }

        // Reserve stock for each component within the same transaction context
        foreach (var item in bundle.Items)
        {
            var requiredQuantity = item.Quantity * quantity;
            
            // Get current inventory record with lock (already locked from availability check)
            var inventoryRecord = await _inventoryRepository.GetBySellableItemAndWarehouseAsync(item.SellableItemId, warehouse.Id);
            if (inventoryRecord != null)
            {
                // Verify we still have enough stock (double-check for safety)
                var currentAvailable = inventoryRecord.OnHand - inventoryRecord.Reserved;
                if (currentAvailable < requiredQuantity)
                {
                    throw new ValidationException("Stock", $"Insufficient stock for item {inventoryRecord.SellableItem?.SKU}. Available: {currentAvailable}, Required: {requiredQuantity}");
                }

                // Update reserved quantity
                await _inventoryRepository.UpdateStockAsync(
                    item.SellableItemId, 
                    warehouse.Id, 
                    inventoryRecord.OnHand, 
                    inventoryRecord.Reserved + requiredQuantity);
            }
            else
            {
                throw new EntityNotFoundException("InventoryRecord", $"SellableItem: {item.SellableItemId}, Warehouse: {warehouse.Code}");
            }
        }

        return true;
    }

    /// <summary>
    /// Release reserved stock for bundle components
    /// </summary>
    public async Task<bool> ReleaseReservedStockAsync(ProductBundle bundle, int quantity, string warehouseCode = "MAIN")
    {
        if (quantity <= 0)
        {
            throw new ValidationException("Quantity", "Quantity must be positive");
        }

        // Get warehouse
        var warehouse = await _warehouseService.GetByCodeAsync(warehouseCode);
        if (warehouse == null)
        {
            throw new EntityNotFoundException("Warehouse", warehouseCode);
        }

        // Release reserved stock for each component
        foreach (var item in bundle.Items)
        {
            var releaseQuantity = item.Quantity * quantity;
            
            // Get current inventory record with lock for consistency
            var inventoryRecord = await _inventoryRepository.GetBySKUAndWarehouseCodeWithLockAsync(
                item.SellableItem?.SKU ?? string.Empty, warehouseCode);
                
            if (inventoryRecord != null)
            {
                // Verify we have enough reserved stock to release
                if (inventoryRecord.Reserved < releaseQuantity)
                {
                    throw new ValidationException("Reserved", $"Cannot release more than reserved for item {inventoryRecord.SellableItem?.SKU}. Reserved: {inventoryRecord.Reserved}, Requested: {releaseQuantity}");
                }

                // Update reserved quantity
                await _inventoryRepository.UpdateStockAsync(
                    item.SellableItemId, 
                    warehouse.Id, 
                    inventoryRecord.OnHand, 
                    inventoryRecord.Reserved - releaseQuantity);
            }
            else
            {
                throw new EntityNotFoundException("InventoryRecord", $"SellableItem: {item.SellableItemId}, Warehouse: {warehouse.Code}");
            }
        }

        return true;
    }
}