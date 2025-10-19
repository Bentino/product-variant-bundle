using FluentAssertions;
using Moq;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Core.Services;
using ProductVariantBundle.Core.Validators;
using Xunit;

namespace ProductVariantBundle.Tests.Core.Services;

public class ProductServiceBatchTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ISellableItemService> _mockSellableItemService;
    private readonly Mock<ProductValidator> _mockValidator;
    private readonly Mock<BatchOperationService> _mockBatchOperationService;
    private readonly ProductService _service;

    public ProductServiceBatchTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockSellableItemService = new Mock<ISellableItemService>();
        _mockValidator = new Mock<ProductValidator>(_mockProductRepository.Object);
        _mockBatchOperationService = new Mock<BatchOperationService>(Mock.Of<IBatchOperationRepository>());
        
        _service = new ProductService(
            _mockProductRepository.Object,
            _mockSellableItemService.Object,
            _mockValidator.Object,
            _mockBatchOperationService.Object);
    }

    #region Batch Create Variants Tests

    [Fact]
    public async Task CreateVariantsBatchAsync_WithExistingIdempotencyKey_ShouldReturnCachedResult()
    {
        // Arrange
        var idempotencyKey = "existing-key-001";
        var cachedResult = new BatchOperationResult<ProductVariant>
        {
            IdempotencyKey = idempotencyKey,
            TotalProcessed = 2,
            SuccessCount = 2,
            FailureCount = 0,
            OnConflict = ConflictResolutionStrategy.Fail
        };

        var request = new BatchCreateVariantRequest
        {
            IdempotencyKey = idempotencyKey,
            OnConflict = ConflictResolutionStrategy.Fail,
            Items = new List<ProductVariantBatchItem>
            {
                CreateValidBatchItem("SKU-001"),
                CreateValidBatchItem("SKU-002")
            }
        };

        _mockBatchOperationService.Setup(x => x.GetExistingOperationAsync<ProductVariant>(idempotencyKey))
            .ReturnsAsync(cachedResult);

        // Act
        var result = await _service.CreateVariantsBatchAsync(request);

        // Assert
        result.Should().Be(cachedResult);
        _mockProductRepository.Verify(x => x.AddVariantAsync(It.IsAny<ProductVariant>()), Times.Never);
    }

    [Fact]
    public async Task CreateVariantsBatchAsync_WithValidItems_ShouldCreateAllVariants()
    {
        // Arrange
        var idempotencyKey = "create-batch-001";
        var productMasterId = Guid.NewGuid();
        var request = new BatchCreateVariantRequest
        {
            IdempotencyKey = idempotencyKey,
            OnConflict = ConflictResolutionStrategy.Fail,
            Items = new List<ProductVariantBatchItem>
            {
                CreateValidBatchItem("SKU-001", productMasterId),
                CreateValidBatchItem("SKU-002", productMasterId)
            }
        };

        SetupSuccessfulBatchCreation(productMasterId);

        // Act
        var result = await _service.CreateVariantsBatchAsync(request);

        // Assert
        result.IdempotencyKey.Should().Be(idempotencyKey);
        result.TotalProcessed.Should().Be(2);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(0);
        result.Results.Should().HaveCount(2);
        result.Results.Should().AllSatisfy(r => r.Success.Should().BeTrue());
    }

    [Fact]
    public async Task CreateVariantsBatchAsync_WithDuplicateCombination_ShouldHandleConflictStrategy()
    {
        // Arrange
        var idempotencyKey = "conflict-test-001";
        var productMasterId = Guid.NewGuid();
        var request = new BatchCreateVariantRequest
        {
            IdempotencyKey = idempotencyKey,
            OnConflict = ConflictResolutionStrategy.Skip,
            Items = new List<ProductVariantBatchItem>
            {
                CreateValidBatchItem("SKU-001", productMasterId),
                CreateValidBatchItem("SKU-002", productMasterId) // This will have duplicate combination
            }
        };

        SetupConflictScenario(productMasterId);

        // Act
        var result = await _service.CreateVariantsBatchAsync(request);

        // Assert
        result.TotalProcessed.Should().Be(2);
        result.SuccessCount.Should().Be(2); // Both should succeed (one created, one skipped)
        result.FailureCount.Should().Be(0);
        
        var firstResult = result.Results.First();
        firstResult.Success.Should().BeTrue();
        firstResult.Data.Should().NotBeNull();

        var secondResult = result.Results.Skip(1).First();
        secondResult.Success.Should().BeTrue();
        secondResult.Data.Should().BeNull(); // Skipped
    }

    [Fact]
    public async Task CreateVariantsBatchAsync_WithFailStrategy_ShouldFailOnConflict()
    {
        // Arrange
        var idempotencyKey = "fail-strategy-001";
        var productMasterId = Guid.NewGuid();
        var request = new BatchCreateVariantRequest
        {
            IdempotencyKey = idempotencyKey,
            OnConflict = ConflictResolutionStrategy.Fail,
            Items = new List<ProductVariantBatchItem>
            {
                CreateValidBatchItem("SKU-001", productMasterId),
                CreateValidBatchItem("SKU-002", productMasterId) // This will have duplicate combination
            }
        };

        SetupConflictScenario(productMasterId, shouldFail: true);

        // Act
        var result = await _service.CreateVariantsBatchAsync(request);

        // Assert
        result.TotalProcessed.Should().Be(2);
        result.SuccessCount.Should().Be(1);
        result.FailureCount.Should().Be(1);
        
        var failedResult = result.Results.Skip(1).First();
        failedResult.Success.Should().BeFalse();
        failedResult.Errors.Should().Contain("Variant combination already exists");
    }

    [Fact]
    public async Task CreateVariantsBatchAsync_WithNonExistentProductMaster_ShouldFail()
    {
        // Arrange
        var idempotencyKey = "missing-product-001";
        var nonExistentProductId = Guid.NewGuid();
        var request = new BatchCreateVariantRequest
        {
            IdempotencyKey = idempotencyKey,
            OnConflict = ConflictResolutionStrategy.Fail,
            Items = new List<ProductVariantBatchItem>
            {
                CreateValidBatchItem("SKU-001", nonExistentProductId)
            }
        };

        _mockBatchOperationService.Setup(x => x.GetExistingOperationAsync<ProductVariant>(idempotencyKey))
            .ReturnsAsync((BatchOperationResult<ProductVariant>?)null);
        
        _mockProductRepository.Setup(x => x.GetByIdAsync(nonExistentProductId))
            .ReturnsAsync((ProductMaster?)null);

        // Act
        var result = await _service.CreateVariantsBatchAsync(request);

        // Assert
        result.TotalProcessed.Should().Be(1);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(1);
        
        var failedResult = result.Results.First();
        failedResult.Success.Should().BeFalse();
        failedResult.Errors.Should().Contain("Product master not found");
    }

    #endregion

    #region Helper Methods

    private ProductVariantBatchItem CreateValidBatchItem(string sku, Guid? productMasterId = null)
    {
        return new ProductVariantBatchItem
        {
            ProductMasterId = productMasterId ?? Guid.NewGuid(),
            Price = 29.99m,
            SKU = sku,
            OptionValues = new List<VariantOptionValueBatchItem>
            {
                new VariantOptionValueBatchItem { VariantOptionId = Guid.NewGuid(), Value = "Red" },
                new VariantOptionValueBatchItem { VariantOptionId = Guid.NewGuid(), Value = "Large" }
            }
        };
    }

    private void SetupSuccessfulBatchCreation(Guid productMasterId)
    {
        _mockBatchOperationService.Setup(x => x.GetExistingOperationAsync<ProductVariant>(It.IsAny<string>()))
            .ReturnsAsync((BatchOperationResult<ProductVariant>?)null);

        _mockProductRepository.Setup(x => x.GetByIdAsync(productMasterId))
            .ReturnsAsync(new ProductMaster { Id = productMasterId });

        _mockProductRepository.Setup(x => x.VariantCombinationExistsAsync(productMasterId, It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(false);

        _mockValidator.Setup(x => x.ValidateProductVariantAsync(It.IsAny<ProductVariant>(), false))
            .Returns(Task.CompletedTask);

        _mockProductRepository.Setup(x => x.AddVariantAsync(It.IsAny<ProductVariant>()))
            .Returns<ProductVariant>(v => Task.FromResult(v));

        _mockSellableItemService.Setup(x => x.CreateSellableItemAsync(It.IsAny<SellableItemType>(), It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(new SellableItem { Id = Guid.NewGuid() });

        _mockBatchOperationService.Setup(x => x.SaveOperationResultAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BatchOperationResult<ProductVariant>>()))
            .Returns<string, string, BatchOperationResult<ProductVariant>>((key, op, result) => Task.FromResult(result));
    }

    private void SetupConflictScenario(Guid productMasterId, bool shouldFail = false)
    {
        _mockBatchOperationService.Setup(x => x.GetExistingOperationAsync<ProductVariant>(It.IsAny<string>()))
            .ReturnsAsync((BatchOperationResult<ProductVariant>?)null);

        _mockProductRepository.Setup(x => x.GetByIdAsync(productMasterId))
            .ReturnsAsync(new ProductMaster { Id = productMasterId });

        var callCount = 0;
        _mockProductRepository.Setup(x => x.VariantCombinationExistsAsync(productMasterId, It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(() => callCount++ > 0); // First call returns false, subsequent calls return true

        _mockValidator.Setup(x => x.ValidateProductVariantAsync(It.IsAny<ProductVariant>(), false))
            .Returns(Task.CompletedTask);

        _mockProductRepository.Setup(x => x.AddVariantAsync(It.IsAny<ProductVariant>()))
            .Returns<ProductVariant>(v => Task.FromResult(v));

        _mockSellableItemService.Setup(x => x.CreateSellableItemAsync(It.IsAny<SellableItemType>(), It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync(new SellableItem { Id = Guid.NewGuid() });

        _mockBatchOperationService.Setup(x => x.SaveOperationResultAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BatchOperationResult<ProductVariant>>()))
            .Returns<string, string, BatchOperationResult<ProductVariant>>((key, op, result) => Task.FromResult(result));
    }

    #endregion
}