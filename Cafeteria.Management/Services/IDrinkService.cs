using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface IDrinkService
{
    Task<List<DrinkDto>> GetAllDrinks();
    Task<DrinkDto?> GetDrinkById(int id);
    Task<DrinkDto> CreateDrink(DrinkDto drinkDto);
    Task<DrinkDto?> UpdateDrinkById(int id, DrinkDto drinkDto);
    Task<bool> DeleteDrinkById(int id);
    Task<bool> SetInStockById(int id, bool inStock);
}
