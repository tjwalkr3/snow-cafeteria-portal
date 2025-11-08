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

}