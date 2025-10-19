namespace ProductVariantBundle.Api.DTOs.Common;

public class PaginationDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
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

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public PaginationMeta Meta { get; set; } = new PaginationMeta();
}