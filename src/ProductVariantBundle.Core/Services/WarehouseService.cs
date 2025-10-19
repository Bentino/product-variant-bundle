using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;

namespace ProductVariantBundle.Core.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<Warehouse?> GetByCodeAsync(string code)
    {
        return await _warehouseRepository.GetByCodeAsync(code);
    }

    public async Task<Warehouse?> GetByIdAsync(Guid id)
    {
        return await _warehouseRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
    {
        return await _warehouseRepository.GetAllAsync();
    }

    public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
    {
        // Validate code uniqueness
        if (await _warehouseRepository.CodeExistsAsync(warehouse.Code))
        {
            throw new DuplicateEntityException("Warehouse", "code", warehouse.Code);
        }

        // Set audit fields
        warehouse.Id = Guid.NewGuid();
        warehouse.CreatedAt = DateTime.UtcNow;
        warehouse.UpdatedAt = DateTime.UtcNow;
        warehouse.Status = EntityStatus.Active;

        return await _warehouseRepository.AddAsync(warehouse);
    }

    public async Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse)
    {
        var existing = await _warehouseRepository.GetByIdAsync(warehouse.Id);
        if (existing == null)
        {
            throw new EntityNotFoundException("Warehouse", warehouse.Id);
        }

        // Validate code uniqueness if changed
        if (warehouse.Code != existing.Code)
        {
            if (await _warehouseRepository.CodeExistsAsync(warehouse.Code, warehouse.Id))
            {
                throw new DuplicateEntityException("Warehouse", "code", warehouse.Code);
            }
        }

        // Update audit fields
        warehouse.UpdatedAt = DateTime.UtcNow;

        await _warehouseRepository.UpdateAsync(warehouse);
        return warehouse;
    }

    public async Task DeleteWarehouseAsync(Guid id)
    {
        var existing = await _warehouseRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new EntityNotFoundException("Warehouse", id);
        }

        await _warehouseRepository.DeleteAsync(id);
    }
}