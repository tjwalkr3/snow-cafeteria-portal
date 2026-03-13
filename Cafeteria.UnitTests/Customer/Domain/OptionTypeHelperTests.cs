using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.UnitTests.Customer.Domain;

public class OptionTypeHelperTests
{
    private static FoodOptionTypeWithOptionsDto CreateOptionType(short maxAmount = 1, string typeName = "Cheese")
    {
        return new FoodOptionTypeWithOptionsDto
        {
            OptionType = new FoodOptionTypeDto
            {
                Id = 1,
                FoodOptionTypeName = typeName,
                MaxAmount = maxAmount,
                NumIncluded = 1
            },
            Options = new List<FoodOptionDto>()
        };
    }

    [Fact]
    public void IsMultiSelectOptionType_WithMaxAmountGreaterThanOne_ReturnsTrue()
    {
        // Arrange
        var optionType = CreateOptionType(maxAmount: 3, typeName: "Cheese");

        // Act
        var result = OptionTypeHelper.IsMultiSelectOptionType(optionType);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMultiSelectOptionType_WithToppingsType_ReturnsTrue()
    {
        // Arrange
        var optionType = CreateOptionType(maxAmount: 1, typeName: "Toppings");

        // Act
        var result = OptionTypeHelper.IsMultiSelectOptionType(optionType);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMultiSelectOptionType_WithFillingsType_ReturnsTrue()
    {
        // Arrange
        var optionType = CreateOptionType(maxAmount: 1, typeName: "Fillings");

        // Act
        var result = OptionTypeHelper.IsMultiSelectOptionType(optionType);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMultiSelectOptionType_WithSingleSelectAndRegularType_ReturnsFalse()
    {
        // Arrange
        var optionType = CreateOptionType(maxAmount: 1, typeName: "Cheese");

        // Act
        var result = OptionTypeHelper.IsMultiSelectOptionType(optionType);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsMultiSelectOptionType_WithSingleSelectAndDifferentType_ReturnsFalse()
    {
        // Arrange
        var optionType = CreateOptionType(maxAmount: 1, typeName: "Size");

        // Act
        var result = OptionTypeHelper.IsMultiSelectOptionType(optionType);

        // Assert
        Assert.False(result);
    }
}
