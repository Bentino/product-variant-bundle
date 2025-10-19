using FluentAssertions;
using Moq;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Core.Services;
using System.Text.Json;
using Xunit;

namespace ProductVariantBundle.Tests.Core.Services;

public class BatchOperationServiceTests
{
    private readonly Mock<IBatchOperationRepository> _mockBatchOperationRepository;
    private readonly BatchOperationService _service;

    public BatchOperationServiceTests()
    {
        _mockBatchOperationRepository = new Mock<IBatchOperationRepository>();
        _service = new BatchOperationService(_mockBatchOperationRepository.Object);
    }

    #region Idempotency Tests

    [Fact]
    public async Task GetExistingOperationAsync_WithValidKey_ShouldReturnExistingResult()
    {
        // Arrange
        var idempotencyKey = "test-key-001";
        var originalResult = new BatchOperationResult<ProductVariant>
        {
            IdempotencyKey = idempotencyKey,
            TotalProcessed = 3,
            SuccessCount = 2,
            FailureCount = 1,
            OnConflict = ConflictResolutionStrategy.Fail,
            Results = new List<BatchItemResult<ProductVariant>>
            {
                new BatchItemResult<ProductVariant> { Index = 0, Success = true },
                new BatchItemResult<ProductVariant> { Index = 1, Success = true },
                new BatchItemResult<ProductVariant> { Index = 2, Success = false, Errors = new[] { "Validation error" } }
            }
        };

        var batchOperation = new BatchOperation
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = idempotencyKey,
            OperationType = "CreateVariants",
            TotalItems = 3,
            SuccessCount = 2,
            FailureCount = 1,
            ResultData = JsonDocument.Parse(JsonSerializer.Serialize(originalResult)),
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            Status = EntityStatus.Active
        };

        _mockBatchOperationRepository.Setup(x => x.GetByIdempotencyKeyAsync(idempotencyKey))
            .ReturnsAsync(batchOperation);

        // Act
        var result = await _service.GetExistingOperationAsync<ProductVariant>(idempotencyKey);

        // Assert
        result.Should().NotBeNull();
        result!.IdempotencyKey.Should().Be(idempotencyKey);
        result.TotalProcessed.Should().Be(3);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(1);
        result.Results.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetExistingOperationAsync_WithNonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var idempotencyKey = "non-existent-key";
        _mockBatchOperationRepository.Setup(x => x.GetByIdempotencyKeyAsync(idempotencyKey))
            .ReturnsAsync((BatchOperation?)null);

        // Act
        var result = await _service.GetExistingOperationAsync<ProductVariant>(idempotencyKey);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetExistingOperationAsync_WithExpiredOperation_ShouldReturnNull()
    {
        // Arrange
        var idempotencyKey = "expired-key";
        var expiredOperation = new BatchOperation
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = idempotencyKey,
            ExpiresAt = DateTime.UtcNow.AddHours(-1), // Expired 1 hour ago
            Status = EntityStatus.Active
        };

        _mockBatchOperationRepository.Setup(x => x.GetByIdempotencyKeyAsync(idempotencyKey))
            .ReturnsAsync(expiredOperation);

        // Act
        var result = await _service.GetExistingOperationAsync<ProductVariant>(idempotencyKey);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Save Operation Tests

    [Fact]
    public async Task SaveOperationResultAsync_WithValidData_ShouldSaveAndReturnResult()
    {
        // Arrange
        var idempotencyKey = "save-test-key";
        var operationType = "CreateVariants";
        var batchResult = new BatchOperationResult<ProductVariant>
        {
            IdempotencyKey = idempotencyKey,
            TotalProcessed = 2,
            SuccessCount = 2,
            FailureCount = 0,
            OnConflict = ConflictResolutionStrategy.Fail,
            Results = new List<BatchItemResult<ProductVariant>>
            {
                new BatchItemResult<ProductVariant> { Index = 0, Success = true },
                new BatchItemResult<ProductVariant> { Index = 1, Success = true }
            }
        };

        BatchOperation? savedOperation = null;
        _mockBatchOperationRepository.Setup(x => x.AddAsync(It.IsAny<BatchOperation>()))
            .Callback<BatchOperation>(op => savedOperation = op)
            .ReturnsAsync((BatchOperation op) => op);

        // Act
        var result = await _service.SaveOperationResultAsync(idempotencyKey, operationType, batchResult);

        // Assert
        result.Should().Be(batchResult);
        savedOperation.Should().NotBeNull();
        savedOperation!.IdempotencyKey.Should().Be(idempotencyKey);
        savedOperation.OperationType.Should().Be(operationType);
        savedOperation.TotalItems.Should().Be(2);
        savedOperation.SuccessCount.Should().Be(2);
        savedOperation.FailureCount.Should().Be(0);
        savedOperation.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task SaveOperationResultAsync_ShouldSetCorrectExpiryTime()
    {
        // Arrange
        var idempotencyKey = "expiry-test-key";
        var operationType = "CreateBundles";
        var batchResult = new BatchOperationResult<ProductBundle>
        {
            IdempotencyKey = idempotencyKey,
            TotalProcessed = 1,
            SuccessCount = 1,
            FailureCount = 0
        };

        BatchOperation? savedOperation = null;
        _mockBatchOperationRepository.Setup(x => x.AddAsync(It.IsAny<BatchOperation>()))
            .Callback<BatchOperation>(op => savedOperation = op)
            .ReturnsAsync((BatchOperation op) => op);

        var beforeSave = DateTime.UtcNow;

        // Act
        await _service.SaveOperationResultAsync(idempotencyKey, operationType, batchResult);

        // Assert
        savedOperation.Should().NotBeNull();
        savedOperation!.ExpiresAt.Should().BeAfter(beforeSave.AddHours(23)); // Should be ~24 hours from now
        savedOperation.ExpiresAt.Should().BeBefore(beforeSave.AddHours(25));
    }

    #endregion

    #region Create Result Tests

    [Fact]
    public void CreateResult_WithMixedResults_ShouldCalculateCountsCorrectly()
    {
        // Arrange
        var idempotencyKey = "create-result-test";
        var onConflict = ConflictResolutionStrategy.Skip;
        var results = new List<BatchItemResult<ProductVariant>>
        {
            new BatchItemResult<ProductVariant> { Index = 0, Success = true },
            new BatchItemResult<ProductVariant> { Index = 1, Success = false, Errors = new[] { "Error 1" } },
            new BatchItemResult<ProductVariant> { Index = 2, Success = true },
            new BatchItemResult<ProductVariant> { Index = 3, Success = false, Errors = new[] { "Error 2" } }
        };

        // Act
        var result = BatchOperationService.CreateResult(idempotencyKey, onConflict, results);

        // Assert
        result.IdempotencyKey.Should().Be(idempotencyKey);
        result.OnConflict.Should().Be(onConflict);
        result.TotalProcessed.Should().Be(4);
        result.SuccessCount.Should().Be(2);
        result.FailureCount.Should().Be(2);
        result.Results.Should().HaveCount(4);
    }

    [Fact]
    public void CreateResult_WithAllSuccessful_ShouldHaveZeroFailures()
    {
        // Arrange
        var idempotencyKey = "all-success-test";
        var results = new List<BatchItemResult<ProductBundle>>
        {
            new BatchItemResult<ProductBundle> { Index = 0, Success = true },
            new BatchItemResult<ProductBundle> { Index = 1, Success = true },
            new BatchItemResult<ProductBundle> { Index = 2, Success = true }
        };

        // Act
        var result = BatchOperationService.CreateResult(idempotencyKey, ConflictResolutionStrategy.Fail, results);

        // Assert
        result.TotalProcessed.Should().Be(3);
        result.SuccessCount.Should().Be(3);
        result.FailureCount.Should().Be(0);
    }

    [Fact]
    public void CreateResult_WithAllFailures_ShouldHaveZeroSuccesses()
    {
        // Arrange
        var idempotencyKey = "all-failure-test";
        var results = new List<BatchItemResult<ProductVariant>>
        {
            new BatchItemResult<ProductVariant> { Index = 0, Success = false, Errors = new[] { "Error 1" } },
            new BatchItemResult<ProductVariant> { Index = 1, Success = false, Errors = new[] { "Error 2" } }
        };

        // Act
        var result = BatchOperationService.CreateResult(idempotencyKey, ConflictResolutionStrategy.Update, results);

        // Assert
        result.TotalProcessed.Should().Be(2);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(2);
    }

    [Fact]
    public void CreateResult_WithEmptyResults_ShouldHaveZeroCounts()
    {
        // Arrange
        var idempotencyKey = "empty-test";
        var results = new List<BatchItemResult<ProductVariant>>();

        // Act
        var result = BatchOperationService.CreateResult(idempotencyKey, ConflictResolutionStrategy.Skip, results);

        // Assert
        result.TotalProcessed.Should().Be(0);
        result.SuccessCount.Should().Be(0);
        result.FailureCount.Should().Be(0);
        result.Results.Should().BeEmpty();
    }

    #endregion

    #region Cleanup Tests

    [Fact]
    public async Task CleanupExpiredOperationsAsync_ShouldCallRepositoryWithCurrentTime()
    {
        // Arrange
        var beforeCleanup = DateTime.UtcNow;

        // Act
        await _service.CleanupExpiredOperationsAsync();

        // Assert
        _mockBatchOperationRepository.Verify(
            x => x.DeleteExpiredAsync(It.Is<DateTime>(dt => dt >= beforeCleanup && dt <= DateTime.UtcNow)),
            Times.Once);
    }

    #endregion
}