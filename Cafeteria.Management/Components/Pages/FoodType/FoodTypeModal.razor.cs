using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.FoodType;

public partial class FoodTypeModal : ComponentBase
{
    [Inject]
    private IFoodTypeModalVM FoodTypeModalVM { get; set; } = default!;

    [Inject]
    private IFoodTypeService FoodTypeService { get; set; } = default!;

    [Parameter]
    public FoodOptionTypeDto FoodType { get; set; } = new();

    [Parameter]
    public EventCallback OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private bool IsEditMode => FoodType.Id > 0;
    private bool IsSaving { get; set; } = false;
    private string? ErrorMessage { get; set; }
    private List<EntreeDto> Entrees { get; set; } = new();
    private List<SideDto> Sides { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Entrees = await FoodTypeService.GetAllEntrees();
            Sides = await FoodTypeService.GetAllSides();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading data: {ex.Message}";
        }
    }

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
