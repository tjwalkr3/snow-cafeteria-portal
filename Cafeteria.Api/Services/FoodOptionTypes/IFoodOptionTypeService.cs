using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.FoodOptionTypes;

public interface IFoodOptionTypeService
{
    Task<FoodOptionTypeDto> CreateFoodOptionType(FoodOptionTypeDto foodTypeDto);
    Task<FoodOptionTypeDto?> GetFoodOptionTypeByID(int id);
    Task<List<FoodOptionTypeDto>> GetAllFoodOptionTypes();
    Task<List<FoodOptionTypeDto>> GetFoodOptionTypesByEntreeId(int entreeId);
    Task<List<FoodOptionTypeWithOptionsDto>> GetFoodOptionTypesWithOptionsByEntreeId(int entreeId);
    Task<FoodOptionTypeDto?> UpdateFoodOptionTypeById(int id, FoodOptionTypeDto foodTypeDto);
    Task<bool> DeleteFoodOptionTypeById(int id);
}
