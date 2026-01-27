using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.FoodOptions;

public interface IFoodOptionService
{
    Task<FoodOptionDto> CreateFoodOption(FoodOptionDto foodOptionDto);
    Task<FoodOptionDto?> GetFoodOptionByID(int id);
    Task<List<FoodOptionDto>> GetAllFoodOptions();
    Task<List<FoodOptionDto>> GetOptionsByEntree(int entreeId);
    Task<List<FoodOptionDto>> GetOptionsBySide(int sideId);
    Task<FoodOptionDto?> UpdateFoodOption(int id, FoodOptionDto foodOptionDto);
    Task<bool> DeleteFoodOption(int id);
}
