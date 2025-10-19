using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Core.Interfaces;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id);
    Task<Warehouse?> GetByCodeAsync(string code);
    Task<IEnumerable<Warehouse>> GetAllAsync();
    Task<Warehouse> AddAsync(Warehouse warehouse);
    Task UpdateAsync(Warehouse warehouse);
    Task DeleteAsync(Guid id);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
}