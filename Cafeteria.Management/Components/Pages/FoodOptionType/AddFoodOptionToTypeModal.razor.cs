using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.FoodOptionType;

public partial class AddFoodOptionToTypeModal : ComponentBase
{
    [Inject]
    private IOptionOptionTypeService OptionOptionTypeService { get; set; } = default!;

    [Parameter]
    public int FoodTypeId { get; set; }

    [Parameter]
    public List<FoodOptionDto> AllOptions { get; set; } = new();

    [Parameter]
    public List<FoodOptionDto> AlreadyAssignedOptions { get; set; } = new();

    [Parameter]
    public EventCallback OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private int SelectedFoodOptionId { get; set; } = 0;
    private bool IsSaving { get; set; } = false;
    private string? ErrorMessage { get; set; }

    private List<FoodOptionDto> AvailableOptions =>
        AllOptions.Where(o => !AlreadyAssignedOptions.Any(a => a.Id == o.Id)).ToList();

    private async Task HandleAdd()
    {
        if (SelectedFoodOptionId == 0)
        {
            ErrorMessage = "Please select a food option.";
            return;
        }

        IsSaving = true;
        ErrorMessage = null;
        StateHasChanged();

        try
        {
            var mapping = new OptionOptionTypeDto
            {
                FoodOptionId = SelectedFoodOptionId,
                FoodOptionTypeId = FoodTypeId
            };

            await OptionOptionTypeService.CreateOptionOptionType(mapping);
            IsSaving = false;
            await OnSave.InvokeAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error adding food option: {ex.Message}";
            IsSaving = false;
            StateHasChanged();
        }
    }

    private async Task Cancel()
    {
        await OnCancel.InvokeAsync();
    }
}
