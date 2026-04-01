using Cafeteria.Customer.Services.Cart;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Customer.Services.Storage;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.UnitTests.Customer.Services;

public class CartServiceTests
{
    // Dictionary-based test implementation
    private class DictionaryStorageWrapper : IStorageWrapper
    {
        private readonly Dictionary<string, object> _storage = new();

        public int GetCallCount { get; private set; }
        public int SetCallCount { get; private set; }
        public int DeleteCallCount { get; private set; }

        public void SetValue<T>(string key, T value)
        {
            _storage[key] = value!;
        }

        public ValueTask<StorageResult<T>> GetAsync<T>(string key)
        {
            GetCallCount++;
            if (_storage.TryGetValue(key, out var value) && value is T typedValue)
            {
                return ValueTask.FromResult(new StorageResult<T>(true, typedValue));
            }
            return ValueTask.FromResult(new StorageResult<T>(false, default));
        }

        public ValueTask SetAsync<T>(string key, T value)
        {
            SetCallCount++;
            _storage[key] = value!;
            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteAsync(string key)
        {
            DeleteCallCount++;
            _storage.Remove(key);
            return ValueTask.CompletedTask;
        }
    }

    [Fact]
    public async Task GetOrder_ReturnsOrder_WhenKeyExists()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var expectedOrder = new BrowserOrder
        {
            IsCardOrder = true,
            Location = new LocationDto { Id = 1, LocationName = "Badger Den" }
        };
        storage.SetValue("test-order", expectedOrder);
        var cartService = new CartService(storage);

        // Act
        var result = await cartService.GetOrder("test-order");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOrder.IsCardOrder, result.IsCardOrder);
        Assert.Equal(expectedOrder.Location.LocationName, result.Location?.LocationName);
    }

    [Fact]
    public async Task GetOrder_ReturnsNull_WhenKeyDoesNotExist()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);

        // Act
        var result = await cartService.GetOrder("non-existent-key");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ClearOrder_RemovesOrderFromStorage()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);
        storage.SetValue("test-order", new BrowserOrder { IsCardOrder = true });

        // Act
        await cartService.ClearOrder("test-order");

        // Assert
        var result = await cartService.GetOrder("test-order");
        Assert.Null(result);
    }

    [Fact]
    public async Task AddEntree_AddsEntreeToNewOrder()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);
        var entree = new EntreeDto { Id = 1, EntreeName = "Turkey Sandwich", EntreePrice = 6.50m };

        // Act
        await cartService.AddEntree("test-order", entree);

        // Assert
        var order = await cartService.GetOrder("test-order");
        Assert.NotNull(order);
        Assert.Single(order.Entrees);
        Assert.Equal("Turkey Sandwich", order.Entrees[0].Entree.EntreeName);
    }

    [Fact]
    public async Task AddSide_AddsSideToExistingOrder()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);
        storage.SetValue("test-order", new BrowserOrder { IsCardOrder = true });
        var side = new SideDto { Id = 1, SideName = "French Fries", SidePrice = 2.50m };

        // Act
        await cartService.AddSide("test-order", side);

        // Assert
        var order = await cartService.GetOrder("test-order");
        Assert.NotNull(order);
        Assert.Single(order.Sides);
        Assert.Equal("French Fries", order.Sides[0].Side.SideName);
    }

    [Fact]
    public async Task AddDrink_AddsDrinkToOrder()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);
        var drink = new DrinkDto { Id = 1, DrinkName = "Soda", DrinkPrice = 1.99m };

        // Act
        await cartService.AddDrink("test-order", drink);

        // Assert
        var order = await cartService.GetOrder("test-order");
        Assert.NotNull(order);
        Assert.Single(order.Drinks);
        Assert.Equal("Soda", order.Drinks[0].DrinkName);
    }

    [Fact]
    public async Task AddEntreeOption_AddsOptionToExistingEntree()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var existingEntree = new OrderEntreeItem
        {
            Entree = new EntreeDto { Id = 10, EntreeName = "Test Entree", EntreePrice = 5.00m }
        };
        var initialOrder = new BrowserOrder();
        initialOrder.Entrees.Add(existingEntree);
        storage.SetValue("test-order", initialOrder);

        var cartService = new CartService(storage);

        var option = new FoodOptionDto { Id = 1, FoodOptionName = "Lettuce" };
        var optionType = new FoodOptionTypeDto { Id = 1, FoodOptionTypeName = "Vegetable", FoodOptionPrice = 0.25m };

        // Act
        await cartService.AddEntreeOption("test-order", 10, option, optionType);

        // Assert
        var order = await cartService.GetOrder("test-order");
        Assert.NotNull(order);
        Assert.Single(order.Entrees);
        Assert.Single(order.Entrees[0].SelectedOptions);
        Assert.Equal("Lettuce", order.Entrees[0].SelectedOptions[0].Option.FoodOptionName);
        Assert.Equal("Vegetable", order.Entrees[0].SelectedOptions[0].OptionType.FoodOptionTypeName);
    }

    [Fact]
    public async Task AddSideOption_AddsOptionToExistingSide()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var existingSide = new OrderSideItem
        {
            Side = new SideDto { Id = 20, SideName = "Test Side", SidePrice = 1.50m }
        };
        var initialOrder = new BrowserOrder();
        initialOrder.Sides.Add(existingSide);
        storage.SetValue("test-order", initialOrder);

        var cartService = new CartService(storage);

        var option = new FoodOptionDto { Id = 2, FoodOptionName = "Ketchup" };
        var optionType = new FoodOptionTypeDto { Id = 2, FoodOptionTypeName = "Condiment", FoodOptionPrice = 0.00m };

        // Act
        await cartService.AddSideOption("test-order", 20, option, optionType);

        // Assert
        var order = await cartService.GetOrder("test-order");
        Assert.NotNull(order);
        Assert.Single(order.Sides);
        Assert.Single(order.Sides[0].SelectedOptions);
        Assert.Equal("Ketchup", order.Sides[0].SelectedOptions[0].Option.FoodOptionName);
        Assert.Equal("Condiment", order.Sides[0].SelectedOptions[0].OptionType.FoodOptionTypeName);
    }

    [Fact]
    public async Task SetLocation_SetsLocationOnOrder()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);
        var location = new LocationDto { Id = 1, LocationName = "Badger Den", LocationDescription = "Main cafeteria" };

        // Act
        await cartService.SetLocation("test-order", location);

        // Assert
        var order = await cartService.GetOrder("test-order");
        Assert.NotNull(order);
        Assert.NotNull(order.Location);
        Assert.Equal("Badger Den", order.Location.LocationName);
    }

    [Fact]
    public async Task SetIsCardOrder_SetsCardOrderFlag()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);

        // Act
        await cartService.SetIsCardOrder("test-order", true);

        // Assert
        var order = await cartService.GetOrder("test-order");
        Assert.NotNull(order);
        Assert.True(order.IsCardOrder);
    }

    [Fact]
    public async Task UpdateCardOrderItems_ReplacesAllCardOrderItemsInSingleReadWrite()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var existingOrder = new BrowserOrder
        {
            IsCardOrder = true,
            StationId = 3,
            StationName = "Grill"
        };
        existingOrder.Entrees.Add(new OrderEntreeItem
        {
            Entree = new EntreeDto { Id = 100, EntreeName = "Old Entree", EntreePrice = 4.99m }
        });
        existingOrder.Sides.Add(new OrderSideItem
        {
            Side = new SideDto { Id = 200, SideName = "Old Side", SidePrice = 1.99m }
        });
        existingOrder.Drinks.Add(new DrinkDto { Id = 300, DrinkName = "Old Drink", DrinkPrice = 1.49m });
        storage.SetValue("test-order", existingOrder);

        var cartService = new CartService(storage);

        var entrees = new List<OrderEntreeItem>
        {
            new()
            {
                Entree = new EntreeDto { Id = 1, EntreeName = "New Entree", EntreePrice = 5.49m },
                SelectedOptions =
                [
                    new SelectedFoodOption
                    {
                        Option = new FoodOptionDto { Id = 10, FoodOptionName = "No Onions" },
                        OptionType = new FoodOptionTypeDto { Id = 11, FoodOptionTypeName = "Toppings", FoodOptionPrice = 0m }
                    }
                ]
            }
        };
        var sides = new List<OrderSideItem>
        {
            new()
            {
                Side = new SideDto { Id = 2, SideName = "New Side", SidePrice = 2.49m },
                SelectedOptions =
                [
                    new SelectedFoodOption
                    {
                        Option = new FoodOptionDto { Id = 12, FoodOptionName = "Ketchup" },
                        OptionType = new FoodOptionTypeDto { Id = 13, FoodOptionTypeName = "Sauce", FoodOptionPrice = 0m }
                    }
                ]
            }
        };
        var drinks = new List<DrinkDto>
        {
            new() { Id = 3, DrinkName = "New Drink", DrinkPrice = 1.99m }
        };

        // Capture counts after seed setup so we only measure method behavior.
        var initialGetCount = storage.GetCallCount;
        var initialSetCount = storage.SetCallCount;

        // Act
        await cartService.UpdateCardOrderItems("test-order", entrees, sides, drinks);

        // Assert
        Assert.Equal(initialGetCount + 1, storage.GetCallCount);
        Assert.Equal(initialSetCount + 1, storage.SetCallCount);

        var order = await cartService.GetOrder("test-order");
        Assert.NotNull(order);
        Assert.True(order.IsCardOrder);
        Assert.Equal(3, order.StationId);
        Assert.Equal("Grill", order.StationName);

        Assert.Single(order.Entrees);
        Assert.Equal("New Entree", order.Entrees[0].Entree.EntreeName);
        Assert.Single(order.Entrees[0].SelectedOptions);
        Assert.Equal("No Onions", order.Entrees[0].SelectedOptions[0].Option.FoodOptionName);

        Assert.Single(order.Sides);
        Assert.Equal("New Side", order.Sides[0].Side.SideName);
        Assert.Single(order.Sides[0].SelectedOptions);
        Assert.Equal("Ketchup", order.Sides[0].SelectedOptions[0].Option.FoodOptionName);

        Assert.Single(order.Drinks);
        Assert.Equal("New Drink", order.Drinks[0].DrinkName);
    }

    [Fact]
    public async Task UpdateCardOrderItems_CreatesOrderWhenMissing()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);

        var entrees = new List<OrderEntreeItem>();
        var sides = new List<OrderSideItem>();
        var drinks = new List<DrinkDto>
        {
            new() { Id = 42, DrinkName = "Water", DrinkPrice = 0.00m }
        };

        // Capture counts before method call.
        var initialGetCount = storage.GetCallCount;
        var initialSetCount = storage.SetCallCount;

        // Act
        await cartService.UpdateCardOrderItems("missing-order", entrees, sides, drinks);

        // Assert
        Assert.Equal(initialGetCount + 1, storage.GetCallCount);
        Assert.Equal(initialSetCount + 1, storage.SetCallCount);

        var order = await cartService.GetOrder("missing-order");
        Assert.NotNull(order);
        Assert.Empty(order.Entrees);
        Assert.Empty(order.Sides);
        Assert.Single(order.Drinks);
        Assert.Equal("Water", order.Drinks[0].DrinkName);
    }
}
