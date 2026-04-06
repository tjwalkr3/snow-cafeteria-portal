using Cafeteria.Customer.Components.Pages.PlaceOrder;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;
using Moq;

namespace Cafeteria.UnitTests.Customer.ViewModels;

public class PlaceOrderVMTests
{
    private readonly Mock<IApiMenuService> _mockMenuService;

    public PlaceOrderVMTests()
    {
        _mockMenuService = new Mock<IApiMenuService>();
    }

    [Fact]
    public void CalculateTotalPrice_ReturnsZero_WhenOrderIsNull()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);

        // Act
        var result = vm.CalculateTotalPrice(null!);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateTotalPrice_ReturnsZero_WhenOrderIsEmpty()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder();

        // Act
        var result = vm.CalculateTotalPrice(order);

        // Assert
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateTotalPrice_CalculatesEntreePrice_Correctly()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder();
        order.Entrees.Add(new OrderEntreeItem
        {
            Entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8.99m }
        });

        // Act
        var result = vm.CalculateTotalPrice(order);

        // Assert
        Assert.Equal(8.99m, result);
    }

    [Fact]
    public void CalculateTotalPrice_CalculatesSidePrice_Correctly()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder();
        order.Sides.Add(new OrderSideItem
        {
            Side = new SideDto { Id = 1, SideName = "Fries", SidePrice = 3.50m }
        });

        // Act
        var result = vm.CalculateTotalPrice(order);

        // Assert
        Assert.Equal(3.50m, result);
    }

    [Fact]
    public void CalculateTotalPrice_CalculatesDrinkPrice_Correctly()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder();
        order.Drinks.Add(new DrinkDto { Id = 1, DrinkName = "Soda", DrinkPrice = 2.00m });

        // Act
        var result = vm.CalculateTotalPrice(order);

        // Assert
        Assert.Equal(2.00m, result);
    }

    [Fact]
    public void CalculateTotalPrice_IncludesEntreeOptions_InTotal()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder();
        order.Entrees.Add(new OrderEntreeItem
        {
            Entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8.99m },
            SelectedOptions = new List<SelectedFoodOption>
            {
                new SelectedFoodOption
                {
                    Option = new FoodOptionDto { Id = 1, FoodOptionName = "Cheese" },
                    OptionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Extra", FoodOptionPrice = 1.50m }
                },
                new SelectedFoodOption
                {
                    Option = new FoodOptionDto { Id = 2, FoodOptionName = "Bacon" },
                    OptionType = new FoodOptionTypeDto { Id = 2, FoodOptionTypeName = "Extra", FoodOptionPrice = 2.00m }
                }
            }
        });

        // Act
        var result = vm.CalculateTotalPrice(order);

        // Assert
        Assert.Equal(12.49m, result); // 8.99 + 1.50 + 2.00
    }

    [Fact]
    public void CalculateTotalPrice_IncludesSideOptions_InTotal()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder();
        order.Sides.Add(new OrderSideItem
        {
            Side = new SideDto { Id = 1, SideName = "Fries", SidePrice = 3.50m },
            SelectedOptions = new List<SelectedFoodOption>
            {
                new SelectedFoodOption
                {
                    Option = new FoodOptionDto { Id = 1, FoodOptionName = "Salt" },
                    OptionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Seasoning", FoodOptionPrice = 0.25m }
                }
            }
        });

        // Act
        var result = vm.CalculateTotalPrice(order);

        // Assert
        Assert.Equal(3.75m, result); // 3.50 + 0.25
    }

    [Fact]
    public void CalculateTotalPrice_CalculatesCompleteOrder_Correctly()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder();

        // Add entrees with options
        order.Entrees.Add(new OrderEntreeItem
        {
            Entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8.99m },
            SelectedOptions = new List<SelectedFoodOption>
            {
                new SelectedFoodOption
                {
                    Option = new FoodOptionDto { Id = 1, FoodOptionName = "Cheese" },
                    OptionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Extra", FoodOptionPrice = 1.50m }
                }
            }
        });

        // Add sides with options
        order.Sides.Add(new OrderSideItem
        {
            Side = new SideDto { Id = 1, SideName = "Fries", SidePrice = 3.50m },
            SelectedOptions = new List<SelectedFoodOption>
            {
                new SelectedFoodOption
                {
                    Option = new FoodOptionDto { Id = 2, FoodOptionName = "Ketchup" },
                    OptionType = new FoodOptionTypeDto { Id = 2, FoodOptionTypeName = "Condiment", FoodOptionPrice = 0.00m }
                }
            }
        });

        // Add drinks
        order.Drinks.Add(new DrinkDto { Id = 1, DrinkName = "Soda", DrinkPrice = 2.00m });

        // Act
        var result = vm.CalculateTotalPrice(order);

        // Assert
        Assert.Equal(15.99m, result); // 8.99 + 1.50 + 3.50 + 0.00 + 2.00
    }

    [Fact]
    public void CalculateTotalPrice_HandlesMultipleItems_Correctly()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder();

        order.Entrees.Add(new OrderEntreeItem
        {
            Entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8.99m }
        });
        order.Entrees.Add(new OrderEntreeItem
        {
            Entree = new EntreeDto { Id = 2, EntreeName = "Sandwich", EntreePrice = 6.50m }
        });

        order.Sides.Add(new OrderSideItem
        {
            Side = new SideDto { Id = 1, SideName = "Fries", SidePrice = 3.50m }
        });
        order.Sides.Add(new OrderSideItem
        {
            Side = new SideDto { Id = 2, SideName = "Salad", SidePrice = 4.00m }
        });

        order.Drinks.Add(new DrinkDto { Id = 1, DrinkName = "Soda", DrinkPrice = 2.00m });
        order.Drinks.Add(new DrinkDto { Id = 2, DrinkName = "Water", DrinkPrice = 0.00m });

        // Act
        var result = vm.CalculateTotalPrice(order);

        // Assert
        Assert.Equal(24.99m, result); // 8.99 + 6.50 + 3.50 + 4.00 + 2.00 + 0.00
    }

    [Fact]
    public void GroupItemsIntoSwipes_NoSides_CreatesGroupsWithNullSide()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder
        {
            IsCardOrder = false,
            Entrees = [new OrderEntreeItem { Entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8m } }],
            Sides = [],
            Drinks = [new DrinkDto { Id = 1, DrinkName = "Soda", DrinkPrice = 1m }]
        };

        // Act
        var groups = vm.GroupItemsIntoSwipes(order);

        // Assert
        Assert.Single(groups);
        Assert.Null(groups[0].Side);
    }

    [Fact]
    public void GroupItemsIntoSwipes_NoSides_SwipeCountMatchesEntreesAndDrinks()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder
        {
            IsCardOrder = false,
            Entrees =
            [
                new OrderEntreeItem { Entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8m } },
                new OrderEntreeItem { Entree = new EntreeDto { Id = 2, EntreeName = "Wrap", EntreePrice = 7m } }
            ],
            Sides = [],
            Drinks =
            [
                new DrinkDto { Id = 1, DrinkName = "Soda", DrinkPrice = 1m },
                new DrinkDto { Id = 2, DrinkName = "Water", DrinkPrice = 0m }
            ]
        };

        // Act
        var groups = vm.GroupItemsIntoSwipes(order);

        // Assert
        Assert.Equal(2, groups.Sum(g => g.Quantity));
    }

    [Fact]
    public void GroupItemsIntoSwipes_WithSides_SideIsNotNull()
    {
        // Arrange
        var vm = new PlaceOrderVM(_mockMenuService.Object);
        var order = new BrowserOrder
        {
            IsCardOrder = false,
            Entrees = [new OrderEntreeItem { Entree = new EntreeDto { Id = 1, EntreeName = "Burger", EntreePrice = 8m } }],
            Sides = [new OrderSideItem { Side = new SideDto { Id = 1, SideName = "Fries", SidePrice = 2m } }],
            Drinks = [new DrinkDto { Id = 1, DrinkName = "Soda", DrinkPrice = 1m }]
        };

        // Act
        var groups = vm.GroupItemsIntoSwipes(order);

        // Assert
        Assert.Single(groups);
        Assert.NotNull(groups[0].Side);
    }

    [Fact]
    public async Task ErrorOccurred_ReturnsTrue_WhenInitializeLocationsFails()
    {
        _mockMenuService.Setup(m => m.GetAllLocations())
            .ThrowsAsync(new Exception("API Error"));

        var vm = new PlaceOrderVM(_mockMenuService.Object);

        await vm.InitializeLocations();

        Assert.True(vm.ErrorOccurred());
    }

    [Fact]
    public async Task InitializeLocations_SetsLocations_WhenApiCallSucceeds()
    {
        var expectedLocations = new List<LocationDto>
        {
            new LocationDto { Id = 1, LocationName = "Location 1" },
            new LocationDto { Id = 2, LocationName = "Location 2" }
        };

        _mockMenuService.Setup(m => m.GetAllLocations())
            .ReturnsAsync(expectedLocations);

        var vm = new PlaceOrderVM(_mockMenuService.Object);

        await vm.InitializeLocations();

        Assert.False(vm.ErrorOccurred());
    }

    [Fact]
    public async Task GetLocationById_ReturnsLocation_WhenLocationExists()
    {
        var expectedLocations = new List<LocationDto>
        {
            new LocationDto { Id = 1, LocationName = "Location 1" },
            new LocationDto { Id = 2, LocationName = "Location 2" }
        };

        _mockMenuService.Setup(m => m.GetAllLocations())
            .ReturnsAsync(expectedLocations);

        var vm = new PlaceOrderVM(_mockMenuService.Object);
        await vm.InitializeLocations();

        var result = vm.GetLocationById(1);

        Assert.NotNull(result);
        Assert.Equal("Location 1", result.LocationName);
    }

    [Fact]
    public async Task GetLocationById_ReturnsNull_WhenLocationDoesNotExist()
    {
        var expectedLocations = new List<LocationDto>
        {
            new LocationDto { Id = 1, LocationName = "Location 1" }
        };

        _mockMenuService.Setup(m => m.GetAllLocations())
            .ReturnsAsync(expectedLocations);

        var vm = new PlaceOrderVM(_mockMenuService.Object);
        await vm.InitializeLocations();

        var result = vm.GetLocationById(99);

        Assert.Null(result);
    }
}
