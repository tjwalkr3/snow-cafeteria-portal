using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.FoodOptionTypes;

public interface IFoodOptionTypeService
{
    Task<FoodOptionTypeDto> CreateFoodType(FoodOptionTypeDto foodTypeDto);
    Task<FoodOptionTypeDto?> GetFoodTypeByID(int id);
    Task<List<FoodOptionTypeDto>> GetAllFoodTypes();
    Task<FoodOptionTypeDto?> UpdateFoodTypeById(int id, FoodOptionTypeDto foodTypeDto);
    Task<bool> DeleteFoodTypeById(int id);
    Task<List<EntreeDto>> GetAllEntrees();
    Task<List<SideDto>> GetAllSides();
}
