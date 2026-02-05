using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.FoodOptions;

public interface IFoodOptionService
{
    Task<FoodOptionDto> CreateFoodOption(FoodOptionDto foodOptionDto);
    Task<FoodOptionDto?> GetFoodOptionById(int id);
    Task<List<FoodOptionDto>> GetAllFoodOptions();
    Task<List<FoodOptionDto>> GetFoodOptionsByEntreeId(int entreeId);
    Task<List<FoodOptionDto>> GetFoodOptionsBySideId(int sideId);
    Task<FoodOptionDto?> UpdateFoodOptionById(int id, FoodOptionDto foodOptionDto);
    Task<bool> DeleteFoodOptionById(int id);
}
