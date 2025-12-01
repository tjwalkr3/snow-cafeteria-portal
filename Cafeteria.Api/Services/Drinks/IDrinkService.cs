using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface IDrinkService
{
    Task<DrinkDto> CreateDrink(DrinkDto drinkDto);
    Task<DrinkDto?> GetDrinkByID(int id);
    Task<List<DrinkDto>> GetAllDrinks();
    Task<DrinkDto?> UpdateDrinkByID(int id, DrinkDto drinkDto);
    Task<bool> DeleteDrinkByID(int id);
}
