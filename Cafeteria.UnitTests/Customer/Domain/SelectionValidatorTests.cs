using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.UnitTests.Customer.Domain;

public class SelectionValidatorTests
{
    private static EntreeDto CreateEntree(int id = 1, string name = "Burger")
    {
        return new EntreeDto { Id = id, EntreeName = name, EntreePrice = 8.99m };
    }

    private static SideDto CreateSide(int id = 1, string name = "Fries")
    {
        return new SideDto { Id = id, SideName = name, SidePrice = 2.99m };
    }

    private static DrinkDto CreateDrink(int id = 1, string name = "Coke")
    {
        return new DrinkDto { Id = id, DrinkName = name, DrinkPrice = 1.99m };
    }

    private static FoodOptionDto CreateFoodOption(int id = 1, string name = "Extra Cheese")
    {
        return new FoodOptionDto { Id = id, FoodOptionName = name };
    }

    private static FoodOptionTypeWithOptionsDto CreateOptionType(int id = 1, string name = "Cheese", short numIncluded = 1, int maxAmount = 1, params FoodOptionDto[] options)
    {
        return new FoodOptionTypeWithOptionsDto
        {
            OptionType = new FoodOptionTypeDto
            {
                Id = id,
                FoodOptionTypeName = name,
                NumIncluded = numIncluded,
                MaxAmount = maxAmount
            },
            Options = options.ToList()
        };
    }

    #region IsValid - Card Order Tests

    [Fact]
    public void IsValid_CardOrderWithEntreeComplete_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState { SelectedEntree = CreateEntree() };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_CardOrderWithSideOnly_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState { SelectedSide = CreateSide() };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_CardOrderWithDrinkOnly_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState { SelectedDrink = CreateDrink() };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_CardOrderWithEntreeAndSide_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState
        {
            SelectedEntree = CreateEntree(),
            SelectedSide = CreateSide()
        };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_CardOrderWithNothing_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_CardOrderWithEntreeStartedIncomplete_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState { SelectedEntree = CreateEntree() };
        var option = CreateFoodOption(1, "Extra Cheese");
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            CreateOptionType(1, "Cheese", 1, 1, option)
        };

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: true);

        // Assert
        Assert.False(result); // Entree started but options not complete
    }

    #endregion

    #region IsValid - Normal Order Tests

    [Fact]
    public void IsValid_NormalOrderWithAllItems_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState
        {
            SelectedEntree = CreateEntree(),
            SelectedSide = CreateSide(),
            SelectedDrink = CreateDrink()
        };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: false, requiresOptionsComplete: false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_NormalOrderWithoutEntree_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState
        {
            SelectedSide = CreateSide(),
            SelectedDrink = CreateDrink()
        };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: false, requiresOptionsComplete: false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_NormalOrderWithoutSide_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState
        {
            SelectedEntree = CreateEntree(),
            SelectedDrink = CreateDrink()
        };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: false, requiresOptionsComplete: false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_NormalOrderWithoutDrink_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState
        {
            SelectedEntree = CreateEntree(),
            SelectedSide = CreateSide()
        };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: false, requiresOptionsComplete: false);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsValid - Toppings Tests

    [Fact]
    public void IsValid_WithMinimumToppingsRequired_EnoughToppings_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState();
        state.SelectedToppings.Add("Pepperoni");
        state.SelectedToppings.Add("Mushroom");
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false, minimumToppings: 2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_WithMinimumToppingsRequired_NotEnoughToppings_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        state.SelectedToppings.Add("Pepperoni");
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false, minimumToppings: 2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithMinimumToppingsRequired_NoToppings_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false, minimumToppings: 1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithMinimumToppingsStartedIncomplete_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        state.SelectedToppings.Add("Pepperoni");
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.IsValid(state, optionTypes, isCardOrder: true, requiresOptionsComplete: false, minimumToppings: 3);

        // Assert
        Assert.False(result); // Toppings started but not enough
    }

    #endregion

    #region AreOptionsComplete Tests

    [Fact]
    public void AreOptionsComplete_WithNoOptionTypes_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState();
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreOptionsComplete_WithSingleSelectCompleted_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState();
        state.SingleSelectOptions[1] = "Extra Cheese";
        var optionType = CreateOptionType(1, "Cheese", 1, 1);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreOptionsComplete_WithSingleSelectMissing_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        var optionType = CreateOptionType(1, "Cheese", 1, 1);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreOptionsComplete_WithSingleSelectEmpty_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        state.SingleSelectOptions[1] = "";
        var optionType = CreateOptionType(1, "Cheese", 1, 1);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreOptionsComplete_WithMultiSelectCompleted_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState();
        state.MultiSelectOptions[1] = new List<string> { "Lettuce", "Tomato" };
        var optionType = CreateOptionType(1, "Toppings", 2, 2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreOptionsComplete_WithMultiSelectNotEnough_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        state.MultiSelectOptions[1] = new List<string> { "Lettuce" };
        var optionType = CreateOptionType(1, "Toppings", 2, 2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreOptionsComplete_WithMultiSelectMissing_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        var optionType = CreateOptionType(1, "Toppings", 2, 2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreOptionsComplete_WithMultiSelectEmpty_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        state.MultiSelectOptions[1] = new List<string>();
        var optionType = CreateOptionType(1, "Toppings", 2, 2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreOptionsComplete_WithMultipleSingleSelectOptions_AllCompleted_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState();
        state.SingleSelectOptions[1] = "Extra Cheese";
        state.SingleSelectOptions[2] = "Toast";
        var optionType1 = CreateOptionType(1, "Cheese", 1, 1);
        var optionType2 = CreateOptionType(2, "Bread", 1, 1);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType1, optionType2 };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreOptionsComplete_WithMultipleSingleSelectOptions_OneIncomplete_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        state.SingleSelectOptions[1] = "Extra Cheese";
        // Missing option type 2
        var optionType1 = CreateOptionType(1, "Cheese", 1, 1);
        var optionType2 = CreateOptionType(2, "Bread", 1, 1);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType1, optionType2 };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreOptionsComplete_WithMixedSingleAndMultiSelect_AllCompleted_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState();
        state.SingleSelectOptions[1] = "Extra Cheese";
        state.MultiSelectOptions[2] = new List<string> { "Lettuce", "Tomato" };
        var optionType1 = CreateOptionType(1, "Cheese", 1, 1);
        var optionType2 = CreateOptionType(2, "Toppings", 2, 2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType1, optionType2 };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreOptionsComplete_WithMixedSingleAndMultiSelect_MultiSelectIncomplete_ReturnsFalse()
    {
        // Arrange
        var state = new SelectionState();
        state.SingleSelectOptions[1] = "Extra Cheese";
        state.MultiSelectOptions[2] = new List<string> { "Lettuce" };
        var optionType1 = CreateOptionType(1, "Cheese", 1, 1);
        var optionType2 = CreateOptionType(2, "Toppings", 2, 2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType1, optionType2 };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreOptionsComplete_WithZeroMinimumToppings_ReturnsTrue()
    {
        // Arrange
        var state = new SelectionState();
        state.SingleSelectOptions[1] = "Extra Cheese";
        var optionType = CreateOptionType(1, "Cheese", 0, 1);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };

        // Act
        var result = SelectionValidator.AreOptionsComplete(state, optionTypes);

        // Assert
        Assert.True(result);
    }

    #endregion
}
