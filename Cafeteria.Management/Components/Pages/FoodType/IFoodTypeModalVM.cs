using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.FoodType;

public interface IFoodTypeModalVM
{
    Task<FoodOptionTypeDto> CreateFoodTypeAsync(FoodOptionTypeDto foodType);
    Task<FoodOptionTypeDto?> UpdateFoodTypeAsync(int id, FoodOptionTypeDto foodType);
}
