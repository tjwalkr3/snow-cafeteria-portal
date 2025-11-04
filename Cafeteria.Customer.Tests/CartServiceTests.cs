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
}
