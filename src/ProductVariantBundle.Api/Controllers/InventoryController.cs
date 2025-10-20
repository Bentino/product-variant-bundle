using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductVariantBundle.Api.DTOs.Common;
using ProductVariantBundle.Api.DTOs.Inventory;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductVariantBundle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly IMapper _mapper;

    public InventoryController(IInventoryService inventoryService, IMapper mapper)
    {
        _inventoryService = inventoryService;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a new inventory record for a SKU
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<InventoryRecordDto>>> CreateInventoryRecord(
        [FromBody] CreateInventoryBySKUDto createDto)
    {
        try
        {
            // Find sellable item by SKU
            var sellableItem = await _inventoryService.GetSellableItemBySKUAsync(createDto.SKU);
            if (sellableItem == null)
            {
                return NotFound(ApiResponse<InventoryRecordDto>.Error($"SellableItem with SKU '{createDto.SKU}' not found"));
            }

            // Find warehouse by code
            var warehouse = await _inventoryService.GetWarehouseByCodeAsync(createDto.WarehouseCode);
            if (warehouse == null)
            {
                return NotFound(ApiResponse<InventoryRecordDto>.Error($"Warehouse with code '{createDto.WarehouseCode}' not found"));
            }

            var inventoryRecord = await _inventoryService.CreateInventoryRecordAsync(
                sellableItem.Id, 
                warehouse.Id, 
                createDto.OnHand, 
                createDto.Reserved);
            
            var inventoryDto = _mapper.Map<InventoryRecordDto>(inventoryRecord);
            return CreatedAtAction(nameof(GetInventory), 
                new { sku = createDto.SKU, warehouseCode = createDto.WarehouseCode }, 
                ApiResponse<InventoryRecordDto>.Success(inventoryDto));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InventoryRecordDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get paginated inventory records with optional filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DTOs.Common.PagedResult<InventoryRecordDto>>>> GetInventoryRecords(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? warehouse = null,
        [FromQuery] string sortBy = "sku",
        [FromQuery] string sortDirection = "asc")
    {
        try
        {
            var result = await _inventoryService.GetInventoryRecordsAsync(page, pageSize, search, warehouse, sortBy, sortDirection);
            var mappedResult = new DTOs.Common.PagedResult<InventoryRecordDto>
            {
                Data = _mapper.Map<List<InventoryRecordDto>>(result.Data),
                Meta = new DTOs.Common.PaginationMeta
                {
                    Page = result.Meta.Page,
                    PageSize = result.Meta.PageSize,
                    Total = result.Meta.Total,
                    TotalPages = result.Meta.TotalPages,
                    HasNext = result.Meta.HasNext,
                    HasPrevious = result.Meta.HasPrevious
                }
            };

            return Ok(ApiResponse<DTOs.Common.PagedResult<InventoryRecordDto>>.Success(mappedResult));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<DTOs.Common.PagedResult<InventoryRecordDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get inventory record for a specific SKU and warehouse
    /// </summary>
    [HttpGet("{sku}")]
    public async Task<ActionResult<ApiResponse<InventoryRecordDto>>> GetInventory(
        string sku, [FromQuery] string warehouseCode = "MAIN")
    {
        try
        {
            var inventory = await _inventoryService.GetInventoryRecordAsync(sku, warehouseCode);
            if (inventory == null)
            {
                return NotFound(ApiResponse<InventoryRecordDto>.Error("Inventory record not found"));
            }

            var inventoryDto = _mapper.Map<InventoryRecordDto>(inventory);
            return Ok(ApiResponse<InventoryRecordDto>.Success(inventoryDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InventoryRecordDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Update stock levels for a specific SKU (both on-hand and reserved)
    /// </summary>
    [HttpPut("{sku}/stock")]
    public async Task<ActionResult<ApiResponse<InventoryRecordDto>>> UpdateStock(
        string sku, [FromBody] UpdateStockDto updateDto)
    {
        try
        {
            if (sku != updateDto.SKU)
            {
                return BadRequest(ApiResponse<InventoryRecordDto>.Error("SKU mismatch"));
            }

            await _inventoryService.UpdateStockAsync(updateDto.SKU, updateDto.OnHand, updateDto.Reserved, updateDto.WarehouseCode);
            
            var updatedInventory = await _inventoryService.GetInventoryRecordAsync(sku, updateDto.WarehouseCode);
            var inventoryDto = _mapper.Map<InventoryRecordDto>(updatedInventory);

            return Ok(ApiResponse<InventoryRecordDto>.Success(inventoryDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InventoryRecordDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Update only on-hand stock quantity (preserves reserved quantity)
    /// </summary>
    [HttpPatch("{sku}/onhand")]
    public async Task<ActionResult<ApiResponse<InventoryRecordDto>>> UpdateOnHandStock(
        string sku, [FromBody] UpdateInventoryDto updateDto)
    {
        try
        {
            if (sku != updateDto.SKU)
            {
                return BadRequest(ApiResponse<InventoryRecordDto>.Error("SKU mismatch"));
            }

            var currentInventory = await _inventoryService.GetInventoryRecordAsync(sku, updateDto.WarehouseCode);
            if (currentInventory == null)
            {
                return NotFound(ApiResponse<InventoryRecordDto>.Error("Inventory record not found"));
            }

            await _inventoryService.UpdateStockAsync(updateDto.SKU, updateDto.Quantity, currentInventory.Reserved, updateDto.WarehouseCode);
            
            var updatedInventory = await _inventoryService.GetInventoryRecordAsync(sku, updateDto.WarehouseCode);
            var inventoryDto = _mapper.Map<InventoryRecordDto>(updatedInventory);

            return Ok(ApiResponse<InventoryRecordDto>.Success(inventoryDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InventoryRecordDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Reserve stock for a specific SKU
    /// </summary>
    [HttpPost("{sku}/reserve")]
    public async Task<ActionResult<ApiResponse<InventoryRecordDto>>> ReserveStock(
        string sku, [FromBody] ReserveInventoryDto reserveDto)
    {
        try
        {
            if (sku != reserveDto.SKU)
            {
                return BadRequest(ApiResponse<InventoryRecordDto>.Error("SKU mismatch"));
            }

            await _inventoryService.ReserveStockAsync(reserveDto.SKU, reserveDto.Quantity, reserveDto.WarehouseCode);
            
            var updatedInventory = await _inventoryService.GetInventoryRecordAsync(sku, reserveDto.WarehouseCode);
            var inventoryDto = _mapper.Map<InventoryRecordDto>(updatedInventory);

            return Ok(ApiResponse<InventoryRecordDto>.Success(inventoryDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InventoryRecordDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Release reserved stock for a specific SKU
    /// </summary>
    [HttpPost("{sku}/release")]
    public async Task<ActionResult<ApiResponse<InventoryRecordDto>>> ReleaseReservedStock(
        string sku, [FromBody] ReserveInventoryDto releaseDto)
    {
        try
        {
            if (sku != releaseDto.SKU)
            {
                return BadRequest(ApiResponse<InventoryRecordDto>.Error("SKU mismatch"));
            }

            await _inventoryService.ReleaseReservedStockAsync(releaseDto.SKU, releaseDto.Quantity, releaseDto.WarehouseCode);
            
            var updatedInventory = await _inventoryService.GetInventoryRecordAsync(sku, releaseDto.WarehouseCode);
            var inventoryDto = _mapper.Map<InventoryRecordDto>(updatedInventory);

            return Ok(ApiResponse<InventoryRecordDto>.Success(inventoryDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<InventoryRecordDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<InventoryRecordDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get available stock quantity for a specific SKU
    /// </summary>
    [HttpGet("{sku}/available")]
    public async Task<ActionResult<ApiResponse<int>>> GetAvailableStock(
        string sku, [FromQuery] string warehouseCode = "MAIN")
    {
        try
        {
            var availableStock = await _inventoryService.GetAvailableStockAsync(sku, warehouseCode);
            return Ok(ApiResponse<int>.Success(availableStock));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<int>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<int>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get detailed stock inquiry with row locking for reservation purposes
    /// </summary>
    [HttpPost("{sku}/inquiry")]
    public async Task<ActionResult<ApiResponse<StockInquiryDto>>> GetStockInquiry(
        string sku, [FromQuery] string warehouseCode = "MAIN", [FromQuery] bool withLock = false)
    {
        try
        {
            InventoryRecord? inventory;
            
            if (withLock)
            {
                // Use row locking when preparing for reservation
                inventory = await _inventoryService.GetInventoryRecordWithLockAsync(sku, warehouseCode);
            }
            else
            {
                inventory = await _inventoryService.GetInventoryRecordAsync(sku, warehouseCode);
            }

            if (inventory == null)
            {
                return NotFound(ApiResponse<StockInquiryDto>.Error("Inventory record not found"));
            }

            var inquiryDto = new StockInquiryDto
            {
                SKU = sku,
                WarehouseCode = warehouseCode,
                OnHand = inventory.OnHand,
                Reserved = inventory.Reserved,
                Available = inventory.OnHand - inventory.Reserved,
                LastUpdated = inventory.UpdatedAt
            };

            return Ok(ApiResponse<StockInquiryDto>.Success(inquiryDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<StockInquiryDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<StockInquiryDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get all inventory records for a specific warehouse
    /// </summary>
    [HttpGet("warehouse/{warehouseCode}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InventoryRecordDto>>>> GetWarehouseInventory(
        string warehouseCode)
    {
        try
        {
            var inventoryRecords = await _inventoryService.GetWarehouseInventoryAsync(warehouseCode);
            var inventoryDtos = _mapper.Map<IEnumerable<InventoryRecordDto>>(inventoryRecords);
            return Ok(ApiResponse<IEnumerable<InventoryRecordDto>>.Success(inventoryDtos));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<IEnumerable<InventoryRecordDto>>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<InventoryRecordDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }
}