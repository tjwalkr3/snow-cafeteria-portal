using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface IDrinkService
{
    Task<DrinkDto> CreateDrink(DrinkDto drinkDto);
    Task<DrinkDto?> GetDrinkByID(int id);
    Task<List<DrinkDto>> GetAllDrinks();
    Task<List<DrinkDto>> GetDrinksByStationID(int stationId);
    Task<DrinkDto?> UpdateDrinkByID(int id, DrinkDto drinkDto);
    Task<bool> DeleteDrinkByID(int id);
    Task<bool> SetInStockById(int id, bool inStock);
}
