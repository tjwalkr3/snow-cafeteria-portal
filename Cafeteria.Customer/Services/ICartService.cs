namespace Cafeteria.Customer.Services;

using Cafeteria.Shared.DTOs;

public interface ICartService
{
    Task<BrowserOrder?> GetOrder(string key);
    Task ClearOrder(string key);
    Task SetLocation(string key, LocationDto location);
    Task SetIsCardOrder(string key, bool isCardOrder);
    Task AddEntree(string key, EntreeDto entree);
    Task AddSide(string key, SideDto side);
    Task AddDrink(string key, DrinkDto drink);
    Task AddEntreeOption(string key, int entreeId, FoodOptionDto option, FoodOptionTypeDto optionType);
    Task AddSideOption(string key, int sideId, FoodOptionDto option, FoodOptionTypeDto optionType);
    Task RemoveEntree(string key, int entreeId);
    Task RemoveSide(string key, int sideId);
    Task RemoveDrink(string key, int drinkId);
    Task RemoveEntreeOption(string key, int entreeId, int optionId);
    Task RemoveSideOption(string key, int sideId, int optionId);
}