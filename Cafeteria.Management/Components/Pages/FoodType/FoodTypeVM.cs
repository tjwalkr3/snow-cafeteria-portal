using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.FoodType;

public class FoodTypeVM : IFoodTypeVM
{
    private readonly IFoodTypeService _foodTypeService;
    private bool initializationFailed = false;

    public List<FoodOptionTypeDto> FoodTypes { get; private set; } = new();

    public FoodTypeVM(IFoodTypeService foodTypeService)
    {
        _foodTypeService = foodTypeService;
    }

    public async Task InitializeFoodTypesAsync()
    {
        try
        {
            FoodTypes = await _foodTypeService.GetAllFoodTypes();
        }
        catch
        {
            initializationFailed = true;
        }
    }

    public async Task<bool> DeleteFoodTypeAsync(int id)
    {
        try
        {
            return await _foodTypeService.DeleteFoodTypeById(id);
        }
        catch
        {
            return false;
        }
    }

    public bool ErrorOccurred()
    {
        return initializationFailed;
    }
}
