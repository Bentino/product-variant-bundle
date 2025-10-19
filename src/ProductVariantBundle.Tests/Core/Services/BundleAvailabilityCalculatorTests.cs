using FluentAssertions;
using Moq;
using ProductVariantBundle.Core.Entities;
using ProductVariantBundle.Core.Enums;
using ProductVariantBundle.Core.Exceptions;
using ProductVariantBundle.Core.Interfaces;
using ProductVariantBundle.Core.Models;
using ProductVariantBundle.Core.Services;
using Xunit;

namespace ProductVariantBundle.Tests.Core.Services;

public class BundleAvailabilityCalculatorTests
{
    private readonly Mock<IInventoryRepository> _mockInventoryRepository;
    private readonly Mock<IWarehouseService> _mockWarehouseService;
    private readonly BundleAvailabilityCalculator _calculator;
    private readonly Warehouse _defaultWarehouse;

    public BundleAvailabilityCalculatorTests()
    {
        _mockInventoryRepository = new Mock<IInventoryRepository>();
        _mockWarehouseService = new Mock<IWarehouseService>();
        _calculator = new BundleAvailabilityCalculator(_mockInventoryRepository.Object, _mockWarehouseService.Object);
        
        _defaultWarehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Code = "MAIN",
            Name = "Main Warehouse",
            Status = EntityStatus.Active
        };

        _mockWarehouseService.Setup(x => x.GetByCodeAsync("MAIN"))
            .ReturnsAsync(_defaultWarehouse);
    }

    #region Basic Availability Calculation Tests

    [Fact]
    public async Task CalculateAvailabilityAsync_WithEmptyBundle_ShouldReturnZeroAvailability()
    {
        // Arrange
        var bundle = new ProductBundle
        {
            Id = Guid.NewGuid(),
            Items = new List<BundleItem>()
        };

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.Should().NotBeNull();
        result.BundleId.Should().Be(bundle.Id);
        result.AvailableQuantity.Should().Be(0);
        result.IsAvailable.Should().BeFalse();
        result.Components.Should().BeEmpty();
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithNullBundle_ShouldReturnZeroAvailability()
    {
        // Act
        var result = await _calculator.CalculateAvailabilityAsync(null!);

        // Assert
        result.Should().NotBeNull();
        result.BundleId.Should().Be(Guid.Empty);
        result.AvailableQuantity.Should().Be(0);
        result.IsAvailable.Should().BeFalse();
        result.Components.Should().BeEmpty();
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithSingleComponent_ShouldCalculateCorrectly()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 1)
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 2); // Available: 8

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(8); // floor(8/1) = 8
        result.IsAvailable.Should().BeTrue();
        result.Components.Should().HaveCount(1);
        result.Components.First().CanFulfill.Should().Be(8);
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithMultipleComponents_ShouldReturnMinimum()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 1),    // Available: 8, Can fulfill: 8
            ("KEYBOARD-001", 1)  // Available: 4, Can fulfill: 4
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 2);    // Available: 8
        SetupInventoryRecord("KEYBOARD-001", onHand: 5, reserved: 1);  // Available: 4

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(4); // Min of (8, 4)
        result.IsAvailable.Should().BeTrue();
        result.Components.Should().HaveCount(2);
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithHigherQuantityRequirement_ShouldCalculateCorrectly()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 3),    // Available: 8, Can fulfill: floor(8/3) = 2
            ("KEYBOARD-001", 2)  // Available: 6, Can fulfill: floor(6/2) = 3
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 2);    // Available: 8
        SetupInventoryRecord("KEYBOARD-001", onHand: 8, reserved: 2);  // Available: 6

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(2); // Min of (floor(8/3), floor(6/2)) = Min(2, 3) = 2
        result.IsAvailable.Should().BeTrue();
    }

    #endregion

    #region Out of Stock Scenarios

    [Fact]
    public async Task CalculateAvailabilityAsync_WithZeroStock_ShouldReturnZeroAvailability()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 1),
            ("KEYBOARD-001", 1)
        });

        SetupInventoryRecord("MOUSE-001", onHand: 0, reserved: 0);     // Available: 0
        SetupInventoryRecord("KEYBOARD-001", onHand: 5, reserved: 1);  // Available: 4

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(0);
        result.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithFullyReservedStock_ShouldReturnZeroAvailability()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 1)
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 10); // Available: 0

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(0);
        result.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithOverReservedStock_ShouldReturnZeroAvailability()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 1)
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 15); // Available: -5

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(0);
        result.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithMissingInventoryRecord_ShouldReturnZeroAvailability()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 1),
            ("MISSING-ITEM", 1)
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 2);
        // No setup for MISSING-ITEM - will return null

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(0);
        result.IsAvailable.Should().BeFalse();
        result.Components.Should().HaveCount(1); // Should stop at first missing item
    }

    #endregion

    #region Warehouse-Specific Tests

    [Fact]
    public async Task CalculateAvailabilityAsync_WithSpecificWarehouse_ShouldUseCorrectWarehouse()
    {
        // Arrange
        var warehouseCode = "WEST";
        var westWarehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Code = warehouseCode,
            Name = "West Warehouse",
            Status = EntityStatus.Active
        };

        _mockWarehouseService.Setup(x => x.GetByCodeAsync(warehouseCode))
            .ReturnsAsync(westWarehouse);

        var bundle = CreateTestBundle(new[] { ("MOUSE-001", 1) });
        SetupInventoryRecordForWarehouse("MOUSE-001", westWarehouse.Id, onHand: 5, reserved: 1);

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle, warehouseCode);

        // Assert
        result.AvailableQuantity.Should().Be(4);
        result.WarehouseCode.Should().Be(warehouseCode);
        _mockWarehouseService.Verify(x => x.GetByCodeAsync(warehouseCode), Times.Once);
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithNonExistentWarehouse_ShouldThrowException()
    {
        // Arrange
        var warehouseCode = "NONEXISTENT";
        _mockWarehouseService.Setup(x => x.GetByCodeAsync(warehouseCode))
            .ReturnsAsync((Warehouse?)null);

        var bundle = CreateTestBundle(new[] { ("MOUSE-001", 1) });

        // Act & Assert
        await _calculator.Invoking(c => c.CalculateAvailabilityAsync(bundle, warehouseCode))
            .Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("*Warehouse*");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task CalculateAvailabilityAsync_WithExactStock_ShouldReturnCorrectAvailability()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 5)  // Exactly matches available stock
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 5); // Available: 5

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(1); // floor(5/5) = 1
        result.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithInsufficientStock_ShouldReturnZero()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 6)  // Requires more than available
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 5); // Available: 5

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(0); // floor(5/6) = 0
        result.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task CalculateAvailabilityAsync_WithLargeNumbers_ShouldHandleCorrectly()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 100)
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10000, reserved: 500); // Available: 9500

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.AvailableQuantity.Should().Be(95); // floor(9500/100) = 95
        result.IsAvailable.Should().BeTrue();
    }

    #endregion

    #region Component Availability Details

    [Fact]
    public async Task CalculateAvailabilityAsync_ShouldProvideDetailedComponentInfo()
    {
        // Arrange
        var bundle = CreateTestBundle(new[]
        {
            ("MOUSE-001", 2),
            ("KEYBOARD-001", 3)
        });

        SetupInventoryRecord("MOUSE-001", onHand: 10, reserved: 2);    // Available: 8
        SetupInventoryRecord("KEYBOARD-001", onHand: 15, reserved: 3); // Available: 12

        // Act
        var result = await _calculator.CalculateAvailabilityAsync(bundle);

        // Assert
        result.Components.Should().HaveCount(2);
        
        var mouseComponent = result.Components.First(c => c.SKU == "MOUSE-001");
        mouseComponent.Required.Should().Be(2);
        mouseComponent.Available.Should().Be(8);
        mouseComponent.CanFulfill.Should().Be(4); // floor(8/2)

        var keyboardComponent = result.Components.First(c => c.SKU == "KEYBOARD-001");
        keyboardComponent.Required.Should().Be(3);
        keyboardComponent.Available.Should().Be(12);
        keyboardComponent.CanFulfill.Should().Be(4); // floor(12/3)
    }

    #endregion

    #region Helper Methods

    private ProductBundle CreateTestBundle(IEnumerable<(string sku, int quantity)> items)
    {
        var bundleItems = items.Select(item => new BundleItem
        {
            Id = Guid.NewGuid(),
            SellableItemId = Guid.NewGuid(),
            Quantity = item.quantity,
            SellableItem = new SellableItem
            {
                Id = Guid.NewGuid(),
                SKU = item.sku,
                Type = SellableItemType.Variant,
                Status = EntityStatus.Active
            }
        }).ToList();

        return new ProductBundle
        {
            Id = Guid.NewGuid(),
            Name = "Test Bundle",
            Items = bundleItems,
            Status = EntityStatus.Active
        };
    }

    private void SetupInventoryRecord(string sku, int onHand, int reserved)
    {
        SetupInventoryRecordForWarehouse(sku, _defaultWarehouse.Id, onHand, reserved);
    }

    private void SetupInventoryRecordForWarehouse(string sku, Guid warehouseId, int onHand, int reserved)
    {
        var sellableItemId = Guid.NewGuid();
        var inventoryRecord = new InventoryRecord
        {
            Id = Guid.NewGuid(),
            SellableItemId = sellableItemId,
            WarehouseId = warehouseId,
            OnHand = onHand,
            Reserved = reserved,
            SellableItem = new SellableItem
            {
                Id = sellableItemId,
                SKU = sku,
                Type = SellableItemType.Variant,
                Status = EntityStatus.Active
            }
        };

        _mockInventoryRepository.Setup(x => x.GetBySellableItemAndWarehouseAsync(It.IsAny<Guid>(), warehouseId))
            .Returns<Guid, Guid>((sellableId, warehouseId) =>
            {
                // Match by SKU through the bundle item's sellable item
                return Task.FromResult<InventoryRecord?>(inventoryRecord);
            });

        _mockInventoryRepository.Setup(x => x.GetBySKUAndWarehouseCodeWithLockAsync(sku, It.IsAny<string>()))
            .ReturnsAsync(inventoryRecord);
    }

    #endregion
}