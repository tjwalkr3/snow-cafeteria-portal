using Cafeteria.Customer.Components.Pages.Stations.Domain;
using Cafeteria.Customer.Services.Cart;
using Cafeteria.Customer.Services.Storage;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.UnitTests.Customer.Domain;

public class CardOrderFlowTests
{
    private class CountingStorageWrapper : IStorageWrapper
    {
        private readonly Dictionary<string, object> _storage = new();

        public int GetCallCount { get; private set; }

        public int SetCallCount { get; private set; }

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
            _storage.Remove(key);
            return ValueTask.CompletedTask;
        }
    }

    [Fact]
    public async Task CardFlow_SelectionAutoSyncThenFlush_PersistsExpectedOrder()
    {
        var storage = new CountingStorageWrapper();
        storage.SetValue("order", new BrowserOrder { IsCardOrder = true, StationId = 3, StationName = "Grill" });
        var cart = new CartService(storage);
        var draftManager = new CardOrderDraftManager();

        var entrees = new List<EntreeDto>
        {
            new() { Id = 1, EntreeName = "Burger", EntreePrice = 5.99m }
        };
        var sides = new List<SideWithOptionsDto>
        {
            new()
            {
                Side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 1.99m },
                OptionTypes =
                [
                    new FoodOptionTypeWithOptionsDto
                    {
                        OptionType = new FoodOptionTypeDto { Id = 200, FoodOptionTypeName = "Sauce", MaxAmount = 3 },
                        Options =
                        [
                            new FoodOptionDto { Id = 201, FoodOptionName = "Ketchup" },
                            new FoodOptionDto { Id = 202, FoodOptionName = "Ranch" }
                        ]
                    }
                ]
            }
        };
        var drinks = new List<DrinkDto>
        {
            new() { Id = 3, DrinkName = "Soda", DrinkPrice = 1.49m }
        };

        draftManager.SetEntreeSelection(
            1,
            [new FoodOptionTypeWithOptionsDto
            {
                OptionType = new FoodOptionTypeDto { Id = 100, FoodOptionTypeName = "Cheese", MaxAmount = 1 },
                Options =
                [
                    new FoodOptionDto { Id = 101, FoodOptionName = "American" },
                    new FoodOptionDto { Id = 102, FoodOptionName = "Cheddar" }
                ]
            }],
            new Dictionary<int, HashSet<string>> { [100] = ["Cheddar"] });
        draftManager.SetSideSelection(
            2,
            sides[0].OptionTypes,
            new Dictionary<int, HashSet<string>> { [200] = ["Ketchup"] });
        draftManager.IncrementDrink(3);
        draftManager.IncrementDrink(3);

        var dirty = true;
        using var coordinator = new CardOrderAutoSyncCoordinator(async () =>
        {
            if (!dirty)
                return;

            var draft = draftManager.CreateSnapshot(entrees, sides, drinks);
            var mapped = StationDraftToOrderMapper.MapCardSelections(draft);
            await cart.UpdateCardOrderItems("order", mapped.Entrees, mapped.Sides, mapped.Drinks);
            dirty = false;
        }, debounceMs: 1000);

        coordinator.Schedule();
        await coordinator.FlushAsync();

        var order = await cart.GetOrder("order");
        Assert.NotNull(order);
        Assert.Single(order.Entrees);
        Assert.Single(order.Entrees[0].SelectedOptions);
        Assert.Equal("Cheddar", order.Entrees[0].SelectedOptions[0].Option.FoodOptionName);
        Assert.Single(order.Sides);
        Assert.Equal("Ketchup", order.Sides[0].SelectedOptions[0].Option.FoodOptionName);
        Assert.Equal(2, order.Drinks.Count);
    }

    [Fact]
    public async Task CardFlow_BurstChangesThenFlush_UsesSingleBatchWriteEvenForLargeSelectionCounts()
    {
        var storage = new CountingStorageWrapper();
        storage.SetValue("order", new BrowserOrder { IsCardOrder = true });
        var cart = new CartService(storage);
        var draftManager = new CardOrderDraftManager();

        var entrees = new List<EntreeDto>
        {
            new() { Id = 1, EntreeName = "Burger", EntreePrice = 5.99m }
        };
        var sides = new List<SideWithOptionsDto>
        {
            new() { Side = new SideDto { Id = 2, SideName = "Fries", SidePrice = 1.99m }, OptionTypes = [] }
        };
        var drinks = new List<DrinkDto>
        {
            new() { Id = 3, DrinkName = "Soda", DrinkPrice = 1.49m }
        };

        for (var i = 0; i < 100; i++)
        {
            draftManager.IncrementEntree(1);
            draftManager.IncrementSide(2);
            draftManager.IncrementDrink(3);
        }

        var dirty = true;
        using var coordinator = new CardOrderAutoSyncCoordinator(async () =>
        {
            if (!dirty)
                return;

            var draft = draftManager.CreateSnapshot(entrees, sides, drinks);
            var mapped = StationDraftToOrderMapper.MapCardSelections(draft);
            await cart.UpdateCardOrderItems("order", mapped.Entrees, mapped.Sides, mapped.Drinks);
            dirty = false;
        }, debounceMs: 1000);

        coordinator.Schedule();
        coordinator.Schedule();
        coordinator.Schedule();
        await coordinator.FlushAsync();

        var getCallsAfterFlush = storage.GetCallCount;
        var setCallsAfterFlush = storage.SetCallCount;

        Assert.Equal(1, setCallsAfterFlush);
        Assert.Equal(1, getCallsAfterFlush);

        var order = await cart.GetOrder("order");
        Assert.NotNull(order);
        Assert.Equal(100, order.Entrees.Count);
        Assert.Equal(100, order.Sides.Count);
        Assert.Equal(100, order.Drinks.Count);
    }
}