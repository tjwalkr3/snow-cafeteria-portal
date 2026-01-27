using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services;

public interface IDrinkService
{
    Task<DrinkDto> CreateDrink(DrinkDto drinkDto);
    Task<DrinkDto?> GetDrinkByID(int id);
    Task<List<DrinkDto>> GetAllDrinks();
    Task<List<DrinkDto>> GetDrinksByLocationID(int locationId);
    Task<DrinkDto?> UpdateDrinkByID(int id, DrinkDto drinkDto);
    Task<bool> DeleteDrinkByID(int id);
    Task<bool> SetStockStatusById(int id, bool inStock);
}
