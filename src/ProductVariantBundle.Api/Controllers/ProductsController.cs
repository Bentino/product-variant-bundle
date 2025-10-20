using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductVariantBundle.Api.DTOs.Common;
using ProductVariantBundle.Api.DTOs.Products;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace ProductVariantBundle.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ISellableItemService _sellableItemService;
    private readonly IMapper _mapper;

    public ProductsController(IProductService productService, ISellableItemService sellableItemService, IMapper mapper)
    {
        _productService = productService;
        _sellableItemService = sellableItemService;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all product masters with optional filtering and pagination
    /// </summary>
    /// <param name="filter">Filter criteria including search, category, status, and pagination</param>
    /// <returns>Paginated list of product masters</returns>
    /// <response code="200">Returns the paginated list of product masters</response>
    /// <response code="400">Invalid filter parameters</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all product masters",
        Description = "Retrieves a paginated list of product masters with optional filtering by name, category, and status. Supports search functionality and sorting.",
        OperationId = "GetProductMasters",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(200, "Success", typeof(ApiResponse<PagedResult<ProductMasterDto>>))]
    [SwaggerResponse(400, "Bad Request", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(500, "Internal Server Error", typeof(ProblemDetailsResponse))]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductMasterDto>>>> GetProductMasters(
        [FromQuery] ProductFilterDto filter)
    {
        try
        {
            // Map DTO filter to domain filter
            var domainFilter = _mapper.Map<ProductVariantBundle.Core.Models.ProductFilter>(filter);
            
            var pagedResult = await _productService.GetProductMastersAsync(domainFilter);
            var productDtos = _mapper.Map<IEnumerable<ProductMasterDto>>(pagedResult.Data);
            
            var result = new PagedResult<ProductMasterDto>
            {
                Data = productDtos,
                Meta = _mapper.Map<PaginationMeta>(pagedResult.Meta)
            };

            return Ok(ApiResponse<PagedResult<ProductMasterDto>>.Success(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PagedResult<ProductMasterDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get a specific product master by ID
    /// </summary>
    /// <param name="id">The unique identifier of the product master</param>
    /// <returns>Product master details including all variants</returns>
    /// <response code="200">Returns the product master</response>
    /// <response code="404">Product master not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Get product master by ID",
        Description = "Retrieves a specific product master by its unique identifier, including all associated variants.",
        OperationId = "GetProductMaster",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(200, "Success", typeof(ApiResponse<ProductMasterDto>))]
    [SwaggerResponse(404, "Not Found", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(500, "Internal Server Error", typeof(ProblemDetailsResponse))]
    public async Task<ActionResult<ApiResponse<ProductMasterDto>>> GetProductMaster(Guid id)
    {
        try
        {
            var product = await _productService.GetProductMasterAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductMasterDto>.Error("Product not found"));
            }

            var productDto = _mapper.Map<ProductMasterDto>(product);
            return Ok(ApiResponse<ProductMasterDto>.Success(productDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductMasterDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Create a new product master
    /// </summary>
    /// <param name="createDto">Product master creation data</param>
    /// <returns>Created product master</returns>
    /// <response code="201">Product master created successfully</response>
    /// <response code="400">Invalid input data or validation errors</response>
    /// <response code="409">Product with the same slug already exists</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new product master",
        Description = "Creates a new product master with the provided information. The slug must be unique across all products.",
        OperationId = "CreateProductMaster",
        Tags = new[] { "Products" }
    )]
    [SwaggerResponse(201, "Created", typeof(ApiResponse<ProductMasterDto>))]
    [SwaggerResponse(400, "Bad Request", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(409, "Conflict", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(500, "Internal Server Error", typeof(ProblemDetailsResponse))]
    public async Task<ActionResult<ApiResponse<ProductMasterDto>>> CreateProductMaster(
        [FromBody] CreateProductMasterDto createDto)
    {
        try
        {
            var product = _mapper.Map<ProductMaster>(createDto);
            var createdProduct = await _productService.CreateProductMasterAsync(product);
            var productDto = _mapper.Map<ProductMasterDto>(createdProduct);

            return CreatedAtAction(
                nameof(GetProductMaster),
                new { id = createdProduct.Id },
                ApiResponse<ProductMasterDto>.Success(productDto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<ProductMasterDto>.Error(ex.Message));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<ProductMasterDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductMasterDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Update an existing product master
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductMasterDto>>> UpdateProductMaster(
        Guid id, [FromBody] UpdateProductMasterDto updateDto)
    {
        try
        {
            var existingProduct = await _productService.GetProductMasterAsync(id);
            if (existingProduct == null)
            {
                return NotFound(ApiResponse<ProductMasterDto>.Error("Product not found"));
            }

            _mapper.Map(updateDto, existingProduct);
            var updatedProduct = await _productService.UpdateProductMasterAsync(existingProduct);
            var productDto = _mapper.Map<ProductMasterDto>(updatedProduct);

            return Ok(ApiResponse<ProductMasterDto>.Success(productDto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<ProductMasterDto>.Error(ex.Message));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<ProductMasterDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductMasterDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Delete a product master
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteProductMaster(Guid id)
    {
        try
        {
            var product = await _productService.GetProductMasterAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse.Error("Product not found"));
            }

            await _productService.DeleteProductMasterAsync(id);
            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get variants for a specific product master
    /// </summary>
    [HttpGet("{id:guid}/variants")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductVariantDto>>>> GetProductVariants(Guid id)
    {
        try
        {
            var product = await _productService.GetProductMasterAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<IEnumerable<ProductVariantDto>>.Error("Product not found"));
            }

            var variantDtos = _mapper.Map<IEnumerable<ProductVariantDto>>(product.Variants);
            return Ok(ApiResponse<IEnumerable<ProductVariantDto>>.Success(variantDtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductVariantDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Create a new variant for a product master
    /// </summary>
    /// <param name="id">Product master ID</param>
    /// <param name="createDto">Variant creation data including SKU, price, and option values</param>
    /// <returns>Created product variant</returns>
    /// <response code="201">Variant created successfully</response>
    /// <response code="400">Invalid input data or validation errors</response>
    /// <response code="404">Product master not found</response>
    /// <response code="409">Variant with same option combination or SKU already exists</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("{id:guid}/variants")]
    [SwaggerOperation(
        Summary = "Create a product variant",
        Description = "Creates a new variant for a product master. Each variant must have a unique SKU and unique combination of option values within the product master.",
        OperationId = "CreateProductVariant",
        Tags = new[] { "Products", "Variants" }
    )]
    [SwaggerResponse(201, "Created", typeof(ApiResponse<ProductVariantDto>))]
    [SwaggerResponse(400, "Bad Request", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(404, "Not Found", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(409, "Conflict", typeof(ProblemDetailsResponse))]
    [SwaggerResponse(500, "Internal Server Error", typeof(ProblemDetailsResponse))]
    public async Task<ActionResult<ApiResponse<ProductVariantDto>>> CreateProductVariant(
        Guid id, [FromBody] CreateProductVariantDto createDto)
    {
        try
        {
            // Check model state first
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                throw new ValidationException(errors);
            }

            if (createDto.ProductMasterId != id)
            {
                throw new ValidationException("ProductMasterId", "Product master ID mismatch");
            }

            // Map DTO to entity
            var variant = _mapper.Map<ProductVariant>(createDto);
            
            // Set variant ID first
            variant.Id = Guid.NewGuid();
            
            // Process OptionValues - handle both VariantOptionId and OptionName
            var optionValues = new List<VariantOptionValue>();
            
            foreach (var ov in createDto.OptionValues)
            {
                Guid variantOptionId;
                
                if (ov.VariantOptionId.HasValue)
                {
                    // Use existing VariantOptionId
                    variantOptionId = ov.VariantOptionId.Value;
                }
                else if (!string.IsNullOrWhiteSpace(ov.OptionName))
                {
                    // Find or create VariantOption by name
                    var existingOption = await _productService.GetVariantOptionByNameAsync(id, ov.OptionName);
                    
                    if (existingOption != null)
                    {
                        variantOptionId = existingOption.Id;
                    }
                    else
                    {
                        // Create new VariantOption
                        var newOption = new VariantOption
                        {
                            Name = ov.OptionName,
                            Slug = ov.OptionName.ToLowerInvariant().Replace(" ", "-"),
                            ProductMasterId = id
                        };
                        
                        var createdOption = await _productService.CreateVariantOptionAsync(newOption);
                        variantOptionId = createdOption.Id;
                    }
                }
                else
                {
                    throw new ValidationException("OptionValues", "Either VariantOptionId or OptionName must be provided");
                }
                
                optionValues.Add(new VariantOptionValue
                {
                    Id = Guid.NewGuid(),
                    VariantOptionId = variantOptionId,
                    Value = ov.Value,
                    ProductVariantId = variant.Id,
                    Status = EntityStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            
            variant.OptionValues = optionValues;
            
            // Create the variant
            var createdVariant = await _productService.CreateVariantAsync(variant);
            
            // Create associated sellable item with SKU
            await _sellableItemService.CreateSellableItemAsync(
                SellableItemType.Variant, 
                createdVariant.Id, 
                createDto.SKU
            );
            
            var variantDto = _mapper.Map<ProductVariantDto>(createdVariant);
            variantDto.SKU = createDto.SKU; // Set SKU manually

            return CreatedAtAction(
                nameof(GetProductVariant),
                new { id = id, variantId = createdVariant.Id },
                ApiResponse<ProductVariantDto>.Success(variantDto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<ProductVariantDto>.Error(ex.Message));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<ProductVariantDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductVariantDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get a specific variant
    /// </summary>
    [HttpGet("{id:guid}/variants/{variantId:guid}")]
    public async Task<ActionResult<ApiResponse<ProductVariantDto>>> GetProductVariant(Guid id, Guid variantId)
    {
        try
        {
            var variant = await _productService.GetVariantAsync(variantId);
            if (variant == null || variant.ProductMasterId != id)
            {
                return NotFound(ApiResponse<ProductVariantDto>.Error("Variant not found"));
            }

            var variantDto = _mapper.Map<ProductVariantDto>(variant);
            return Ok(ApiResponse<ProductVariantDto>.Success(variantDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductVariantDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Update a product variant
    /// </summary>
    [HttpPut("{id:guid}/variants/{variantId:guid}")]
    public async Task<ActionResult<ApiResponse<ProductVariantDto>>> UpdateProductVariant(
        Guid id, Guid variantId, [FromBody] UpdateProductVariantDto updateDto)
    {
        try
        {
            var existingVariant = await _productService.GetVariantAsync(variantId);
            if (existingVariant == null || existingVariant.ProductMasterId != id)
            {
                return NotFound(ApiResponse<ProductVariantDto>.Error("Variant not found"));
            }

            _mapper.Map(updateDto, existingVariant);
            var updatedVariant = await _productService.UpdateVariantAsync(existingVariant);
            var variantDto = _mapper.Map<ProductVariantDto>(updatedVariant);

            return Ok(ApiResponse<ProductVariantDto>.Success(variantDto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<ProductVariantDto>.Error(ex.Message));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<ProductVariantDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductVariantDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Delete a product variant
    /// </summary>
    [HttpDelete("{id:guid}/variants/{variantId:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteProductVariant(Guid id, Guid variantId)
    {
        try
        {
            var variant = await _productService.GetVariantAsync(variantId);
            if (variant == null || variant.ProductMasterId != id)
            {
                return NotFound(ApiResponse.Error("Variant not found"));
            }

            await _productService.DeleteVariantAsync(variantId);
            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get variant options for a specific product master
    /// </summary>
    [HttpGet("{id:guid}/options")]
    public async Task<ActionResult<ApiResponse<IEnumerable<VariantOptionDto>>>> GetProductVariantOptions(Guid id)
    {
        try
        {
            var product = await _productService.GetProductMasterAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<IEnumerable<VariantOptionDto>>.Error("Product not found"));
            }

            var options = await _productService.GetVariantOptionsAsync(id);
            var optionDtos = _mapper.Map<IEnumerable<VariantOptionDto>>(options);
            return Ok(ApiResponse<IEnumerable<VariantOptionDto>>.Success(optionDtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<VariantOptionDto>>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Create a variant option for a product master
    /// </summary>
    [HttpPost("{id:guid}/options")]
    public async Task<ActionResult<ApiResponse<VariantOptionDto>>> CreateVariantOption(
        Guid id, [FromBody] CreateVariantOptionDto createDto)
    {
        try
        {
            var product = await _productService.GetProductMasterAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<VariantOptionDto>.Error("Product not found"));
            }

            var option = _mapper.Map<VariantOption>(createDto);
            option.ProductMasterId = id;
            
            var createdOption = await _productService.CreateVariantOptionAsync(option);
            var optionDto = _mapper.Map<VariantOptionDto>(createdOption);

            return CreatedAtAction(
                nameof(GetVariantOption),
                new { id = id, optionId = createdOption.Id },
                ApiResponse<VariantOptionDto>.Success(optionDto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<VariantOptionDto>.Error(ex.Message));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<VariantOptionDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<VariantOptionDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Get a specific variant option
    /// </summary>
    [HttpGet("{id:guid}/options/{optionId:guid}")]
    public async Task<ActionResult<ApiResponse<VariantOptionDto>>> GetVariantOption(Guid id, Guid optionId)
    {
        try
        {
            var option = await _productService.GetVariantOptionAsync(optionId);
            if (option == null || option.ProductMasterId != id)
            {
                return NotFound(ApiResponse<VariantOptionDto>.Error("Variant option not found"));
            }

            var optionDto = _mapper.Map<VariantOptionDto>(option);
            return Ok(ApiResponse<VariantOptionDto>.Success(optionDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<VariantOptionDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Update a variant option
    /// </summary>
    [HttpPut("{id:guid}/options/{optionId:guid}")]
    public async Task<ActionResult<ApiResponse<VariantOptionDto>>> UpdateVariantOption(
        Guid id, Guid optionId, [FromBody] UpdateVariantOptionDto updateDto)
    {
        try
        {
            var existingOption = await _productService.GetVariantOptionAsync(optionId);
            if (existingOption == null || existingOption.ProductMasterId != id)
            {
                return NotFound(ApiResponse<VariantOptionDto>.Error("Variant option not found"));
            }

            _mapper.Map(updateDto, existingOption);
            var updatedOption = await _productService.UpdateVariantOptionAsync(existingOption);
            var optionDto = _mapper.Map<VariantOptionDto>(updatedOption);

            return Ok(ApiResponse<VariantOptionDto>.Success(optionDto));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<VariantOptionDto>.Error(ex.Message));
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(ApiResponse<VariantOptionDto>.Error(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<VariantOptionDto>.Error($"Internal server error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Delete a variant option
    /// </summary>
    [HttpDelete("{id:guid}/options/{optionId:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteVariantOption(Guid id, Guid optionId)
    {
        try
        {
            var option = await _productService.GetVariantOptionAsync(optionId);
            if (option == null || option.ProductMasterId != id)
            {
                return NotFound(ApiResponse.Error("Variant option not found"));
            }

            await _productService.DeleteVariantOptionAsync(optionId);
            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Error($"Internal server error: {ex.Message}"));
        }
    }
}