using FluentAssertions;
using Moq;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Services;
using ProductVariantBundle.Core.Validators;
using Xunit;

namespace ProductVariantBundle.Tests.Core.Services;

public class SellableItemServiceTests
{
    private readonly Mock<ISellableItemRepository> _mockSellableItemRepository;
    private readonly Mock<SkuValidator> _mockSkuValidator;
    private readonly SellableItemService _service;

    public SellableItemServiceTests()
    {
        _mockSellableItemRepository = new Mock<ISellableItemRepository>();
        _mockSkuValidator = new Mock<SkuValidator>(_mockSellableItemRepository.Object);
        _service = new SellableItemService(_mockSellableItemRepository.Object, _mockSkuValidator.Object);
    }

    #region SKU Uniqueness Tests

    [Fact]
    public async Task ValidateSkuUniquenessAsync_WithUniqueSku_ShouldReturnTrue()
    {
        // Arrange
        var sku = "UNIQUE-SKU-001";
        _mockSellableItemRepository.Setup(x => x.SKUExistsAsync(sku, null))
            .ReturnsAsync(false);

        // Act
        var result = await _service.ValidateSkuUniquenessAsync(sku);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateSkuUniquenessAsync_WithExistingSku_ShouldReturnFalse()
    {
        // Arrange
        var sku = "EXISTING-SKU-001";
        _mockSellableItemRepository.Setup(x => x.SKUExistsAsync(sku, null))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ValidateSkuUniquenessAsync(sku);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateSkuUniquenessAsync_WithExistingSkuButExcludedId_ShouldReturnTrue()
    {
        // Arrange
        var sku = "EXISTING-SKU-001";
        var existingId = Guid.NewGuid();
        _mockSellableItemRepository.Setup(x => x.SKUExistsAsync(sku, existingId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.ValidateSkuUniquenessAsync(sku, existingId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateSkuUniquenessAsync_WithExistingSkuAndDifferentExcludedId_ShouldReturnFalse()
    {
        // Arrange
        var sku = "EXISTING-SKU-001";
        var differentId = Guid.NewGuid();
        _mockSellableItemRepository.Setup(x => x.SKUExistsAsync(sku, differentId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ValidateSkuUniquenessAsync(sku, differentId);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Create SellableItem Tests

    [Fact]
    public async Task CreateSellableItemAsync_WithValidData_ShouldCreateVariantSellableItem()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var sku = "VARIANT-SKU-001";
        var expectedSellableItem = new SellableItem
        {
            Id = Guid.NewGuid(),
            SKU = sku,
            Type = SellableItemType.Variant,
            VariantId = entityId,
            Status = EntityStatus.Active
        };

        _mockSkuValidator.Setup(x => x.ValidateSkuAsync(sku, null))
            .Returns(Task.CompletedTask);
        _mockSellableItemRepository.Setup(x => x.AddAsync(It.IsAny<SellableItem>()))
            .ReturnsAsync(expectedSellableItem);

        // Act
        var result = await _service.CreateSellableItemAsync(SellableItemType.Variant, entityId, sku);

        // Assert
        result.Should().NotBeNull();
        result.SKU.Should().Be(sku);
        result.Type.Should().Be(SellableItemType.Variant);
        result.VariantId.Should().Be(entityId);
        result.BundleId.Should().BeNull();
    }

    [Fact]
    public async Task CreateSellableItemAsync_WithValidData_ShouldCreateBundleSellableItem()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var sku = "BUNDLE-SKU-001";
        var expectedSellableItem = new SellableItem
        {
            Id = Guid.NewGuid(),
            SKU = sku,
            Type = SellableItemType.Bundle,
            BundleId = entityId,
            Status = EntityStatus.Active
        };

        _mockSkuValidator.Setup(x => x.ValidateSkuAsync(sku, null))
            .Returns(Task.CompletedTask);
        _mockSellableItemRepository.Setup(x => x.AddAsync(It.IsAny<SellableItem>()))
            .ReturnsAsync(expectedSellableItem);

        // Act
        var result = await _service.CreateSellableItemAsync(SellableItemType.Bundle, entityId, sku);

        // Assert
        result.Should().NotBeNull();
        result.SKU.Should().Be(sku);
        result.Type.Should().Be(SellableItemType.Bundle);
        result.BundleId.Should().Be(entityId);
        result.VariantId.Should().BeNull();
    }

    [Fact]
    public async Task CreateSellableItemAsync_WithDuplicateSku_ShouldThrowValidationException()
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var sku = "DUPLICATE-SKU-001";
        var normalizedSku = "DUPLICATE-SKU-001";

        var validationErrors = new Dictionary<string, string[]>
        {
            { "SKU", new[] { $"SKU '{normalizedSku}' already exists" } }
        };

        _mockSkuValidator.Setup(x => x.ValidateSkuAsync(normalizedSku, null))
            .ThrowsAsync(new ValidationException(validationErrors));

        // Act & Assert
        var exception = await _service.Invoking(s => s.CreateSellableItemAsync(SellableItemType.Variant, entityId, sku))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("SKU");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateSellableItemAsync_WithInvalidSku_ShouldThrowValidationException(string? invalidSku)
    {
        // Arrange
        var entityId = Guid.NewGuid();
        var normalizedSku = string.Empty;

        var validationErrors = new Dictionary<string, string[]>
        {
            { "SKU", new[] { "SKU is required" } }
        };

        _mockSkuValidator.Setup(x => x.ValidateSkuAsync(normalizedSku, null))
            .ThrowsAsync(new ValidationException(validationErrors));

        // Act & Assert
        var exception = await _service.Invoking(s => s.CreateSellableItemAsync(SellableItemType.Variant, entityId, invalidSku!))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("SKU");
    }

    #endregion

    #region Get By SKU Tests

    [Fact]
    public async Task GetBySKUAsync_WithExistingSku_ShouldReturnSellableItem()
    {
        // Arrange
        var sku = "EXISTING-SKU-001";
        var expectedSellableItem = new SellableItem
        {
            Id = Guid.NewGuid(),
            SKU = sku,
            Type = SellableItemType.Variant,
            Status = EntityStatus.Active
        };

        _mockSellableItemRepository.Setup(x => x.GetBySKUAsync(sku))
            .ReturnsAsync(expectedSellableItem);

        // Act
        var result = await _service.GetBySKUAsync(sku);

        // Assert
        result.Should().NotBeNull();
        result.SKU.Should().Be(sku);
        result.Id.Should().Be(expectedSellableItem.Id);
    }

    [Fact]
    public async Task GetBySKUAsync_WithNonExistentSku_ShouldReturnNull()
    {
        // Arrange
        var sku = "NON-EXISTENT-SKU";
        _mockSellableItemRepository.Setup(x => x.GetBySKUAsync(sku))
            .ReturnsAsync((SellableItem?)null);

        // Act
        var result = await _service.GetBySKUAsync(sku);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}