using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.Drinks;

public interface IDrinkService
{
    Task<DrinkDto> CreateDrink(DrinkDto drinkDto);
    Task<DrinkDto?> GetDrinkById(int id);
    Task<List<DrinkDto>> GetAllDrinks();
    Task<List<DrinkDto>> GetDrinksByLocationId(int locationId);
    Task<DrinkDto?> UpdateDrinkById(int id, DrinkDto drinkDto);
    Task<bool> DeleteDrinkById(int id);
    Task<bool> SetStockStatusById(int id, bool inStock);
}
