using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Validators;

namespace ProductVariantBundle.Core.Services;

public class SellableItemService : ISellableItemService
{
    private readonly ISellableItemRepository _sellableItemRepository;
    private readonly SkuValidator _skuValidator;

    public SellableItemService(ISellableItemRepository sellableItemRepository, SkuValidator skuValidator)
    {
        _sellableItemRepository = sellableItemRepository;
        _skuValidator = skuValidator;
    }

    public async Task<bool> ValidateSkuUniquenessAsync(string sku, Guid? excludeId = null)
    {
        return !await _sellableItemRepository.SKUExistsAsync(sku, excludeId);
    }

    public async Task<SellableItem?> GetBySKUAsync(string sku)
    {
        return await _sellableItemRepository.GetBySKUAsync(sku);
    }

    public async Task<SellableItem?> GetByVariantIdAsync(Guid variantId)
    {
        return await _sellableItemRepository.GetByVariantIdAsync(variantId);
    }

    public async Task<SellableItem?> GetByBundleIdAsync(Guid bundleId)
    {
        return await _sellableItemRepository.GetByBundleIdAsync(bundleId);
    }

    public async Task<SellableItem> CreateSellableItemAsync(SellableItemType type, Guid entityId, string sku)
    {
        // Normalize and validate SKU
        sku = SkuValidator.NormalizeSku(sku);
        await _skuValidator.ValidateSkuAsync(sku);

        var sellableItem = new SellableItem
        {
            Id = Guid.NewGuid(),
            SKU = sku,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = EntityStatus.Active
        };

        // Set the appropriate foreign key based on type
        switch (type)
        {
            case SellableItemType.Variant:
                sellableItem.VariantId = entityId;
                break;
            case SellableItemType.Bundle:
                sellableItem.BundleId = entityId;
                break;
            default:
                throw new ValidationException("Type", $"Unsupported sellable item type: {type}");
        }

        return await _sellableItemRepository.AddAsync(sellableItem);
    }

    public async Task<SellableItem> UpdateSellableItemAsync(SellableItem sellableItem)
    {
        var existing = await _sellableItemRepository.GetByIdAsync(sellableItem.Id);
        if (existing == null)
        {
            throw new EntityNotFoundException("SellableItem", sellableItem.Id);
        }

        // Normalize and validate SKU if changed
        if (sellableItem.SKU != existing.SKU)
        {
            sellableItem.SKU = SkuValidator.NormalizeSku(sellableItem.SKU);
            await _skuValidator.ValidateSkuAsync(sellableItem.SKU, sellableItem.Id);
        }

        // Update audit fields
        sellableItem.UpdatedAt = DateTime.UtcNow;

        await _sellableItemRepository.UpdateAsync(sellableItem);
        return sellableItem;
    }

    public async Task DeleteSellableItemAsync(Guid id)
    {
        var existing = await _sellableItemRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new EntityNotFoundException("SellableItem", id);
        }

        await _sellableItemRepository.DeleteAsync(id);
    }
}