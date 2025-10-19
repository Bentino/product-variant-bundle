using Microsoft.EntityFrameworkCore;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Infrastructure.Data;

namespace ProductVariantBundle.Infrastructure.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly ApplicationDbContext _context;

    public WarehouseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Warehouse?> GetByIdAsync(Guid id)
    {
        return await _context.Warehouses
            .Where(w => w.Status == EntityStatus.Active)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<Warehouse?> GetByCodeAsync(string code)
    {
        return await _context.Warehouses
            .Where(w => w.Status == EntityStatus.Active)
            .FirstOrDefaultAsync(w => w.Code == code);
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
    {
        return await _context.Warehouses
            .Where(w => w.Status == EntityStatus.Active)
            .ToListAsync();
    }

    public async Task<Warehouse> AddAsync(Warehouse warehouse)
    {
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();
        return warehouse;
    }

    public async Task UpdateAsync(Warehouse warehouse)
    {
        _context.Warehouses.Update(warehouse);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse != null)
        {
            warehouse.Status = EntityStatus.Archived;
            warehouse.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
    {
        var query = _context.Warehouses
            .Where(w => w.Status == EntityStatus.Active && w.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(w => w.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}