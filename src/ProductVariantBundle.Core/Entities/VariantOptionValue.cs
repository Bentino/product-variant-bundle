namespace ProductVariantBundle.Core.Entities;

public class VariantOptionValue : BaseEntity
{
    public string Value { get; set; } = string.Empty;
    public Guid VariantOptionId { get; set; }
    public VariantOption VariantOption { get; set; } = null!;
    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
}