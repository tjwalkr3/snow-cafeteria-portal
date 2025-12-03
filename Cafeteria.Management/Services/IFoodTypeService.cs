using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface IFoodTypeService
{
    Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto);
    Task<FoodOptionTypeDto?> GetFoodTypeByID(int id);
    Task<List<FoodOptionTypeDto>> GetAllFoodTypes();
    Task<FoodOptionTypeDto?> UpdateFoodTypeById(int id, FoodOptionTypeDto foodTypeDto);
    Task<bool> DeleteFoodTypeById(int id);
}
