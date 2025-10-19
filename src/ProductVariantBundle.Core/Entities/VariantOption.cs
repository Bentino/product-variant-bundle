namespace ProductVariantBundle.Core.Entities;

public class VariantOption : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid ProductMasterId { get; set; }
    public ProductMaster ProductMaster { get; set; } = null!;
    public ICollection<VariantOptionValue> Values { get; set; } = new List<VariantOptionValue>();
}