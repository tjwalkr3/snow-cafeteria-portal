using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public class OptionOptionTypeVM : IOptionOptionTypeVM
{
    private readonly IOptionOptionTypeService _optionOptionTypeService;
    private bool initializationFailed = false;

    public List<OptionOptionTypeDto> OptionOptionTypes { get; private set; } = new();

    public OptionOptionTypeVM(IOptionOptionTypeService optionOptionTypeService)
    {
        _optionOptionTypeService = optionOptionTypeService;
    }

    public async Task InitializeOptionOptionTypesAsync()
    {
        try
        {
            OptionOptionTypes = await _optionOptionTypeService.GetAllOptionOptionTypes();
        }
        catch
        {
            initializationFailed = true;
        }
    }

    public async Task<bool> DeleteOptionOptionTypeAsync(int id)
    {
        try
        {
            return await _optionOptionTypeService.DeleteOptionOptionTypeById(id);
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
