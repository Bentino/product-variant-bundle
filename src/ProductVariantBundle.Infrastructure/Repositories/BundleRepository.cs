using Microsoft.EntityFrameworkCore;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Infrastructure.Data;

namespace ProductVariantBundle.Infrastructure.Repositories;

public class BundleRepository : IBundleRepository
{
    private readonly ApplicationDbContext _context;

    public BundleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductBundle?> GetByIdAsync(Guid id)
    {
        return await _context.ProductBundles
            .Include(b => b.SellableItem)
            .FirstOrDefaultAsync(b => b.Id == id && b.Status == EntityStatus.Active);
    }

    public async Task<ProductBundle?> GetBySKUAsync(string sku)
    {
        return await _context.ProductBundles
            .Include(b => b.SellableItem)
            .Include(b => b.Items)
                .ThenInclude(bi => bi.SellableItem)
            .FirstOrDefaultAsync(b => b.SellableItem != null && 
                                    b.SellableItem.SKU == sku && 
                                    b.Status == EntityStatus.Active);
    }

    public async Task<IEnumerable<ProductBundle>> GetAllAsync()
    {
        return await _context.ProductBundles
            .Include(b => b.SellableItem)
            .Include(b => b.Items)
                .ThenInclude(i => i.SellableItem)
            .AsSplitQuery()
            .Where(b => b.Status == EntityStatus.Active)
            .ToListAsync();
    }

    public async Task<PagedResult<ProductBundle>> GetAllAsync(BundleFilter filter)
    {
        filter.Normalize();

        var query = _context.ProductBundles
            .Include(b => b.SellableItem)
            .Include(b => b.Items)
                .ThenInclude(i => i.SellableItem)
            .AsSplitQuery()
            .AsQueryable();

        // Apply filters
        if (filter.Status.HasValue)
        {
            query = query.Where(b => b.Status == filter.Status.Value);
        }
        else
        {
            query = query.Where(b => b.Status == EntityStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.ToLower();
            query = query.Where(b => 
                b.Name.ToLower().Contains(searchTerm) ||
                b.Description.ToLower().Contains(searchTerm) ||
                (b.SellableItem != null && b.SellableItem.SKU.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(b => b.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(b => b.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(b => b.Price <= filter.MaxPrice.Value);
        }

        if (filter.CreatedAfter.HasValue)
        {
            query = query.Where(b => b.CreatedAt >= filter.CreatedAfter.Value);
        }

        if (filter.CreatedBefore.HasValue)
        {
            query = query.Where(b => b.CreatedAt <= filter.CreatedBefore.Value);
        }

        // Apply sorting
        query = ApplyBundleSorting(query, filter.SortBy, filter.SortDirection);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<ProductBundle>
        {
            Data = items,
            Meta = new PaginationMeta
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
                Total = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
                HasNext = filter.Page * filter.PageSize < totalCount,
                HasPrevious = filter.Page > 1
            }
        };
    }

    private static IQueryable<ProductBundle> ApplyBundleSorting(IQueryable<ProductBundle> query, string? sortBy, string? sortDirection)
    {
        var isDescending = sortDirection?.ToLowerInvariant() == "desc";

        return sortBy?.ToLowerInvariant() switch
        {
            "name" => isDescending ? query.OrderByDescending(b => b.Name) : query.OrderBy(b => b.Name),
            "price" => isDescending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price),
            "sku" => isDescending ? query.OrderByDescending(b => b.SellableItem!.SKU) : query.OrderBy(b => b.SellableItem!.SKU),
            "createdat" => isDescending ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt),
            "updatedat" => isDescending ? query.OrderByDescending(b => b.UpdatedAt) : query.OrderBy(b => b.UpdatedAt),
            _ => query.OrderBy(b => b.Name) // Default sorting by name
        };
    }

    public async Task<ProductBundle> AddAsync(ProductBundle bundle)
    {
        bundle.CreatedAt = DateTime.UtcNow;
        bundle.UpdatedAt = DateTime.UtcNow;
        bundle.Status = EntityStatus.Active;

        _context.ProductBundles.Add(bundle);
        await _context.SaveChangesAsync();
        return bundle;
    }

    public async Task UpdateAsync(ProductBundle bundle)
    {
        bundle.UpdatedAt = DateTime.UtcNow;
        _context.ProductBundles.Update(bundle);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var bundle = await _context.ProductBundles.FindAsync(id);
        if (bundle != null)
        {
            bundle.Status = EntityStatus.Archived;
            bundle.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ProductBundle?> GetByIdWithItemsAsync(Guid id)
    {
        return await _context.ProductBundles
            .Include(b => b.SellableItem)
            .Include(b => b.Items)
                .ThenInclude(bi => bi.SellableItem)
                    .ThenInclude(si => si.InventoryRecords)
                        .ThenInclude(ir => ir.Warehouse)
            .FirstOrDefaultAsync(b => b.Id == id && b.Status == EntityStatus.Active);
    }
}