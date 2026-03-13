using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Shared.DTOs.Menu;
using Moq;

namespace Cafeteria.UnitTests.Customer.Domain;

public class CartSubmitterTests
{
    private const string CART_KEY = "order";
    private readonly Mock<ICartService> _mockCartService;
    private readonly CartSubmitter _cartSubmitter;

    public CartSubmitterTests()
    {
        _mockCartService = new Mock<ICartService>();
        _cartSubmitter = new CartSubmitter(_mockCartService.Object);
    }

    #region Submit Tests

    [Fact]
    public async Task SubmitAsync_WithOnlyEntree_CallsAddEntree()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8.99m };
        var state = new SelectionState { SelectedEntree = entree };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var allEntreeOptions = new List<FoodOptionDto>();

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, allEntreeOptions);

        // Assert
        _mockCartService.Verify(x => x.AddEntree(CART_KEY, entree), Times.Once);
        _mockCartService.Verify(x => x.AddSide(It.IsAny<string>(), It.IsAny<SideDto>()), Times.Never);
        _mockCartService.Verify(x => x.AddDrink(It.IsAny<string>(), It.IsAny<DrinkDto>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithOnlySide_CallsAddSide()
    {
        // Arrange
        var side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 2.99m };
        var state = new SelectionState { SelectedSide = side };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var allEntreeOptions = new List<FoodOptionDto>();

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, allEntreeOptions, new List<FoodOptionTypeWithOptionsDto>());

        // Assert
        _mockCartService.Verify(x => x.AddSide(CART_KEY, side), Times.Once);
        _mockCartService.Verify(x => x.AddEntree(It.IsAny<string>(), It.IsAny<EntreeDto>()), Times.Never);
        _mockCartService.Verify(x => x.AddDrink(It.IsAny<string>(), It.IsAny<DrinkDto>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithOnlyDrink_CallsAddDrink()
    {
        // Arrange
        var drink = new DrinkDto { Id = 3, DrinkName = "Coke", DrinkPrice = 2.49m };
        var state = new SelectionState { SelectedDrink = drink };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var allEntreeOptions = new List<FoodOptionDto>();

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, allEntreeOptions);

        // Assert
        _mockCartService.Verify(x => x.AddDrink(CART_KEY, drink), Times.Once);
        _mockCartService.Verify(x => x.AddEntree(It.IsAny<string>(), It.IsAny<EntreeDto>()), Times.Never);
        _mockCartService.Verify(x => x.AddSide(It.IsAny<string>(), It.IsAny<SideDto>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithEntreeAndSide_CallsBoth()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8.99m };
        var side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 2.99m };
        var state = new SelectionState
        {
            SelectedEntree = entree,
            SelectedSide = side
        };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var allEntreeOptions = new List<FoodOptionDto>();

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, allEntreeOptions, new List<FoodOptionTypeWithOptionsDto>());

        // Assert
        _mockCartService.Verify(x => x.AddEntree(CART_KEY, entree), Times.Once);
        _mockCartService.Verify(x => x.AddSide(CART_KEY, side), Times.Once);
        _mockCartService.Verify(x => x.AddDrink(It.IsAny<string>(), It.IsAny<DrinkDto>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithAllItems_CallsAllMethods()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8.99m };
        var side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 2.99m };
        var drink = new DrinkDto { Id = 3, DrinkName = "Coke", DrinkPrice = 2.49m };
        var state = new SelectionState
        {
            SelectedEntree = entree,
            SelectedSide = side,
            SelectedDrink = drink
        };
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var allEntreeOptions = new List<FoodOptionDto>();

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, allEntreeOptions, new List<FoodOptionTypeWithOptionsDto>());

        // Assert
        _mockCartService.Verify(x => x.AddEntree(CART_KEY, entree), Times.Once);
        _mockCartService.Verify(x => x.AddSide(CART_KEY, side), Times.Once);
        _mockCartService.Verify(x => x.AddDrink(CART_KEY, drink), Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithNoItems_CallsNoMethods()
    {
        // Arrange
        var state = new SelectionState();
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var allEntreeOptions = new List<FoodOptionDto>();

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, allEntreeOptions);

        // Assert
        _mockCartService.Verify(x => x.AddEntree(It.IsAny<string>(), It.IsAny<EntreeDto>()), Times.Never);
        _mockCartService.Verify(x => x.AddSide(It.IsAny<string>(), It.IsAny<SideDto>()), Times.Never);
        _mockCartService.Verify(x => x.AddDrink(It.IsAny<string>(), It.IsAny<DrinkDto>()), Times.Never);
    }

    #endregion

    #region Entree Option Tests

    [Fact]
    public async Task SubmitAsync_WithSingleSelectOption_AddsSingleSelectOption()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Breakfast", EntreePrice = 6.99m };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Eggs", MaxAmount = 1 };
        var option = new FoodOptionDto { Id = 10, FoodOptionName = "Scrambled" };

        var state = new SelectionState { SelectedEntree = entree };
        state.SingleSelectOptions[optionType.Id] = "Scrambled";

        var optionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            new FoodOptionTypeWithOptionsDto
            {
                OptionType = optionType,
                Options = new List<FoodOptionDto> { option }
            }
        };

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, new List<FoodOptionDto>());

        // Assert
        _mockCartService.Verify(
            x => x.AddEntreeOption(CART_KEY, entree.Id, option, optionType),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithMultiSelectOptions_AddsMultipleOptions()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Sandwich", EntreePrice = 7.99m };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Toppings", MaxAmount = 5 };
        var option1 = new FoodOptionDto { Id = 10, FoodOptionName = "Lettuce" };
        var option2 = new FoodOptionDto { Id = 11, FoodOptionName = "Tomato" };

        var state = new SelectionState { SelectedEntree = entree };
        state.MultiSelectOptions[optionType.Id] = new List<string> { "Lettuce", "Tomato" };

        var optionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            new FoodOptionTypeWithOptionsDto
            {
                OptionType = optionType,
                Options = new List<FoodOptionDto> { option1, option2 }
            }
        };

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, new List<FoodOptionDto>());

        // Assert
        _mockCartService.Verify(
            x => x.AddEntreeOption(CART_KEY, entree.Id, option1, It.IsAny<FoodOptionTypeDto>()),
            Times.Once);
        _mockCartService.Verify(
            x => x.AddEntreeOption(CART_KEY, entree.Id, option2, It.IsAny<FoodOptionTypeDto>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithToppings_AddsToppingOptions()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Pizza", EntreePrice = 12.99m };
        var toppingsOptionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Pizza Toppings" };
        var topping1 = new FoodOptionDto { Id = 20, FoodOptionName = "Pepperoni" };
        var topping2 = new FoodOptionDto { Id = 21, FoodOptionName = "Mushroom" };

        var state = new SelectionState { SelectedEntree = entree };
        state.SelectedToppings.Add("Pepperoni");
        state.SelectedToppings.Add("Mushroom");

        var optionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            new FoodOptionTypeWithOptionsDto
            {
                OptionType = toppingsOptionType,
                Options = new List<FoodOptionDto> { topping1, topping2 }
            }
        };

        var allEntreeOptions = new List<FoodOptionDto> { topping1, topping2 };

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, allEntreeOptions);

        // Assert
        _mockCartService.Verify(
            x => x.AddEntreeOption(CART_KEY, entree.Id, topping1, It.IsAny<FoodOptionTypeDto>()),
            Times.Once);
        _mockCartService.Verify(
            x => x.AddEntreeOption(CART_KEY, entree.Id, topping2, It.IsAny<FoodOptionTypeDto>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithNonExistentOption_SkipsOption()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Breakfast", EntreePrice = 6.99m };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Eggs", MaxAmount = 1 };
        var option = new FoodOptionDto { Id = 10, FoodOptionName = "Scrambled" };

        var state = new SelectionState { SelectedEntree = entree };
        state.SingleSelectOptions[optionType.Id] = "Fried"; // Option name that doesn't exist

        var optionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            new FoodOptionTypeWithOptionsDto
            {
                OptionType = optionType,
                Options = new List<FoodOptionDto> { option }
            }
        };

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, new List<FoodOptionDto>());

        // Assert
        _mockCartService.Verify(
            x => x.AddEntreeOption(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FoodOptionDto>(), It.IsAny<FoodOptionTypeDto>()),
            Times.Never);
    }

    #endregion

    #region Side Option Tests

    [Fact]
    public async Task SubmitAsync_WithSideOptions_AddsSideOptions()
    {
        // Arrange
        var side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 2.99m };
        var sideOptionType = new FoodOptionTypeDto { Id = 2, FoodOptionTypeName = "Size", MaxAmount = 1 };
        var sideOption = new FoodOptionDto { Id = 30, FoodOptionName = "Large" };

        var state = new SelectionState { SelectedSide = side };
        state.SideOptions[sideOptionType.Id] = new HashSet<string> { "Large" };

        var sideOptionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            new FoodOptionTypeWithOptionsDto
            {
                OptionType = sideOptionType,
                Options = new List<FoodOptionDto> { sideOption }
            }
        };

        // Act
        await _cartSubmitter.SubmitAsync(new SelectionState { SelectedSide = side }, new List<FoodOptionTypeWithOptionsDto>(), new List<FoodOptionDto>(), sideOptionTypes);

        // Assert
        // Update the state with side options for the actual submit
        var stateWithOptions = new SelectionState { SelectedSide = side };
        stateWithOptions.SideOptions[sideOptionType.Id] = new HashSet<string> { "Large" };

        await _cartSubmitter.SubmitAsync(stateWithOptions, new List<FoodOptionTypeWithOptionsDto>(), new List<FoodOptionDto>(), sideOptionTypes);

        _mockCartService.Verify(
            x => x.AddSideOption(CART_KEY, side.Id, sideOption, sideOptionType),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithSideAndNullSideOptionTypes_SkipsSideOptions()
    {
        // Arrange
        var side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 2.99m };
        var state = new SelectionState { SelectedSide = side };

        // Act
        await _cartSubmitter.SubmitAsync(state, new List<FoodOptionTypeWithOptionsDto>(), new List<FoodOptionDto>(), null);

        // Assert
        _mockCartService.Verify(x => x.AddSide(CART_KEY, side), Times.Once);
        _mockCartService.Verify(
            x => x.AddSideOption(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FoodOptionDto>(), It.IsAny<FoodOptionTypeDto>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithSideAndEmptySideOptions_SkipsSideOptions()
    {
        // Arrange
        var side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 2.99m };
        var state = new SelectionState { SelectedSide = side };
        var sideOptionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        await _cartSubmitter.SubmitAsync(state, new List<FoodOptionTypeWithOptionsDto>(), new List<FoodOptionDto>(), sideOptionTypes);

        // Assert
        _mockCartService.Verify(x => x.AddSide(CART_KEY, side), Times.Once);
        _mockCartService.Verify(
            x => x.AddSideOption(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FoodOptionDto>(), It.IsAny<FoodOptionTypeDto>()),
            Times.Never);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task SubmitAsync_WithEmptySingleSelectOption_SkipsOption()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Breakfast", EntreePrice = 6.99m };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Eggs", MaxAmount = 1 };
        var option = new FoodOptionDto { Id = 10, FoodOptionName = "Scrambled" };

        var state = new SelectionState { SelectedEntree = entree };
        state.SingleSelectOptions[optionType.Id] = ""; // Empty string

        var optionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            new FoodOptionTypeWithOptionsDto
            {
                OptionType = optionType,
                Options = new List<FoodOptionDto> { option }
            }
        };

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, new List<FoodOptionDto>());

        // Assert
        _mockCartService.Verify(
            x => x.AddEntreeOption(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FoodOptionDto>(), It.IsAny<FoodOptionTypeDto>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithEmptyMultiSelectOptions_SkipsOptions()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Sandwich", EntreePrice = 7.99m };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Toppings", MaxAmount = 5 };

        var state = new SelectionState { SelectedEntree = entree };
        state.MultiSelectOptions[optionType.Id] = new List<string>(); // Empty list

        var optionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            new FoodOptionTypeWithOptionsDto
            {
                OptionType = optionType,
                Options = new List<FoodOptionDto>()
            }
        };

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, new List<FoodOptionDto>());

        // Assert
        _mockCartService.Verify(
            x => x.AddEntreeOption(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FoodOptionDto>(), It.IsAny<FoodOptionTypeDto>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithNoToppings_SkipsToppingLogic()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Pizza", EntreePrice = 12.99m };
        var state = new SelectionState { SelectedEntree = entree };
        // No toppings selected

        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, new List<FoodOptionDto>());

        // Assert
        _mockCartService.Verify(x => x.AddEntree(CART_KEY, entree), Times.Once);
        _mockCartService.Verify(
            x => x.AddEntreeOption(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FoodOptionDto>(), It.IsAny<FoodOptionTypeDto>()),
            Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithToppingButNoMatchingOptionType_SkipsTopping()
    {
        // Arrange
        var entree = new EntreeDto { Id = 1, EntreeName = "Pizza", EntreePrice = 12.99m };
        var topping = new FoodOptionDto { Id = 20, FoodOptionName = "Pepperoni" };

        var state = new SelectionState { SelectedEntree = entree };
        state.SelectedToppings.Add("Pepperoni");

        var optionTypes = new List<FoodOptionTypeWithOptionsDto>
        {
            new FoodOptionTypeWithOptionsDto
            {
                OptionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Other" },
                Options = new List<FoodOptionDto>()
            }
        };

        var allEntreeOptions = new List<FoodOptionDto> { topping };

        // Act
        await _cartSubmitter.SubmitAsync(state, optionTypes, allEntreeOptions);

        // Assert
        // Should not add the topping option because no matching option type was found
        _mockCartService.Verify(
            x => x.AddEntreeOption(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<FoodOptionDto>(), It.IsAny<FoodOptionTypeDto>()),
            Times.Never);
    }

    #endregion
}
