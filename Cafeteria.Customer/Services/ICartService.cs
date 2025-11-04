namespace Cafeteria.Customer.Services;

using Cafeteria.Shared.DTOs;

public interface ICartService
{
    Task<BrowserOrder?> GetOrder(string key);
    Task AddEntree(string key, OrderEntreeItem entree);
    Task AddSide(string key, OrderSideItem side);
    Task AddDrink(string key, DrinkDto drink);
}