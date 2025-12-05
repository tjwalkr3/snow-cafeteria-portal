using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public class FoodOptionVM : IFoodOptionVM
{
    private readonly IFoodOptionService _foodOptionService;
    private bool initializationFailed = false;

    public List<FoodOptionDto> FoodOptions { get; private set; } = new();

    public FoodOptionVM(IFoodOptionService foodOptionService)
    {
        _foodOptionService = foodOptionService;
    }

    public async Task InitializeFoodOptionsAsync()
    {
        try
        {
            FoodOptions = await _foodOptionService.GetAllFoodOptions();
        }
        catch
        {
            initializationFailed = true;
        }
    }

    public async Task<bool> DeleteFoodOptionAsync(int id)
    {
        try
        {
            return await _foodOptionService.DeleteFoodOptionById(id);
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
