using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;

namespace ProductVariantBundle.Core.Validators;

public class BundleValidator
{
    private readonly ISellableItemRepository _sellableItemRepository;

    public BundleValidator(ISellableItemRepository sellableItemRepository)
    {
        _sellableItemRepository = sellableItemRepository;
    }

    public async Task ValidateProductBundleAsync(ProductBundle bundle, bool isUpdate = false)
    {
        var errors = new Dictionary<string, List<string>>();

        // Validate required fields
        if (string.IsNullOrWhiteSpace(bundle.Name))
        {
            errors.AddError("Name", "Name is required");
        }

        if (bundle.Price < 0)
        {
            errors.AddError("Price", "Price cannot be negative");
        }

        // Validate bundle items
        await ValidateBundleItemsAsync(bundle.Items, errors);

        if (errors.Any())
        {
            throw new ValidationException(errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
        }
    }

    public async Task ValidateBundleItemsAsync(IEnumerable<BundleItem> items, Dictionary<string, List<string>>? errors = null)
    {
        errors ??= new Dictionary<string, List<string>>();

        if (!items.Any())
        {
            errors.AddError("Items", "Bundle must contain at least one item");
            return;
        }

        var sellableItemIds = new HashSet<Guid>();
        var index = 0;

        foreach (var item in items)
        {
            var itemKey = $"Items[{index}]";

            // Validate quantity is positive
            if (item.Quantity <= 0)
            {
                errors.AddError($"{itemKey}.Quantity", "Quantity must be positive");
            }

            // Check for duplicate sellable items in the same bundle
            if (sellableItemIds.Contains(item.SellableItemId))
            {
                errors.AddError($"{itemKey}.SellableItemId", "Duplicate sellable item in bundle");
            }
            else
            {
                sellableItemIds.Add(item.SellableItemId);
            }

            // Validate sellable item exists and is active
            var sellableItem = await _sellableItemRepository.GetByIdAsync(item.SellableItemId);
            if (sellableItem == null)
            {
                errors.AddError($"{itemKey}.SellableItemId", "Sellable item not found");
            }
            else if (sellableItem.Status != EntityStatus.Active)
            {
                errors.AddError($"{itemKey}.SellableItemId", "Sellable item must be active");
            }

            index++;
        }

        if (errors.Any())
        {
            throw new ValidationException(errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
        }
    }
}