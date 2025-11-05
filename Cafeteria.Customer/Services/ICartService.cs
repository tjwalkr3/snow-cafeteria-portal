namespace Cafeteria.Customer.Services;

using Cafeteria.Shared.DTOs;

public interface ICartService
{
    Task<BrowserOrder?> GetOrder(string key);
    Task SetLocation(string key, LocationDto location);
    Task SetIsCardOrder(string key, bool isCardOrder);
    Task AddEntree(string key, OrderEntreeItem entree);
    Task AddSide(string key, OrderSideItem side);
    Task AddDrink(string key, DrinkDto drink);
    Task AddEntreeOption(string key, int entreeId, FoodOptionDto option, FoodOptionTypeDto optionType);
    Task AddSideOption(string key, int sideId, FoodOptionDto option, FoodOptionTypeDto optionType);
}