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
}