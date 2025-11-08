using Cafeteria.Customer.Components.Pages.PlaceOrder;
using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;
using Moq;

namespace Cafeteria.Customer.Tests.ViewModelTests;

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
}
