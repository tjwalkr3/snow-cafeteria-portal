using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.FoodType;

public class FoodTypeModalVM : IFoodTypeModalVM
{
    private readonly IFoodTypeService _foodTypeService;

    public FoodTypeModalVM(IFoodTypeService foodTypeService)
    {
        _foodTypeService = foodTypeService;
    }

    public async Task<FoodOptionTypeDto> CreateFoodTypeAsync(FoodOptionTypeDto foodType)
    {
        return await _foodTypeService.CreateFoodType(foodType);
    }

    public async Task<FoodOptionTypeDto?> UpdateFoodTypeAsync(int id, FoodOptionTypeDto foodType)
    {
        return await _foodTypeService.UpdateFoodTypeById(id, foodType);
    }
}
