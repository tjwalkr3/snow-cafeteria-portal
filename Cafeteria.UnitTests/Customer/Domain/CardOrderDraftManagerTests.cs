using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.UnitTests.Customer.Domain;

public class CardOrderDraftManagerTests
{
    [Fact]
    public void SetSelections_IncrementsAndTracksState()
    {
        var manager = new CardOrderDraftManager();

        manager.SetEntreeSelection(
            1,
            [new FoodOptionTypeWithOptionsDto { OptionType = new FoodOptionTypeDto { Id = 100 } }],
            new Dictionary<int, HashSet<string>> { [100] = ["Cheddar"] });

        manager.SetSideSelection(
            2,
            [new FoodOptionTypeWithOptionsDto { OptionType = new FoodOptionTypeDto { Id = 200 } }],
            new Dictionary<int, HashSet<string>> { [200] = ["Ranch"] });

        manager.IncrementDrink(3);

        Assert.True(manager.HasAnySelection());
        Assert.True(manager.HasEntreeSelection());
        Assert.True(manager.HasSideSelection());
        Assert.True(manager.HasDrinkSelection());
        Assert.Equal(1, manager.TotalEntreeCount());
        Assert.Equal(1, manager.TotalSideCount());
        Assert.Equal(1, manager.TotalDrinkCount());
    }

    [Fact]
    public void SetQuantityZero_RemovesAssociatedSelectionState()
    {
        var manager = new CardOrderDraftManager();

        manager.SetEntreeSelection(
            1,
            [new FoodOptionTypeWithOptionsDto { OptionType = new FoodOptionTypeDto { Id = 100 } }],
            new Dictionary<int, HashSet<string>> { [100] = ["Cheddar"] });

        manager.SetEntreeQuantity(1, 0);

        Assert.False(manager.ContainsEntree(1));
        Assert.False(manager.HasEntreeSelection());
    }

    [Fact]
    public void CreateSideTempSelectionState_ReturnsClonedSelections()
    {
        var manager = new CardOrderDraftManager();
        manager.SetSideSelection(
            2,
            [new FoodOptionTypeWithOptionsDto { OptionType = new FoodOptionTypeDto { Id = 200 } }],
            new Dictionary<int, HashSet<string>> { [200] = ["Ketchup"] });

        var temp = manager.CreateSideTempSelectionState(2);
        temp.SideOptions[200].Add("Ranch");

        var secondTemp = manager.CreateSideTempSelectionState(2);
        Assert.Single(secondTemp.SideOptions[200]);
        Assert.Contains("Ketchup", secondTemp.SideOptions[200]);
    }

    [Fact]
    public void CreateSnapshot_ClonesInternalState()
    {
        var manager = new CardOrderDraftManager();
        manager.SetEntreeSelection(
            1,
            [new FoodOptionTypeWithOptionsDto { OptionType = new FoodOptionTypeDto { Id = 100 } }],
            new Dictionary<int, HashSet<string>> { [100] = ["Cheddar"] });

        var snapshot = manager.CreateSnapshot(
            [new EntreeDto { Id = 1, EntreeName = "Burger" }],
            [],
            []);

        snapshot.EntreeOptions[1][100].Add("Swiss");

        var secondSnapshot = manager.CreateSnapshot(
            [new EntreeDto { Id = 1, EntreeName = "Burger" }],
            [],
            []);

        Assert.Single(secondSnapshot.EntreeOptions[1][100]);
        Assert.Contains("Cheddar", secondSnapshot.EntreeOptions[1][100]);
    }
}