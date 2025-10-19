using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Core.Validators;

namespace ProductVariantBundle.Core.Services;

public class BundleService : IBundleService
{
    private readonly IBundleRepository _bundleRepository;
    private readonly ISellableItemService _sellableItemService;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IWarehouseService _warehouseService;
    private readonly BundleValidator _validator;
    private readonly BundleAvailabilityCalculator _availabilityCalculator;
    private readonly BatchOperationService _batchOperationService;

    public BundleService(
        IBundleRepository bundleRepository, 
        ISellableItemService sellableItemService,
        IInventoryRepository inventoryRepository,
        IWarehouseService warehouseService,
        BundleValidator validator,
        BundleAvailabilityCalculator availabilityCalculator,
        BatchOperationService batchOperationService)
    {
        _bundleRepository = bundleRepository;
        _sellableItemService = sellableItemService;
        _inventoryRepository = inventoryRepository;
        _warehouseService = warehouseService;
        _validator = validator;
        _availabilityCalculator = availabilityCalculator;
        _batchOperationService = batchOperationService;
    }

    public async Task<ProductBundle> CreateBundleAsync(ProductBundle bundle)
    {
        // Validate using validator
        await _validator.ValidateProductBundleAsync(bundle, false);

        // Pre-validate SKU if provided to fail fast before creating bundle
        if (bundle.SellableItem != null && !string.IsNullOrEmpty(bundle.SellableItem.SKU))
        {
            var normalizedSku = SkuValidator.NormalizeSku(bundle.SellableItem.SKU);
            var isUnique = await _sellableItemService.ValidateSkuUniquenessAsync(normalizedSku);
            if (!isUnique)
            {
                throw new ValidationException("SKU", $"SKU '{normalizedSku}' already exists");
            }
        }

        // Set audit fields
        bundle.Id = Guid.NewGuid();
        bundle.CreatedAt = DateTime.UtcNow;
        bundle.UpdatedAt = DateTime.UtcNow;
        bundle.Status = EntityStatus.Active;

        var createdBundle = await _bundleRepository.AddAsync(bundle);

        // Create sellable item if SKU is provided
        if (bundle.SellableItem != null && !string.IsNullOrEmpty(bundle.SellableItem.SKU))
        {
            var sellableItem = await _sellableItemService.CreateSellableItemAsync(
                SellableItemType.Bundle, 
                createdBundle.Id, 
                bundle.SellableItem.SKU,
                skipValidation: true);
            
            createdBundle.SellableItem = sellableItem;
        }

        return createdBundle;
    }

    public async Task<ProductBundle?> GetBundleAsync(Guid id)
    {
        return await _bundleRepository.GetByIdAsync(id);
    }

    public async Task<ProductBundle?> GetBundleWithItemsAsync(Guid id)
    {
        return await _bundleRepository.GetByIdWithItemsAsync(id);
    }

    public async Task<IEnumerable<ProductBundle>> GetBundlesAsync()
    {
        return await _bundleRepository.GetAllAsync();
    }

    public async Task<PagedResult<ProductBundle>> GetBundlesAsync(BundleFilter filter)
    {
        return await _bundleRepository.GetAllAsync(filter);
    }

    public async Task<ProductBundle> UpdateBundleAsync(ProductBundle bundle)
    {
        var existing = await _bundleRepository.GetByIdAsync(bundle.Id);
        if (existing == null)
        {
            throw new EntityNotFoundException("ProductBundle", bundle.Id);
        }

        // Validate using validator
        await _validator.ValidateProductBundleAsync(bundle, true);

        // Update audit fields
        bundle.UpdatedAt = DateTime.UtcNow;

        await _bundleRepository.UpdateAsync(bundle);
        return bundle;
    }

    public async Task DeleteBundleAsync(Guid id)
    {
        var existing = await _bundleRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new EntityNotFoundException("ProductBundle", id);
        }

        await _bundleRepository.DeleteAsync(id);
    }

    public async Task<BundleAvailability> CalculateAvailabilityAsync(Guid bundleId, string warehouseCode = "MAIN")
    {
        var bundle = await _bundleRepository.GetByIdWithItemsAsync(bundleId);
        if (bundle == null)
        {
            throw new EntityNotFoundException("ProductBundle", bundleId);
        }

        return await _availabilityCalculator.CalculateAvailabilityAsync(bundle, warehouseCode);
    }

    public async Task<BundleAvailability> CalculateAvailabilityWithLockAsync(Guid bundleId, string warehouseCode = "MAIN")
    {
        var bundle = await _bundleRepository.GetByIdWithItemsAsync(bundleId);
        if (bundle == null)
        {
            throw new EntityNotFoundException("ProductBundle", bundleId);
        }

        return await _availabilityCalculator.CalculateAvailabilityWithLockAsync(bundle, warehouseCode);
    }

    public async Task ValidateBundleItemsAsync(IEnumerable<BundleItem> items)
    {
        await _validator.ValidateBundleItemsAsync(items);
    }


    public async Task<BatchOperationResult<ProductBundle>> CreateBundlesBatchAsync(BatchCreateBundleRequest request)
    {
        // Check for existing operation with same idempotency key
        var existingResult = await _batchOperationService.GetExistingOperationAsync<ProductBundle>(request.IdempotencyKey);
        if (existingResult != null)
        {
            return existingResult;
        }

        var results = new List<BatchItemResult<ProductBundle>>();
        var index = 0;

        foreach (var item in request.Items)
        {
            var itemResult = new BatchItemResult<ProductBundle> { Index = index };
            
            try
            {
                // Check if bundle with same SKU already exists
                var existingBundle = await _bundleRepository.GetBySKUAsync(item.SKU);

                if (existingBundle != null)
                {
                    switch (request.OnConflict)
                    {
                        case ConflictResolutionStrategy.Fail:
                            itemResult.Success = false;
                            itemResult.Errors = new[] { $"Bundle with SKU '{item.SKU}' already exists" };
                            break;
                        case ConflictResolutionStrategy.Skip:
                            itemResult.Success = true;
                            itemResult.Data = null; // Skipped
                            break;
                        case ConflictResolutionStrategy.Update:
                            // Update existing bundle
                            existingBundle.Name = item.Name;
                            existingBundle.Description = item.Description;
                            existingBundle.Price = item.Price;
                            existingBundle.Items = item.Items.Select(bi => new BundleItem
                            {
                                SellableItemId = bi.SellableItemId,
                                Quantity = bi.Quantity
                            }).ToList();

                            var updatedBundle = await UpdateBundleAsync(existingBundle);
                            itemResult.Success = true;
                            itemResult.Data = updatedBundle;
                            break;
                    }
                }
                else
                {
                    // Create new bundle
                    var bundle = new ProductBundle
                    {
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        SellableItem = new SellableItem { SKU = item.SKU },
                        Items = item.Items.Select(bi => new BundleItem
                        {
                            SellableItemId = bi.SellableItemId,
                            Quantity = bi.Quantity
                        }).ToList()
                    };

                    var createdBundle = await CreateBundleAsync(bundle);
                    itemResult.Success = true;
                    itemResult.Data = createdBundle;
                }
            }
            catch (ValidationException ex)
            {
                itemResult.Success = false;
                itemResult.Errors = ex.Errors.SelectMany(kvp => kvp.Value).ToArray();
            }
            catch (Exception ex)
            {
                itemResult.Success = false;
                itemResult.Errors = new[] { ex.Message };
            }

            results.Add(itemResult);
            index++;
        }

        var batchResult = BatchOperationService.CreateResult(request.IdempotencyKey, request.OnConflict, results);
        
        // Save the operation result for idempotency
        await _batchOperationService.SaveOperationResultAsync(request.IdempotencyKey, "CreateBundles", batchResult);
        
        return batchResult;
    }
}