using ProductVariantBundle.Core.Entities;

namespace ProductVariantBundle.Core.Interfaces;

public interface IWarehouseService
{
    Task<Warehouse?> GetByCodeAsync(string code);
    Task<Warehouse?> GetByIdAsync(Guid id);
    Task<IEnumerable<Warehouse>> GetAllAsync();
    Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
    Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse);
    Task DeleteWarehouseAsync(Guid id);
}