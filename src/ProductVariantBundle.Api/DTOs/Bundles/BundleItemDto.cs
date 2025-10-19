using System.ComponentModel.DataAnnotations;
using ProductVariantBundle.Api.DTOs.Common;

namespace ProductVariantBundle.Api.DTOs.Bundles;

public class BundleItemDto : BaseDto
{
    public Guid SellableItemId { get; set; }
    public int Quantity { get; set; }
    public Guid BundleId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
}

public class CreateBundleItemDto
{
    [Required(ErrorMessage = "Sellable Item ID is required")]
    public Guid SellableItemId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}