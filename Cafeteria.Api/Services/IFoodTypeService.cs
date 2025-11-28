using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface IFoodTypeService
{
    Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto);
    Task<FoodOptionTypeDto?> GetFoodTypeByID(int id);
    Task<List<FoodOptionTypeDto>> GetAllFoodTypes();
    Task<FoodOptionTypeDto?> UpdateFoodType(int id, FoodOptionTypeDto foodTypeDto);
    Task<bool> DeleteFoodType(int id);
}
