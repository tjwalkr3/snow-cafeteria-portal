using Cafeteria.Management.Components.Pages.Order;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.UnitTests.Management.Components.Pages.Order;

public class OrderDetailGroupingTests
{
    [Fact]
    public void GetGroupedCardItems_GroupsExactDuplicates()
    {
        var items = new List<FoodItemDto>
        {
            CreateItem("Burger", 8.99m, 1, null, "Cheese", "Bacon"),
            CreateItem("Burger", 8.99m, 1, null, "Bacon", "Cheese"),
            CreateItem("Burger", 8.99m, 1, null, "Lettuce")
        };

        var grouped = OrderDetail.GetGroupedCardItems(items);

        Assert.Equal(2, grouped.Count);
        Assert.Contains(grouped, g => g.Item.Name == "Burger" && g.Item.Options.Count == 2 && g.Count == 2);
        Assert.Contains(grouped, g => g.Item.Name == "Burger" && g.Item.Options.Count == 1 && g.Count == 1);
    }

    [Fact]
    public void GetGroupedSwipeItems_GroupsDuplicateSwipeCombos()
    {
        var items = new List<FoodItemDto>
        {
            CreateItem("Burger", null, 1, null, "Cheese"),
            CreateItem("Fries", null, 1, null, "Salt"),
            CreateItem("Coke", null, null, 1),
            CreateItem("Burger", null, 1, null, "Cheese"),
            CreateItem("Fries", null, 1, null, "Salt"),
            CreateItem("Coke", null, null, 1),
            CreateItem("Chicken", null, 1, null),
            CreateItem("Rice", null, 1, null),
            CreateItem("Water", null, null, 1)
        };

        var grouped = OrderDetail.GetGroupedSwipeItems(items);

        Assert.Equal(2, grouped.Count);
        Assert.Contains(grouped, g => g.Entree.Name == "Burger" && g.Side.Name == "Fries" && g.Drink.Name == "Coke" && g.Count == 2);
        Assert.Contains(grouped, g => g.Entree.Name == "Chicken" && g.Side.Name == "Rice" && g.Drink.Name == "Water" && g.Count == 1);
    }

    private static FoodItemDto CreateItem(string name, decimal? cardCost, int? stationId, int? locationId, params string[] options)
    {
        return new FoodItemDto
        {
            Name = name,
            CardCost = cardCost,
            SwipeCost = 1,
            StationId = stationId,
            LocationId = locationId,
            Options = options.Select(option => new FoodItemOptionDto { FoodOptionName = option }).ToList()
        };
    }
}
