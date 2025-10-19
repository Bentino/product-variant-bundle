using FluentAssertions;
using ProductVariantBundle.Core.Helpers;
using Xunit;

namespace ProductVariantBundle.Tests.Core.Helpers;

public class SlugHelperTests
{
    [Theory]
    [InlineData("Product Name", "product-name")]
    [InlineData("PRODUCT NAME", "product-name")]
    [InlineData("Product  Name", "product-name")]
    [InlineData("Product_Name", "product-name")]
    [InlineData("Product-Name", "product-name")]
    [InlineData("Product   Name   Test", "product-name-test")]
    [InlineData("Product___Name", "product-name")]
    [InlineData("Product---Name", "product-name")]
    [InlineData("  Product Name  ", "product-name")]
    [InlineData("-Product-Name-", "product-name")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData(null, "")]
    public void Normalize_ShouldProduceConsistentFormat(string input, string expected)
    {
        // Act
        var result = SlugHelper.Normalize(input);
        
        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Normalize_WithComplexInput_ShouldHandleAllCases()
    {
        // Arrange
        var input = "  My___AWESOME---Product  Name  ";
        
        // Act
        var result = SlugHelper.Normalize(input);
        
        // Assert
        result.Should().Be("my-awesome-product-name");
    }

    [Fact]
    public void Normalize_WithSpecialCharacters_ShouldOnlyNormalizeSpacesAndUnderscores()
    {
        // Arrange
        var input = "Product@Name#123";
        
        // Act
        var result = SlugHelper.Normalize(input);
        
        // Assert
        result.Should().Be("product@name#123");
    }
}