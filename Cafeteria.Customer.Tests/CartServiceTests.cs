using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Tests;

public class CartServiceTests
{
    // Dictionary-based test implementation
    private class DictionaryStorageWrapper : IProtectedStorageWrapper
    {
        private readonly Dictionary<string, object> _storage = new();

        public void SetValue<T>(string key, T value)
        {
            _storage[key] = value!;
        }

        public ValueTask<StorageResult<T>> GetAsync<T>(string key)
        {
            if (_storage.TryGetValue(key, out var value) && value is T typedValue)
            {
                return ValueTask.FromResult(new StorageResult<T>(true, typedValue));
            }
            return ValueTask.FromResult(new StorageResult<T>(false, default));
        }

        public ValueTask SetAsync<T>(string key, T value)
        {
            _storage[key] = value!;
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
    public async Task AddEntree_AddsEntreeToNewOrder()
    {
        // Arrange
        var storage = new DictionaryStorageWrapper();
        var cartService = new CartService(storage);
        var entree = new OrderEntreeItem
        {
            Entree = new EntreeDto { Id = 1, EntreeName = "Turkey Sandwich", EntreePrice = 6.50m }
        };

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
        var side = new OrderSideItem
        {
            Side = new SideDto { Id = 1, SideName = "French Fries", SidePrice = 2.50m }
        };

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
}
