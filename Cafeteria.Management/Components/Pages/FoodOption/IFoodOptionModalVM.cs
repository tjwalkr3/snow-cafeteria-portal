using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public interface IFoodOptionModalVM
{
    Task<FoodOptionDto> CreateFoodOptionAsync(FoodOptionDto foodOption);
    Task<FoodOptionDto?> UpdateFoodOptionAsync(int id, FoodOptionDto foodOption);
}
