using FluentAssertions;
using Moq;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Validators;
using System.Text.Json;
using Xunit;

namespace ProductVariantBundle.Tests.Core.Validators;

public class ProductValidatorTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly ProductValidator _validator;

    public ProductValidatorTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _validator = new ProductValidator(_mockProductRepository.Object);
    }

    #region ProductMaster Validation Tests

    [Fact]
    public async Task ValidateProductMasterAsync_WithValidData_ShouldNotThrow()
    {
        // Arrange
        var productMaster = new ProductMaster
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Slug = "test-product",
            Description = "Test Description",
            Category = "Electronics"
        };

        _mockProductRepository.Setup(x => x.SlugExistsAsync("test-product", It.IsAny<Guid?>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _validator.Invoking(v => v.ValidateProductMasterAsync(productMaster))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateProductMasterAsync_WithEmptyName_ShouldThrowValidationException()
    {
        // Arrange
        var productMaster = new ProductMaster
        {
            Id = Guid.NewGuid(),
            Name = "",
            Slug = "test-product"
        };

        // Act & Assert
        var exception = await _validator.Invoking(v => v.ValidateProductMasterAsync(productMaster))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("Name");
        exception.Which.Errors["Name"].Should().Contain("Name is required");
    }

    [Fact]
    public async Task ValidateProductMasterAsync_WithEmptySlug_ShouldThrowValidationException()
    {
        // Arrange
        var productMaster = new ProductMaster
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Slug = ""
        };

        // Act & Assert
        var exception = await _validator.Invoking(v => v.ValidateProductMasterAsync(productMaster))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("Slug");
        exception.Which.Errors["Slug"].Should().Contain("Slug is required");
    }

    [Fact]
    public async Task ValidateProductMasterAsync_WithDuplicateSlug_ShouldThrowValidationException()
    {
        // Arrange
        var productMaster = new ProductMaster
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Slug = "existing-slug"
        };

        _mockProductRepository.Setup(x => x.SlugExistsAsync("existing-slug", It.IsAny<Guid?>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await _validator.Invoking(v => v.ValidateProductMasterAsync(productMaster))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("Slug");
        exception.Which.Errors["Slug"].Should().Contain("Slug 'existing-slug' already exists");
    }

    [Fact]
    public async Task ValidateProductMasterAsync_WithSlugNormalization_ShouldNormalizeSlug()
    {
        // Arrange
        var productMaster = new ProductMaster
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Slug = "Test Product Slug"
        };

        _mockProductRepository.Setup(x => x.SlugExistsAsync("test-product-slug", It.IsAny<Guid?>()))
            .ReturnsAsync(false);

        // Act
        await _validator.ValidateProductMasterAsync(productMaster);

        // Assert
        productMaster.Slug.Should().Be("test-product-slug");
    }

    [Fact]
    public async Task ValidateProductMasterAsync_ForUpdate_ShouldExcludeCurrentId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productMaster = new ProductMaster
        {
            Id = productId,
            Name = "Test Product",
            Slug = "test-slug"
        };

        _mockProductRepository.Setup(x => x.SlugExistsAsync("test-slug", productId))
            .ReturnsAsync(false);

        // Act & Assert
        await _validator.Invoking(v => v.ValidateProductMasterAsync(productMaster, true))
            .Should().NotThrowAsync();

        _mockProductRepository.Verify(x => x.SlugExistsAsync("test-slug", productId), Times.Once);
    }

    #endregion

    #region ProductVariant Validation Tests

    [Fact]
    public async Task ValidateProductVariantAsync_WithValidData_ShouldNotThrow()
    {
        // Arrange
        var productMasterId = Guid.NewGuid();
        var variant = CreateValidVariant(productMasterId);

        _mockProductRepository.Setup(x => x.GetByIdAsync(productMasterId))
            .ReturnsAsync(new ProductMaster { Id = productMasterId });
        _mockProductRepository.Setup(x => x.VariantCombinationExistsAsync(productMasterId, It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _validator.Invoking(v => v.ValidateProductVariantAsync(variant))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateProductVariantAsync_WithNegativePrice_ShouldThrowValidationException()
    {
        // Arrange
        var productMasterId = Guid.NewGuid();
        var variant = CreateValidVariant(productMasterId);
        variant.Price = -10.50m;

        _mockProductRepository.Setup(x => x.GetByIdAsync(productMasterId))
            .ReturnsAsync(new ProductMaster { Id = productMasterId });

        // Act & Assert
        var exception = await _validator.Invoking(v => v.ValidateProductVariantAsync(variant))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("Price");
        exception.Which.Errors["Price"].Should().Contain("Price cannot be negative");
    }

    [Fact]
    public async Task ValidateProductVariantAsync_WithNonExistentProductMaster_ShouldThrowValidationException()
    {
        // Arrange
        var productMasterId = Guid.NewGuid();
        var variant = CreateValidVariant(productMasterId);

        _mockProductRepository.Setup(x => x.GetByIdAsync(productMasterId))
            .ReturnsAsync((ProductMaster?)null);

        // Act & Assert
        var exception = await _validator.Invoking(v => v.ValidateProductVariantAsync(variant))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("ProductMasterId");
        exception.Which.Errors["ProductMasterId"].Should().Contain("Product master not found");
    }

    [Fact]
    public async Task ValidateProductVariantAsync_WithNoOptionValues_ShouldThrowValidationException()
    {
        // Arrange
        var productMasterId = Guid.NewGuid();
        var variant = CreateValidVariant(productMasterId);
        variant.OptionValues = new List<VariantOptionValue>();

        _mockProductRepository.Setup(x => x.GetByIdAsync(productMasterId))
            .ReturnsAsync(new ProductMaster { Id = productMasterId });

        // Act & Assert
        var exception = await _validator.Invoking(v => v.ValidateProductVariantAsync(variant))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("OptionValues");
        exception.Which.Errors["OptionValues"].Should().Contain("Variant must have at least one option value");
    }

    [Fact]
    public async Task ValidateProductVariantAsync_WithDuplicateCombination_ShouldThrowValidationException()
    {
        // Arrange
        var productMasterId = Guid.NewGuid();
        var variant = CreateValidVariant(productMasterId);

        _mockProductRepository.Setup(x => x.GetByIdAsync(productMasterId))
            .ReturnsAsync(new ProductMaster { Id = productMasterId });
        _mockProductRepository.Setup(x => x.VariantCombinationExistsAsync(productMasterId, It.IsAny<string>(), It.IsAny<Guid?>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await _validator.Invoking(v => v.ValidateProductVariantAsync(variant))
            .Should().ThrowAsync<ValidationException>();

        exception.Which.Errors.Should().ContainKey("OptionValues");
        exception.Which.Errors["OptionValues"].Should().Contain("This combination of option values already exists for this product");
    }

    #endregion

    #region Combination Key Generation Tests

    [Fact]
    public void GenerateVariantCombinationKey_WithSingleOption_ShouldGenerateCorrectKey()
    {
        // Arrange
        var optionValues = new List<VariantOptionValue>
        {
            new VariantOptionValue
            {
                Value = "Red",
                VariantOption = new VariantOption { Slug = "color" }
            }
        };

        // Act
        var result = ProductValidator.GenerateVariantCombinationKey(optionValues);

        // Assert
        result.Should().Be("color:red");
    }

    [Fact]
    public void GenerateVariantCombinationKey_WithMultipleOptions_ShouldSortBySlug()
    {
        // Arrange
        var optionValues = new List<VariantOptionValue>
        {
            new VariantOptionValue
            {
                Value = "Large",
                VariantOption = new VariantOption { Slug = "size" }
            },
            new VariantOptionValue
            {
                Value = "Red",
                VariantOption = new VariantOption { Slug = "color" }
            }
        };

        // Act
        var result = ProductValidator.GenerateVariantCombinationKey(optionValues);

        // Assert
        result.Should().Be("color:red|size:large");
    }

    [Fact]
    public void GenerateVariantCombinationKey_WithDifferentOrder_ShouldProduceSameKey()
    {
        // Arrange
        var optionValues1 = new List<VariantOptionValue>
        {
            new VariantOptionValue { Value = "Red", VariantOption = new VariantOption { Slug = "color" } },
            new VariantOptionValue { Value = "Large", VariantOption = new VariantOption { Slug = "size" } }
        };

        var optionValues2 = new List<VariantOptionValue>
        {
            new VariantOptionValue { Value = "Large", VariantOption = new VariantOption { Slug = "size" } },
            new VariantOptionValue { Value = "Red", VariantOption = new VariantOption { Slug = "color" } }
        };

        // Act
        var key1 = ProductValidator.GenerateVariantCombinationKey(optionValues1);
        var key2 = ProductValidator.GenerateVariantCombinationKey(optionValues2);

        // Assert
        key1.Should().Be(key2);
        key1.Should().Be("color:red|size:large");
    }

    [Fact]
    public void GenerateVariantCombinationKey_WithWhitespaceInValues_ShouldNormalizeValues()
    {
        // Arrange
        var optionValues = new List<VariantOptionValue>
        {
            new VariantOptionValue
            {
                Value = "  Red  ",
                VariantOption = new VariantOption { Slug = "color" }
            },
            new VariantOptionValue
            {
                Value = "LARGE",
                VariantOption = new VariantOption { Slug = "size" }
            }
        };

        // Act
        var result = ProductValidator.GenerateVariantCombinationKey(optionValues);

        // Assert
        result.Should().Be("color:red|size:large");
    }

    [Fact]
    public void GenerateVariantCombinationKey_WithEmptyCollection_ShouldReturnEmptyString()
    {
        // Arrange
        var optionValues = new List<VariantOptionValue>();

        // Act
        var result = ProductValidator.GenerateVariantCombinationKey(optionValues);

        // Assert
        result.Should().Be(string.Empty);
    }

    #endregion

    #region Helper Methods

    private ProductVariant CreateValidVariant(Guid productMasterId)
    {
        return new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductMasterId = productMasterId,
            Price = 29.99m,
            OptionValues = new List<VariantOptionValue>
            {
                new VariantOptionValue
                {
                    Value = "Red",
                    VariantOption = new VariantOption { Slug = "color" }
                },
                new VariantOptionValue
                {
                    Value = "Large",
                    VariantOption = new VariantOption { Slug = "size" }
                }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = EntityStatus.Active
        };
    }

    #endregion
}