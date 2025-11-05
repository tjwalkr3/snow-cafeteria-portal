namespace Cafeteria.Customer.Services;

using Cafeteria.Shared.DTOs;

public class CartService : ICartService
{
    private readonly IProtectedStorageWrapper _protectedStorage;

    public CartService(IProtectedStorageWrapper protectedStorage)
    {
        _protectedStorage = protectedStorage;
    }

    public async Task<BrowserOrder?> GetOrder(string key)
    {
        var result = await _protectedStorage.GetAsync<BrowserOrder>(key);
        return result.Success ? result.Value : null;
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

    public async Task AddEntree(string key, OrderEntreeItem entree)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        order.Entrees.Add(entree);
        await _protectedStorage.SetAsync(key, order);
    }

    public async Task AddSide(string key, OrderSideItem side)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        order.Sides.Add(side);
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
        if (item == null)
        {
            item = new OrderEntreeItem { Entree = new EntreeDto { Id = entreeId } };
            order.Entrees.Add(item);
        }

        item.SelectedOptions.Add(new SelectedFoodOption { Option = option, OptionType = optionType });
        await _protectedStorage.SetAsync(key, order);
    }

    public async Task AddSideOption(string key, int sideId, FoodOptionDto option, FoodOptionTypeDto optionType)
    {
        var order = await GetOrder(key) ?? new BrowserOrder();
        var item = order.Sides.FirstOrDefault(s => s.Side.Id == sideId);
        if (item == null)
        {
            item = new OrderSideItem { Side = new SideDto { Id = sideId } };
            order.Sides.Add(item);
        }

        item.SelectedOptions.Add(new SelectedFoodOption { Option = option, OptionType = optionType });
        await _protectedStorage.SetAsync(key, order);
    }

}