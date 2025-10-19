using ProductVariantBundle.Core.Enums;

namespace ProductVariantBundle.Core.Models;

public class ProductFilter : BaseFilter
{
    public string? Category { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }

    // Override to add product-specific normalization
    public new void Normalize()
    {
        base.Normalize();
        Name = Name?.Trim();
        Category = Category?.Trim();
        Slug = Slug?.Trim();
    }
}

public class BundleFilter : BaseFilter
{
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Override to add bundle-specific normalization
    public new void Normalize()
    {
        base.Normalize();
        Name = Name?.Trim();
    }
}

public class BaseFilter
{
    public EntityStatus? Status { get; set; }
    public string? Search { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";

    // Validation and normalization
    public virtual void Normalize()
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Min(Math.Max(1, PageSize), 100); // Max 100 items per page
        SortDirection = SortDirection?.ToLowerInvariant() == "desc" ? "desc" : "asc";
        Search = Search?.Trim();
    }
}

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public PaginationMeta Meta { get; set; } = new PaginationMeta();
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}