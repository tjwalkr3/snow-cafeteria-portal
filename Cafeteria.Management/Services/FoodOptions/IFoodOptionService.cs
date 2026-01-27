using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.FoodOptions;

public interface IFoodOptionService
{
    Task<FoodOptionDto> CreateFoodOption(FoodOptionDto foodOptionDto);
    Task<FoodOptionDto?> GetFoodOptionByID(int id);
    Task<List<FoodOptionDto>> GetAllFoodOptions();
    Task<FoodOptionDto?> UpdateFoodOptionById(int id, FoodOptionDto foodOptionDto);
    Task<bool> DeleteFoodOptionById(int id);
}
