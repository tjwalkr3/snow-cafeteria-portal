using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.FoodType;

public interface IFoodTypeVM
{
    List<FoodOptionTypeDto> FoodTypes { get; }
    Task InitializeFoodTypesAsync();
    Task<bool> DeleteFoodTypeAsync(int id);
    bool ErrorOccurred();
}
