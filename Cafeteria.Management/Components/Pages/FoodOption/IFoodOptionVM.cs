using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public interface IFoodOptionVM
{
    List<FoodOptionDto> FoodOptions { get; }
    Task InitializeFoodOptionsAsync();
    Task<bool> DeleteFoodOptionAsync(int id);
    bool ErrorOccurred();
}
