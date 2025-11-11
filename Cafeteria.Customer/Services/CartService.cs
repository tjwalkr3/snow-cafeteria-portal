namespace Cafeteria.Customer.Services;

using Cafeteria.Shared.DTOs;

public class CartService : ICartService
{
    private readonly IStorageWrapper _protectedStorage;

    public CartService(IStorageWrapper protectedStorage)
    {
        _protectedStorage = protectedStorage;
    }

    public async Task<BrowserOrder?> GetOrder(string key)
    {
        var result = await _protectedStorage.GetAsync<BrowserOrder>(key);
        return result.Success ? result.Value : null;
    }

    public async Task ClearOrder(string key)
    {
        await _protectedStorage.DeleteAsync(key);
    }

    public async Task SetLocation(string key, LocationDto location)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        order.Location = location;
        await _protectedStorage.SetAsync(key, order);
    }

    public async Task SetIsCardOrder(string key, bool isCardOrder)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        order.IsCardOrder = isCardOrder;
        await _protectedStorage.SetAsync(key, order);
    }

    public async Task AddEntree(string key, EntreeDto entree)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        order.Entrees.Add(new OrderEntreeItem { Entree = entree });
        await _protectedStorage.SetAsync(key, order);
    }

    public async Task AddSide(string key, SideDto side)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        order.Sides.Add(new OrderSideItem { Side = side });
        await _protectedStorage.SetAsync(key, order);
    }

    public async Task AddDrink(string key, DrinkDto drink)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        order.Drinks.Add(drink);
        await _protectedStorage.SetAsync(key, order);
    }

    public async Task AddEntreeOption(string key, int entreeId, FoodOptionDto option, FoodOptionTypeDto optionType)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        var item = order.Entrees.FirstOrDefault(e => e.Entree.Id == entreeId);
        if (item != null)
        {
            item.SelectedOptions.Add(new SelectedFoodOption { Option = option, OptionType = optionType });
            await _protectedStorage.SetAsync(key, order);
        }
    }

    public async Task AddSideOption(string key, int sideId, FoodOptionDto option, FoodOptionTypeDto optionType)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        var item = order.Sides.FirstOrDefault(s => s.Side.Id == sideId);
        if (item != null)
        {
            item.SelectedOptions.Add(new SelectedFoodOption { Option = option, OptionType = optionType });
            await _protectedStorage.SetAsync(key, order);
        }
    }

    public async Task RemoveEntree(string key, int entreeId)
    {
        var order = await GetOrder(key);
        if (order != null)
        {
            var item = order.Entrees.FirstOrDefault(e => e.Entree.Id == entreeId);
            if (item != null)
            {
                order.Entrees.Remove(item);
                await _protectedStorage.SetAsync(key, order);
            }
        }
    }

    public async Task RemoveSide(string key, int sideId)
    {
        var order = await GetOrder(key);
        if (order != null)
        {
            var item = order.Sides.FirstOrDefault(s => s.Side.Id == sideId);
            if (item != null)
            {
                order.Sides.Remove(item);
                await _protectedStorage.SetAsync(key, order);
            }
        }
    }

    public async Task RemoveDrink(string key, int drinkId)
    {
        var order = await GetOrder(key);
        if (order != null)
        {
            var drink = order.Drinks.FirstOrDefault(d => d.Id == drinkId);
            if (drink != null)
            {
                order.Drinks.Remove(drink);
                await _protectedStorage.SetAsync(key, order);
            }
        }
    }

    public async Task RemoveEntreeOption(string key, int entreeId, int optionId)
    {
        var order = await GetOrder(key);
        if (order != null)
        {
            var item = order.Entrees.FirstOrDefault(e => e.Entree.Id == entreeId);
            if (item != null)
            {
                var option = item.SelectedOptions.FirstOrDefault(o => o.Option.Id == optionId);
                if (option != null)
                {
                    item.SelectedOptions.Remove(option);
                    await _protectedStorage.SetAsync(key, order);
                }
            }
        }
    }

    public async Task RemoveSideOption(string key, int sideId, int optionId)
    {
        var order = await GetOrder(key);
        if (order != null)
        {
            var item = order.Sides.FirstOrDefault(s => s.Side.Id == sideId);
            if (item != null)
            {
                var option = item.SelectedOptions.FirstOrDefault(o => o.Option.Id == optionId);
                if (option != null)
                {
                    item.SelectedOptions.Remove(option);
                    await _protectedStorage.SetAsync(key, order);
                }
            }
        }
    }

}