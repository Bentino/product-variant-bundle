using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductVariantBundle.Api.DTOs.Common;
using ProductVariantBundle.Api.DTOs.Batch;
using ProductVariantBundle.Api.DTOs.Products;
using ProductVariantBundle.Api.DTOs.Bundles;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Exceptions;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductVariantBundle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BatchController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IBundleService _bundleService;
    private readonly IMapper _mapper;

    public BatchController(
        IProductService productService,
        IBundleService bundleService,
        IMapper mapper)
    {
        _productService = productService;
        _bundleService = bundleService;
        _mapper = mapper;
    }

    /// <summary>
    /// Create multiple variants in a single batch operation
    /// </summary>
    /// <param name="request">Batch request containing variants to create and operation settings</param>
    /// <returns>Batch operation result with success/failure details for each item</returns>
    /// <response code="200">Batch operation completed (check individual results for success/failure)</response>
    /// <response code="400">Invalid batch request or validation errors</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("variants")]
    [SwaggerOperation(
        Summary = "Create multiple variants in batch",
        Description = "Creates multiple product variants in a single transaction. Supports idempotency keys and conflict resolution strategies (skip, update, fail). All operations are atomic - if any critical error occurs, all changes are rolled back.",
        OperationId = "CreateVariantsBatch",
        Tags = new[] { "Batch", "Variants" }
    )]
    [SwaggerResponse(200, "Success", typeof(ApiResponse<BatchOperationResultDto<ProductVariantDto>>))]
    [SwaggerResponse(400, "Bad Request", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(500, "Internal Server Error", typeof(ProblemDetailsResponse))]
    public async Task<ActionResult<ApiResponse<BatchOperationResultDto<ProductVariantDto>>>> CreateVariantsBatch(
        [FromBody] BatchCreateVariantRequestDto request)
    {
        try
        {
            var batchRequest = _mapper.Map<ProductVariantBundle.Core.Models.BatchCreateVariantRequest>(request);
            var result = await _productService.CreateVariantsBatchAsync(batchRequest);
            
            var resultDto = new BatchOperationResultDto<ProductVariantDto>
            {
                SuccessCount = result.SuccessCount,
                FailureCount = result.FailureCount,
                TotalProcessed = result.TotalProcessed,
                IdempotencyKey = result.IdempotencyKey,
                OnConflict = _mapper.Map<ConflictResolutionStrategy>(result.OnConflict),
                Results = result.Results.Select(r => new BatchItemResultDto<ProductVariantDto>
                {
                    Index = r.Index,
                    Success = r.Success,
                    Data = r.Success && r.Data != null ? _mapper.Map<ProductVariantDto>(r.Data) : null,
                    Errors = r.Errors
                })
            };

            var meta = new
            {
                operation = "batch_create_variants",
                timestamp = DateTime.UtcNow
            };

            return Ok(ApiResponse<BatchOperationResultDto<ProductVariantDto>>.Success(resultDto, meta));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<BatchOperationResultDto<ProductVariantDto>>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BatchOperationResultDto<ProductVariantDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Update multiple variants in a single batch operation
    /// </summary>
    [HttpPut("variants")]
    public async Task<ActionResult<ApiResponse<BatchOperationResultDto<ProductVariantDto>>>> UpdateVariantsBatch(
        [FromBody] BatchUpdateVariantRequestDto request)
    {
        try
        {
            var batchRequest = _mapper.Map<ProductVariantBundle.Core.Models.BatchUpdateVariantRequest>(request);
            var result = await _productService.UpdateVariantsBatchAsync(batchRequest);
            
            var resultDto = new BatchOperationResultDto<ProductVariantDto>
            {
                SuccessCount = result.SuccessCount,
                FailureCount = result.FailureCount,
                TotalProcessed = result.TotalProcessed,
                IdempotencyKey = result.IdempotencyKey,
                OnConflict = _mapper.Map<ConflictResolutionStrategy>(result.OnConflict),
                Results = result.Results.Select(r => new BatchItemResultDto<ProductVariantDto>
                {
                    Index = r.Index,
                    Success = r.Success,
                    Data = r.Success && r.Data != null ? _mapper.Map<ProductVariantDto>(r.Data) : null,
                    Errors = r.Errors
                })
            };

            var meta = new
            {
                operation = "batch_update_variants",
                timestamp = DateTime.UtcNow
            };

            return Ok(ApiResponse<BatchOperationResultDto<ProductVariantDto>>.Success(resultDto, meta));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<BatchOperationResultDto<ProductVariantDto>>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BatchOperationResultDto<ProductVariantDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Create multiple bundles in a single batch operation
    /// </summary>
    [HttpPost("bundles")]
    public async Task<ActionResult<ApiResponse<BatchOperationResultDto<ProductBundleDto>>>> CreateBundlesBatch(
        [FromBody] BatchCreateBundleRequestDto request)
    {
        try
        {
            var batchRequest = _mapper.Map<ProductVariantBundle.Core.Models.BatchCreateBundleRequest>(request);
            var result = await _bundleService.CreateBundlesBatchAsync(batchRequest);
            
            var resultDto = new BatchOperationResultDto<ProductBundleDto>
            {
                SuccessCount = result.SuccessCount,
                FailureCount = result.FailureCount,
                TotalProcessed = result.TotalProcessed,
                IdempotencyKey = result.IdempotencyKey,
                OnConflict = _mapper.Map<ConflictResolutionStrategy>(result.OnConflict),
                Results = result.Results.Select(r => new BatchItemResultDto<ProductBundleDto>
                {
                    Index = r.Index,
                    Success = r.Success,
                    Data = r.Success && r.Data != null ? _mapper.Map<ProductBundleDto>(r.Data) : null,
                    Errors = r.Errors
                })
            };

            var meta = new
            {
                operation = "batch_create_bundles",
                timestamp = DateTime.UtcNow
            };

            return Ok(ApiResponse<BatchOperationResultDto<ProductBundleDto>>.Success(resultDto, meta));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<BatchOperationResultDto<ProductBundleDto>>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<BatchOperationResultDto<ProductBundleDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }
}