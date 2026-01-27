using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.FoodOptionTypes;

public interface IFoodOptionTypeService
{
    Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto);
    Task<FoodOptionTypeDto?> GetFoodTypeByID(int id);
    Task<List<FoodOptionTypeDto>> GetAllFoodTypes();
    Task<FoodOptionTypeDto?> UpdateFoodType(int id, FoodOptionTypeDto foodTypeDto);
    Task<bool> DeleteFoodType(int id);
}
