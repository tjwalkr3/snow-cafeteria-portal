using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.UnitTests.Customer.Domain;

public class SelectionStateTests
{
    private SelectionState _state = null!;

    public SelectionStateTests()
    {
        _state = new SelectionState();
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

    #region Clear Tests

    [Fact]
    public void Clear_WithAllSelectionsAndOptions_ClearsEverything()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SelectedSide = CreateSide();
        _state.SelectedDrink = CreateDrink();
        _state.SingleSelectOptions[1] = "Extra Cheese";
        _state.MultiSelectOptions[1] = new List<string> { "Lettuce", "Tomato" };
        _state.SideOptions[2] = new HashSet<string> { "Extra Salt" };

        // Act
        _state.Clear();

        // Assert
        Assert.Null(_state.SelectedEntree);
        Assert.Null(_state.SelectedSide);
        Assert.Null(_state.SelectedDrink);
        Assert.Empty(_state.SingleSelectOptions);
        Assert.Empty(_state.MultiSelectOptions);
        Assert.Empty(_state.SideOptions);
    }

    [Fact]
    public void Clear_WithOnlyEntree_ClearsEntree()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();

        // Act
        _state.Clear();

        // Assert
        Assert.Null(_state.SelectedEntree);
    }

    [Fact]
    public void Clear_WithOnlySide_ClearsSide()
    {
        // Arrange
        _state.SelectedSide = CreateSide();

        // Act
        _state.Clear();

        // Assert
        Assert.Null(_state.SelectedSide);
    }

    [Fact]
    public void Clear_WithOnlyDrink_ClearsDrink()
    {
        // Arrange
        _state.SelectedDrink = CreateDrink();

        // Act
        _state.Clear();

        // Assert
        Assert.Null(_state.SelectedDrink);
    }

    [Fact]
    public void Clear_WithOnlySingleSelectOptions_ClearsSingleSelectOptions()
    {
        // Arrange
        _state.SingleSelectOptions[1] = "Extra Cheese";
        _state.SingleSelectOptions[2] = "Extra Sauce";

        // Act
        _state.Clear();

        // Assert
        Assert.Empty(_state.SingleSelectOptions);
    }

    [Fact]
    public void Clear_WithOnlyMultiSelectOptions_ClearsMultiSelectOptions()
    {
        // Arrange
        _state.MultiSelectOptions[1] = new List<string> { "Lettuce", "Tomato" };
        _state.MultiSelectOptions[2] = new List<string> { "Onion" };

        // Act
        _state.Clear();

        // Assert
        Assert.Empty(_state.MultiSelectOptions);
    }

    [Fact]
    public void Clear_WithOnlySideOptions_ClearsSideOptions()
    {
        // Arrange
        _state.SideOptions[1] = new HashSet<string> { "Extra Salt" };
        _state.SideOptions[2] = new HashSet<string> { "No Salt" };

        // Act
        _state.Clear();

        // Assert
        Assert.Empty(_state.SideOptions);
    }

    [Fact]
    public void Clear_WhenEmpty_StaysEmpty()
    {
        // Act
        _state.Clear();

        // Assert
        Assert.Null(_state.SelectedEntree);
        Assert.Null(_state.SelectedSide);
        Assert.Null(_state.SelectedDrink);
        Assert.Empty(_state.SingleSelectOptions);
        Assert.Empty(_state.MultiSelectOptions);
        Assert.Empty(_state.SideOptions);
    }

    #endregion

    #region ClearOptionsOnly Tests

    [Fact]
    public void ClearOptionsOnly_WithAllSelectionsAndOptions_ClearsOnlyOptions()
    {
        // Arrange
        var entree = CreateEntree();
        var side = CreateSide();
        var drink = CreateDrink();
        _state.SelectedEntree = entree;
        _state.SelectedSide = side;
        _state.SelectedDrink = drink;
        _state.SingleSelectOptions[1] = "Extra Cheese";
        _state.MultiSelectOptions[1] = new List<string> { "Lettuce", "Tomato" };
        _state.SideOptions[2] = new HashSet<string> { "Extra Salt" };

        // Act
        _state.ClearOptionsOnly();

        // Assert
        Assert.Equal(entree, _state.SelectedEntree);
        Assert.Equal(side, _state.SelectedSide);
        Assert.Equal(drink, _state.SelectedDrink);
        Assert.Empty(_state.SingleSelectOptions);
        Assert.Empty(_state.MultiSelectOptions);
        // SideOptions is NOT cleared by ClearOptionsOnly()
        Assert.NotEmpty(_state.SideOptions);
    }

    [Fact]
    public void ClearOptionsOnly_WithOnlySingleSelectOptions_ClearsSingleSelectOptions()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SingleSelectOptions[1] = "Extra Cheese";

        // Act
        _state.ClearOptionsOnly();

        // Assert
        Assert.NotNull(_state.SelectedEntree);
        Assert.Empty(_state.SingleSelectOptions);
    }

    [Fact]
    public void ClearOptionsOnly_WithOnlyMultiSelectOptions_ClearsMultiSelectOptions()
    {
        // Arrange
        _state.SelectedSide = CreateSide();
        _state.MultiSelectOptions[1] = new List<string> { "Lettuce" };

        // Act
        _state.ClearOptionsOnly();

        // Assert
        Assert.NotNull(_state.SelectedSide);
        Assert.Empty(_state.MultiSelectOptions);
    }

    [Fact]
    public void ClearOptionsOnly_DoesNotClearSideOptions()
    {
        // Arrange
        _state.SelectedSide = CreateSide();
        _state.SideOptions[1] = new HashSet<string> { "Extra Salt" };

        // Act
        _state.ClearOptionsOnly();

        // Assert
        Assert.NotNull(_state.SelectedSide);
        Assert.NotEmpty(_state.SideOptions);
    }

    #endregion

    #region HasAnySelection Tests

    [Fact]
    public void HasAnySelection_WithEntree_ReturnsTrue()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();

        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAnySelection_WithSide_ReturnsTrue()
    {
        // Arrange
        _state.SelectedSide = CreateSide();

        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAnySelection_WithDrink_ReturnsTrue()
    {
        // Arrange
        _state.SelectedDrink = CreateDrink();

        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAnySelection_WithEntreeAndSide_ReturnsTrue()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SelectedSide = CreateSide();

        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAnySelection_WithEntreeAndDrink_ReturnsTrue()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SelectedDrink = CreateDrink();

        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAnySelection_WithSideAndDrink_ReturnsTrue()
    {
        // Arrange
        _state.SelectedSide = CreateSide();
        _state.SelectedDrink = CreateDrink();

        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAnySelection_WithAllItems_ReturnsTrue()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SelectedSide = CreateSide();
        _state.SelectedDrink = CreateDrink();

        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasAnySelection_WithNoSelections_ReturnsFalse()
    {
        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasAnySelection_WithOnlyOptions_ReturnsFalse()
    {
        // Arrange
        _state.SingleSelectOptions[1] = "Extra Cheese";

        // Act
        var result = _state.HasAnySelection();

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetTotalSelectionCount Tests

    [Fact]
    public void GetTotalSelectionCount_WithNoSelections_ReturnsZero()
    {
        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void GetTotalSelectionCount_WithOnlyEntree_ReturnsOne()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();

        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void GetTotalSelectionCount_WithOnlySide_ReturnsOne()
    {
        // Arrange
        _state.SelectedSide = CreateSide();

        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void GetTotalSelectionCount_WithOnlyDrink_ReturnsOne()
    {
        // Arrange
        _state.SelectedDrink = CreateDrink();

        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void GetTotalSelectionCount_WithEntreeAndSide_ReturnsTwo()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SelectedSide = CreateSide();

        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void GetTotalSelectionCount_WithEntreeAndDrink_ReturnsTwo()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SelectedDrink = CreateDrink();

        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void GetTotalSelectionCount_WithSideAndDrink_ReturnsTwo()
    {
        // Arrange
        _state.SelectedSide = CreateSide();
        _state.SelectedDrink = CreateDrink();

        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void GetTotalSelectionCount_WithAllItems_ReturnsThree()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SelectedSide = CreateSide();
        _state.SelectedDrink = CreateDrink();

        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public void GetTotalSelectionCount_WithOptions_StillCountsOnlyMainItems()
    {
        // Arrange
        _state.SelectedEntree = CreateEntree();
        _state.SingleSelectOptions[1] = "Extra Cheese";
        _state.MultiSelectOptions[1] = new List<string> { "Lettuce", "Tomato" };

        // Act
        var count = _state.GetTotalSelectionCount();

        // Assert
        Assert.Equal(1, count); // Options don't count
    }

    #endregion
}
