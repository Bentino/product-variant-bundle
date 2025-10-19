using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Api.DTOs.Common;

public class BaseFilterDto : PaginationDto
{
    public EntityStatus? Status { get; set; }
    public string? Search { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}

public class ProductFilterDto : BaseFilterDto
{
    public string? Category { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
}

public class BundleFilterDto : BaseFilterDto
{
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}