using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.FoodType;

public partial class FoodTypeModal : ComponentBase
{
    [Inject]
    private IFoodTypeModalVM FoodTypeModalVM { get; set; } = default!;

    [Parameter]
    public FoodOptionTypeDto FoodType { get; set; } = new();

    [Parameter]
    public EventCallback OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private bool IsEditMode => FoodType.Id > 0;
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
                await FoodTypeModalVM.UpdateFoodTypeAsync(FoodType.Id, FoodType);
            }
            else
            {
                await FoodTypeModalVM.CreateFoodTypeAsync(FoodType);
            }

            IsSaving = false;
            await OnSave.InvokeAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving food type: {ex.Message}";
            IsSaving = false;
            StateHasChanged();
        }
    }

    private async Task Cancel()
    {
        await OnCancel.InvokeAsync();
    }
}
