using Microsoft.EntityFrameworkCore;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Infrastructure.Data;

namespace ProductVariantBundle.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductMaster?> GetByIdAsync(Guid id)
    {
        return await _context.ProductMasters
            .Include(p => p.Variants)
                .ThenInclude(v => v.SellableItem)
            .FirstOrDefaultAsync(p => p.Id == id && p.Status == EntityStatus.Active);
    }

    public async Task<ProductMaster?> GetBySlugAsync(string slug)
    {
        return await _context.ProductMasters
            .Include(p => p.Variants)
                .ThenInclude(v => v.OptionValues)
                    .ThenInclude(ov => ov.VariantOption)
            .Include(p => p.Variants)
                .ThenInclude(v => v.SellableItem)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == EntityStatus.Active);
    }

    public async Task<IEnumerable<ProductMaster>> GetAllAsync()
    {
        return await _context.ProductMasters
            .FromSqlRaw("SELECT * FROM ProductMaster WHERE \"Status\" = 'Active'")
            .Include(p => p.Variants)
                .ThenInclude(v => v.SellableItem)
            .ToListAsync();
    }

    public async Task<PagedResult<ProductMaster>> GetAllAsync(ProductFilter filter)
    {
        filter.Normalize();

        var query = _context.ProductMasters
            .Include(p => p.Variants)
                .ThenInclude(v => v.SellableItem)
            .AsQueryable();

        // Apply filters
        if (filter.Status.HasValue)
        {
            query = query.Where(p => p.Status == filter.Status.Value);
        }
        else
        {
            query = query.Where(p => p.Status == EntityStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm) ||
                p.Slug.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(p => p.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(p => p.Category.ToLower().Contains(filter.Category.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(filter.Slug))
        {
            query = query.Where(p => p.Slug.ToLower().Contains(filter.Slug.ToLower()));
        }

        if (filter.CreatedAfter.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= filter.CreatedAfter.Value);
        }

        if (filter.CreatedBefore.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= filter.CreatedBefore.Value);
        }

        // Apply sorting
        query = ApplyProductSorting(query, filter.SortBy, filter.SortDirection);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<ProductMaster>
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

    private static IQueryable<ProductMaster> ApplyProductSorting(IQueryable<ProductMaster> query, string? sortBy, string? sortDirection)
    {
        var isDescending = sortDirection?.ToLowerInvariant() == "desc";

        return sortBy?.ToLowerInvariant() switch
        {
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "slug" => isDescending ? query.OrderByDescending(p => p.Slug) : query.OrderBy(p => p.Slug),
            "category" => isDescending ? query.OrderByDescending(p => p.Category) : query.OrderBy(p => p.Category),
            "createdat" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "updatedat" => isDescending ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),
            _ => query.OrderBy(p => p.Name) // Default sorting by name
        };
    }

    public async Task<ProductMaster> AddAsync(ProductMaster product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        product.Status = EntityStatus.Active;

        _context.ProductMasters.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateAsync(ProductMaster product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.ProductMasters.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.ProductMasters.FindAsync(id);
        if (product != null)
        {
            product.Status = EntityStatus.Archived;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
    {
        var query = _context.ProductMasters
            .Where(p => p.Slug == slug && p.Status == EntityStatus.Active);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<ProductVariant?> GetVariantByIdAsync(Guid id)
    {
        return await _context.ProductVariants
            .Include(v => v.ProductMaster)
            .Include(v => v.OptionValues)
                .ThenInclude(ov => ov.VariantOption)
            .Include(v => v.SellableItem)
            .FirstOrDefaultAsync(v => v.Id == id && v.Status == EntityStatus.Active);
    }

    public async Task<ProductVariant> AddVariantAsync(ProductVariant variant)
    {
        variant.CreatedAt = DateTime.UtcNow;
        variant.UpdatedAt = DateTime.UtcNow;
        variant.Status = EntityStatus.Active;

        _context.ProductVariants.Add(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task UpdateVariantAsync(ProductVariant variant)
    {
        variant.UpdatedAt = DateTime.UtcNow;
        _context.ProductVariants.Update(variant);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteVariantAsync(Guid variantId)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant != null)
        {
            // Hard delete the variant and its related data
            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> VariantCombinationExistsAsync(Guid productMasterId, string combinationKey, Guid? excludeId = null)
    {
        var query = _context.ProductVariants
            .Where(v => v.ProductMasterId == productMasterId && 
                       v.CombinationKey == combinationKey && 
                       v.Status == EntityStatus.Active);

        if (excludeId.HasValue)
        {
            query = query.Where(v => v.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<VariantOption?> GetVariantOptionAsync(Guid id)
    {
        return await _context.VariantOptions
            .Include(vo => vo.Values)
            .FirstOrDefaultAsync(vo => vo.Id == id && vo.Status == EntityStatus.Active);
    }

    public async Task<IEnumerable<VariantOption>> GetVariantOptionsAsync(Guid productMasterId)
    {
        return await _context.VariantOptions
            .Include(vo => vo.Values)
            .Where(vo => vo.ProductMasterId == productMasterId && vo.Status == EntityStatus.Active)
            .ToListAsync();
    }

    public async Task<VariantOption> AddVariantOptionAsync(VariantOption option)
    {
        option.CreatedAt = DateTime.UtcNow;
        option.UpdatedAt = DateTime.UtcNow;
        option.Status = EntityStatus.Active;

        _context.VariantOptions.Add(option);
        await _context.SaveChangesAsync();
        return option;
    }

    public async Task<VariantOption> UpdateVariantOptionAsync(VariantOption option)
    {
        option.UpdatedAt = DateTime.UtcNow;
        _context.VariantOptions.Update(option);
        await _context.SaveChangesAsync();
        return option;
    }

    public async Task DeleteVariantOptionAsync(Guid id)
    {
        var option = await _context.VariantOptions.FindAsync(id);
        if (option != null)
        {
            option.Status = EntityStatus.Archived;
            option.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}