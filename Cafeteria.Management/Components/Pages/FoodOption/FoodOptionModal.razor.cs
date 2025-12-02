using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public partial class FoodOptionModal : ComponentBase
{
    [Inject]
    private IFoodOptionModalVM FoodOptionModalVM { get; set; } = default!;

    [Parameter]
    public FoodOptionDto FoodOption { get; set; } = new();

    [Parameter]
    public EventCallback OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private bool IsEditMode => FoodOption.Id > 0;
    private bool IsSaving { get; set; } = false;
    private string? ErrorMessage { get; set; }

    private async Task HandleSubmit()
    {
        IsSaving = true;
        ErrorMessage = null;
        StateHasChanged();

        try
        {
            if (IsEditMode)
            {
                await FoodOptionModalVM.UpdateFoodOptionAsync(FoodOption.Id, FoodOption);
            }
            else
            {
                await FoodOptionModalVM.CreateFoodOptionAsync(FoodOption);
            }

            IsSaving = false;
            await OnSave.InvokeAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving food option: {ex.Message}";
            IsSaving = false;
            StateHasChanged();
        }
    }

    private async Task Cancel()
    {
        await OnCancel.InvokeAsync();
    }
}
