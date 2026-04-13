using Cafeteria.Customer.Components.Pages.OrderHistory;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Customer.Services.Order;
using Cafeteria.Shared.DTOs.Order;
using Moq;

namespace Cafeteria.UnitTests.Customer.ViewModels;

public class OrderHistoryVMTests
{
    [Fact]
    public void GetGroupedFoodItems_GroupsExactDuplicatesAndReturnsCounts()
    {
        var orderService = new Mock<IApiOrderService>();
        var menuService = new Mock<IApiMenuService>();
        var vm = new OrderHistoryVM(orderService.Object, menuService.Object);

        var order = new OrderDto
        {
            FoodItems =
            [
                new FoodItemDto
                {
                    Name = "Burger",
                    StationId = 1,
                    CardCost = 8.99m,
                    SwipeCost = 1,
                    Options =
                    [
                        new FoodItemOptionDto { FoodOptionName = "Cheese" },
                        new FoodItemOptionDto { FoodOptionName = "Bacon" }
                    ]
                },
                new FoodItemDto
                {
                    Name = "Burger",
                    StationId = 1,
                    CardCost = 8.99m,
                    SwipeCost = 1,
                    Options =
                    [
                        new FoodItemOptionDto { FoodOptionName = "Bacon" },
                        new FoodItemOptionDto { FoodOptionName = "Cheese" }
                    ]
                },
                new FoodItemDto
                {
                    Name = "Burger",
                    StationId = 1,
                    CardCost = 8.99m,
                    SwipeCost = 1,
                    Options =
                    [
                        new FoodItemOptionDto { FoodOptionName = "Lettuce" }
                    ]
                },
                new FoodItemDto
                {
                    Name = "Fries",
                    LocationId = 2,
                    CardCost = 2.50m,
                    SwipeCost = 0,
                    Options = []
                }
            ]
        };

        var grouped = vm.GetGroupedFoodItems(order);

        Assert.Equal(3, grouped.Count);
        Assert.Contains(grouped, g => g.Item.Name == "Burger" && g.Item.Options.Count == 2 && g.Count == 2);
        Assert.Contains(grouped, g => g.Item.Name == "Burger" && g.Item.Options.Count == 1 && g.Count == 1);
        Assert.Contains(grouped, g => g.Item.Name == "Fries" && g.Count == 1);
    }
}
