using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public class FoodOptionModalVM : IFoodOptionModalVM
{
    private readonly IFoodOptionService _foodOptionService;

    public FoodOptionModalVM(IFoodOptionService foodOptionService)
    {
        _foodOptionService = foodOptionService;
    }

    public async Task<FoodOptionDto> CreateFoodOptionAsync(FoodOptionDto foodOption)
    {
        return await _foodOptionService.CreateFoodOption(foodOption);
    }

    public async Task<FoodOptionDto?> UpdateFoodOptionAsync(int id, FoodOptionDto foodOption)
    {
        return await _foodOptionService.UpdateFoodOptionById(id, foodOption);
    }
}
