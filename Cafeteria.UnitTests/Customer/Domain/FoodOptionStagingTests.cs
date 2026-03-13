using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.UnitTests.Customer.Domain;

public class FoodOptionStagingStoreTests
{
    #region Setup

    private FoodOptionStagingStore _store = null!;

    public FoodOptionStagingStoreTests()
    {
        _store = new FoodOptionStagingStore();
    }

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

    private static FoodOptionTypeDto CreateOptionType(int id = 1, string name = "Cheese", short numIncluded = 1, int maxAmount = 1)
    {
        return new FoodOptionTypeDto
        {
            Id = id,
            FoodOptionTypeName = name,
            IncludedAmount = numIncluded,
            MaxAmount = maxAmount
        };
    }

    private static FoodOptionTypeWithOptionsDto CreateOptionTypeWithOptions(int id = 1, string name = "Cheese", short numIncluded = 1, int maxAmount = 1, params FoodOptionDto[] options)
    {
        return new FoodOptionTypeWithOptionsDto
        {
            OptionType = CreateOptionType(id, name, numIncluded, maxAmount),
            Options = options.ToList()
        };
    }

    private static SideWithOptionsDto CreateSideWithOptions(SideDto? side = null, params FoodOptionTypeWithOptionsDto[] optionTypes)
    {
        return new SideWithOptionsDto
        {
            Side = side ?? CreateSide(),
            OptionTypes = optionTypes.ToList()
        };
    }

    #endregion

    #region Open Tests

    [Fact]
    public void Open_WithEntree_SetsModalProperties()
    {
        // Arrange
        var entree = CreateEntree(1, "Burger");
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var state = new SelectionState();

        // Act
        _store.Open(entree, optionTypes, state);

        // Assert
        Assert.True(_store.IsModalOpen);
        Assert.Equal("Burger", _store.ModalTitle);
        Assert.Equal(entree, _store.StagedEntree);
        Assert.Null(_store.StagedSide);
    }

    [Fact]
    public void Open_WithEntreeAndOptions_InitializesStagedSelections()
    {
        // Arrange
        var entree = CreateEntree();
        var option1 = CreateFoodOption(1, "Extra Cheese");
        var option2 = CreateFoodOption(2, "No Cheese");
        var optionType = CreateOptionTypeWithOptions(1, "Cheese", 1, 1, option1, option2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };
        var state = new SelectionState();

        // Act
        _store.Open(entree, optionTypes, state);

        // Assert
        Assert.NotEmpty(_store.StagedSelections);
        Assert.Contains(1, _store.StagedSelections.Keys);
        Assert.Empty(_store.StagedSelections[1]);
    }

    [Fact]
    public void Open_WithExistingSingleSelectOption_PreservesPreviousSelection()
    {
        // Arrange
        var entree = CreateEntree();
        var option1 = CreateFoodOption(1, "Extra Cheese");
        var option2 = CreateFoodOption(2, "No Cheese");
        var optionType = CreateOptionTypeWithOptions(1, "Cheese", 1, 1, option1, option2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };
        var state = new SelectionState();
        state.SingleSelectOptions[1] = "Extra Cheese";

        // Act
        _store.Open(entree, optionTypes, state);

        // Assert
        Assert.Contains("Extra Cheese", _store.StagedSelections[1]);
    }

    [Fact]
    public void Open_WithExistingMultiSelectOptions_PreservesMultiplePreviousSelections()
    {
        // Arrange
        var entree = CreateEntree();
        var option1 = CreateFoodOption(1, "Lettuce");
        var option2 = CreateFoodOption(2, "Tomato");
        var option3 = CreateFoodOption(3, "Onion");
        var optionType = CreateOptionTypeWithOptions(1, "Toppings", 3, 3, option1, option2, option3);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };
        var state = new SelectionState();
        state.MultiSelectOptions[1] = new List<string> { "Lettuce", "Tomato" };

        // Act
        _store.Open(entree, optionTypes, state);

        // Assert
        Assert.Contains("Lettuce", _store.StagedSelections[1]);
        Assert.Contains("Tomato", _store.StagedSelections[1]);
        Assert.DoesNotContain("Onion", _store.StagedSelections[1]);
    }

    [Fact]
    public void OpenForSide_WithSide_SetsModalProperties()
    {
        // Arrange
        var side = CreateSide(1, "Fries");
        var sideWithOptions = CreateSideWithOptions(side);
        var state = new SelectionState();

        // Act
        _store.OpenForSide(sideWithOptions, state);

        // Assert
        Assert.True(_store.IsModalOpen);
        Assert.Equal("Fries", _store.ModalTitle);
        Assert.Equal(side, _store.StagedSide?.Side);
        Assert.Null(_store.StagedEntree);
    }

    [Fact]
    public void OpenForSide_WithSideOptions_InitializesStagedSelections()
    {
        // Arrange
        var side = CreateSide();
        var option = CreateFoodOption(1, "Extra Salt");
        var optionType = CreateOptionTypeWithOptions(1, "Seasoning", 1, 1, option);
        var sideWithOptions = CreateSideWithOptions(side, optionType);
        var state = new SelectionState();

        // Act
        _store.OpenForSide(sideWithOptions, state);

        // Assert
        Assert.NotEmpty(_store.StagedSelections);
        Assert.Contains(1, _store.StagedSelections.Keys);
    }

    [Fact]
    public void OpenForSide_WithExistingOptions_PreservesSelections()
    {
        // Arrange
        var side = CreateSide();
        var option1 = CreateFoodOption(1, "Extra Salt");
        var option2 = CreateFoodOption(2, "No Salt");
        var optionType = CreateOptionTypeWithOptions(1, "Seasoning", 1, 1, option1, option2);
        var sideWithOptions = CreateSideWithOptions(side, optionType);
        var state = new SelectionState();
        state.SideOptions[1] = new HashSet<string> { "Extra Salt" };

        // Act
        _store.OpenForSide(sideWithOptions, state);

        // Assert
        Assert.Contains("Extra Salt", _store.StagedSelections[1]);
    }

    #endregion

    #region Toggle Tests

    [Fact]
    public void Toggle_AddingOption_AddsToStagedSelections()
    {
        // Arrange
        var optionType = CreateOptionTypeWithOptions(1, "Cheese", 1, 1);
        _store.StagedSelections[1] = new HashSet<string>();

        // Act
        _store.Toggle(1, "Extra Cheese", optionType, true);

        // Assert
        Assert.Contains("Extra Cheese", _store.StagedSelections[1]);
    }

    [Fact]
    public void Toggle_RemovingOption_RemovesFromStagedSelections()
    {
        // Arrange
        var optionType = CreateOptionTypeWithOptions(1, "Cheese", 1, 1);
        _store.StagedSelections[1] = new HashSet<string> { "Extra Cheese" };

        // Act
        _store.Toggle(1, "Extra Cheese", optionType, true);

        // Assert
        Assert.DoesNotContain("Extra Cheese", _store.StagedSelections[1]);
    }

    [Fact]
    public void Toggle_MultiSelectBelowLimit_AddsOption()
    {
        // Arrange
        var optionType = CreateOptionTypeWithOptions(1, "Toppings", 3, 3);
        _store.StagedSelections[1] = new HashSet<string> { "Lettuce" };

        // Act
        _store.Toggle(1, "Tomato", optionType, false);

        // Assert
        Assert.Contains("Tomato", _store.StagedSelections[1]);
        Assert.Equal(2, _store.StagedSelections[1].Count);
    }

    [Fact]
    public void Toggle_MultiSelectAtLimit_ExceedsLimit()
    {
        // Arrange
        var optionType = CreateOptionTypeWithOptions(1, "Toppings", 2, 2);
        _store.StagedSelections[1] = new HashSet<string> { "Lettuce", "Tomato" };

        // Act
        _store.Toggle(1, "Onion", optionType, false);

        // Assert
        Assert.DoesNotContain("Onion", _store.StagedSelections[1]);
        Assert.Equal(2, _store.StagedSelections[1].Count);
    }

[Fact]
    public void Toggle_NonExistentOptionType_CreatesNewEntry()
    {
        // Arrange
        var optionType = CreateOptionTypeWithOptions(5, "NewType", 1, 1);

        // Act
        _store.Toggle(5, "Option", optionType, true);

        // Assert
        Assert.Contains(5, _store.StagedSelections.Keys);
        Assert.Contains("Option", _store.StagedSelections[5]);
    }

    #endregion

    #region Set Tests

    [Fact]
    public void Set_SingleSelection_ReplacesExistingSelections()
    {
        // Arrange
        _store.StagedSelections[1] = new HashSet<string> { "Lettuce", "Tomato" };

        // Act
        _store.Set(1, "Onion");

        // Assert
        Assert.Single(_store.StagedSelections[1]);
        Assert.Contains("Onion", _store.StagedSelections[1]);
        Assert.DoesNotContain("Lettuce", _store.StagedSelections[1]);
        Assert.DoesNotContain("Tomato", _store.StagedSelections[1]);
    }

    [Fact]
    public void Set_NonExistentOptionType_CreatesNewEntry()
    {
        // Act
        _store.Set(3, "NewOption");

        // Assert
        Assert.Contains(3, _store.StagedSelections.Keys);
        Assert.Single(_store.StagedSelections[3]);
        Assert.Contains("NewOption", _store.StagedSelections[3]);
    }

    #endregion

    #region Confirm Tests

    [Fact]
    public void Confirm_WithEntree_UpdatesStateWithEntree()
    {
        // Arrange
        var entree = CreateEntree(1, "Burger");
        var optionType = CreateOptionTypeWithOptions(1, "Cheese", 1, 1);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };
        var state = new SelectionState();

        _store.Open(entree, optionTypes, state);

        // Act
        _store.Confirm(state, optionTypes);

        // Assert
        Assert.Equal(entree, state.SelectedEntree);
        Assert.False(_store.IsModalOpen);
    }

    [Fact]
    public void Confirm_WithSingleSelectOptions_UpdatesStateWithSelection()
    {
        // Arrange
        var entree = CreateEntree();
        var option = CreateFoodOption(1, "Extra Cheese");
        var optionType = CreateOptionTypeWithOptions(1, "Cheese", 1, 1, option);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };
        var state = new SelectionState();

        _store.Open(entree, optionTypes, state);
        _store.Set(1, "Extra Cheese");

        // Act
        _store.Confirm(state, optionTypes);

        // Assert
        Assert.True(state.SingleSelectOptions.ContainsKey(1));
        Assert.Equal("Extra Cheese", state.SingleSelectOptions[1]);
    }

    [Fact]
    public void Confirm_WithMultiSelectOptions_UpdatesStateWithSelections()
    {
        // Arrange
        var entree = CreateEntree();
        var option1 = CreateFoodOption(1, "Lettuce");
        var option2 = CreateFoodOption(2, "Tomato");
        var optionType = CreateOptionTypeWithOptions(1, "Toppings", 2, 2, option1, option2);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };
        var state = new SelectionState();

        _store.Open(entree, optionTypes, state);
        _store.Toggle(1, "Lettuce", optionType, true);
        _store.Toggle(1, "Tomato", optionType, true);

        // Act
        _store.Confirm(state, optionTypes);

        // Assert
        Assert.True(state.MultiSelectOptions.ContainsKey(1));
        Assert.Contains("Lettuce", state.MultiSelectOptions[1]);
        Assert.Contains("Tomato", state.MultiSelectOptions[1]);
    }

    [Fact]
    public void Confirm_WithSide_UpdatesStateWithSide()
    {
        // Arrange
        var side = CreateSide(1, "Fries");
        var sideWithOptions = CreateSideWithOptions(side);
        var state = new SelectionState();

        _store.OpenForSide(sideWithOptions, state);

        // Act
        _store.Confirm(state, new List<FoodOptionTypeWithOptionsDto>());

        // Assert
        Assert.Equal(side, state.SelectedSide);
        Assert.False(_store.IsModalOpen);
    }

    [Fact]
    public void Confirm_WithSideOptions_UpdatesStateWithSideOptions()
    {
        // Arrange
        var side = CreateSide();
        var option = CreateFoodOption(1, "Extra Salt");
        var optionType = CreateOptionTypeWithOptions(1, "Seasoning", 1, 1, option);
        var sideWithOptions = CreateSideWithOptions(side, optionType);
        var state = new SelectionState();

        _store.OpenForSide(sideWithOptions, state);
        _store.Toggle(1, "Extra Salt", optionType, true);

        // Act
        _store.Confirm(state, new List<FoodOptionTypeWithOptionsDto> { optionType });

        // Assert
        Assert.True(state.SideOptions.ContainsKey(1));
        Assert.Contains("Extra Salt", state.SideOptions[1]);
    }

    [Fact]
    public void Confirm_WithEmptySelections_UpdatesStateWithEmptySelections()
    {
        // Arrange
        var entree = CreateEntree();
        var optionType = CreateOptionTypeWithOptions(1, "Cheese", 1, 1);
        var optionTypes = new List<FoodOptionTypeWithOptionsDto> { optionType };
        var state = new SelectionState();

        _store.Open(entree, optionTypes, state);
        // Don't select anything

        // Act
        _store.Confirm(state, optionTypes);

        // Assert
        Assert.Equal(entree, state.SelectedEntree);
        Assert.False(state.SingleSelectOptions.ContainsKey(1));
    }

    [Fact]
    public void Confirm_ClosesModal()
    {
        // Arrange
        var entree = CreateEntree();
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var state = new SelectionState();

        _store.Open(entree, optionTypes, state);
        Assert.True(_store.IsModalOpen);

        // Act
        _store.Confirm(state, optionTypes);

        // Assert
        Assert.False(_store.IsModalOpen);
    }

    #endregion

    #region Discard Tests

    [Fact]
    public void Discard_ClearsAllStagedData()
    {
        // Arrange
        var entree = CreateEntree();
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var state = new SelectionState();

        _store.Open(entree, optionTypes, state);
        _store.Toggle(1, "Option", CreateOptionTypeWithOptions(1, "Type", 1, 1), true);

        // Act
        _store.Discard();

        // Assert
        Assert.Null(_store.StagedEntree);
        Assert.Null(_store.StagedSide);
        Assert.Empty(_store.StagedSelections);
        Assert.False(_store.IsModalOpen);
    }

    [Fact]
    public void Discard_DoesNotModifyOriginalState()
    {
        // Arrange
        var entree = CreateEntree();
        var optionTypes = new List<FoodOptionTypeWithOptionsDto>();
        var state = new SelectionState();
        state.SelectedEntree = entree;

        _store.Open(CreateEntree(2, "Pizza"), optionTypes, state);

        // Act
        _store.Discard();

        // Assert
        Assert.Equal(entree, state.SelectedEntree); // Original state unchanged
        Assert.Null(_store.StagedEntree);
    }

    [Fact]
    public void Discard_WhenNotOpen_HasNoEffect()
    {
        // Act
        _store.Discard();

        // Assert
        Assert.False(_store.IsModalOpen);
        Assert.Empty(_store.StagedSelections);
    }

    #endregion
}
