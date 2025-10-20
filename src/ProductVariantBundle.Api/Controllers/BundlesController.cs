using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductVariantBundle.Api.DTOs.Common;
using ProductVariantBundle.Api.DTOs.Bundles;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductVariantBundle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BundlesController : ControllerBase
{
    private readonly IBundleService _bundleService;
    private readonly IMapper _mapper;

    public BundlesController(IBundleService bundleService, IMapper mapper)
    {
        _bundleService = bundleService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all bundles with optional filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductBundleDto>>>> GetBundles(
        [FromQuery] BundleFilterDto filter)
    {
        try
        {
            // Map DTO filter to domain filter
            var domainFilter = _mapper.Map<ProductVariantBundle.Core.Models.BundleFilter>(filter);
            
            var pagedResult = await _bundleService.GetBundlesAsync(domainFilter);
            var bundleDtos = _mapper.Map<IEnumerable<ProductBundleDto>>(pagedResult.Data);
            
            var result = new PagedResult<ProductBundleDto>
            {
                Data = bundleDtos,
                Meta = _mapper.Map<PaginationMeta>(pagedResult.Meta)
            };

            return Ok(ApiResponse<PagedResult<ProductBundleDto>>.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<ProductBundleDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get a specific bundle by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductBundleDto>>> GetBundle(Guid id)
    {
        try
        {
            var bundle = await _bundleService.GetBundleWithItemsAsync(id);
            if (bundle == null)
            {
                return NotFound(ApiResponse<ProductBundleDto>.Error("Bundle not found"));
            }

            var bundleDto = _mapper.Map<ProductBundleDto>(bundle);
            return Ok(ApiResponse<ProductBundleDto>.Success(bundleDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductBundleDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Create a new bundle
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductBundleDto>>> CreateBundle(
        [FromBody] CreateProductBundleDto createDto)
    {
        try
        {
            var bundle = _mapper.Map<ProductBundle>(createDto);
            var createdBundle = await _bundleService.CreateBundleAsync(bundle);
            var bundleDto = _mapper.Map<ProductBundleDto>(createdBundle);

            return CreatedAtAction(
                nameof(GetBundle),
                new { id = createdBundle.Id },
                ApiResponse<ProductBundleDto>.Success(bundleDto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<ProductBundleDto>.Error(ex.Message));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<ProductBundleDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductBundleDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Update an existing bundle
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductBundleDto>>> UpdateBundle(
        Guid id, [FromBody] UpdateProductBundleDto updateDto)
    {
        try
        {
            var existingBundle = await _bundleService.GetBundleAsync(id);
            if (existingBundle == null)
            {
                return NotFound(ApiResponse<ProductBundleDto>.Error("Bundle not found"));
            }

            _mapper.Map(updateDto, existingBundle);
            var updatedBundle = await _bundleService.UpdateBundleAsync(existingBundle);
            var bundleDto = _mapper.Map<ProductBundleDto>(updatedBundle);

            return Ok(ApiResponse<ProductBundleDto>.Success(bundleDto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<ProductBundleDto>.Error(ex.Message));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<ProductBundleDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductBundleDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Delete a bundle
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteBundle(Guid id)
    {
        try
        {
            var bundle = await _bundleService.GetBundleAsync(id);
            if (bundle == null)
            {
                return NotFound(ApiResponse.Error("Bundle not found"));
            }

            await _bundleService.DeleteBundleAsync(id);
            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Calculate bundle availability
    /// </summary>
    /// <param name="id">Bundle ID</param>
    /// <param name="warehouseCode">Warehouse code (default: MAIN)</param>
    /// <returns>Bundle availability information including component details</returns>
    /// <response code="200">Returns bundle availability calculation</response>
    /// <response code="404">Bundle not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}/availability")]
    [SwaggerOperation(
        Summary = "Calculate bundle availability",
        Description = "Calculates how many units of the bundle can be fulfilled based on current inventory levels of all components. Uses the formula: min(floor((on_hand - reserved) / required_quantity)) for each component.",
        OperationId = "GetBundleAvailability",
        Tags = new[] { "Bundles", "Inventory" }
    )]
    [SwaggerResponse(200, "Success", typeof(ApiResponse<BundleAvailabilityDto>))]
    [SwaggerResponse(404, "Not Found", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(500, "Internal Server Error", typeof(ProblemDetailsResponse))]
    public async Task<ActionResult<ApiResponse<BundleAvailabilityDto>>> GetBundleAvailability(
        Guid id, [FromQuery] string warehouseCode = "MAIN")
    {
        try
        {
            var availability = await _bundleService.CalculateAvailabilityAsync(id, warehouseCode);
            var availabilityDto = _mapper.Map<BundleAvailabilityDto>(availability);
            availabilityDto.WarehouseCode = warehouseCode;

            return Ok(ApiResponse<BundleAvailabilityDto>.Success(availabilityDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<BundleAvailabilityDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BundleAvailabilityDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Calculate bundle availability with transaction-safe row locking for reservation purposes
    /// </summary>
    [HttpPost("{id:guid}/availability/reserve-check")]
    public async Task<ActionResult<ApiResponse<BundleAvailabilityDto>>> GetBundleAvailabilityWithLock(
        Guid id, [FromQuery] string warehouseCode = "MAIN")
    {
        try
        {
            var availability = await _bundleService.CalculateAvailabilityWithLockAsync(id, warehouseCode);
            var availabilityDto = _mapper.Map<BundleAvailabilityDto>(availability);
            availabilityDto.WarehouseCode = warehouseCode;

            return Ok(ApiResponse<BundleAvailabilityDto>.Success(availabilityDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ApiResponse<BundleAvailabilityDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BundleAvailabilityDto>.Error($"Internal server error: {ex.Message}"));
        }
    }
}