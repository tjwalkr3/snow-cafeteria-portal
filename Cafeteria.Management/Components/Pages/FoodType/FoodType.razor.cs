using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.FoodType;

public partial class FoodType : ComponentBase
{
    [Inject]
    private IFoodTypeVM FoodTypeVM { get; set; } = default!;

    public bool IsInitialized { get; set; } = false;
    private bool ShowModal { get; set; } = false;
    private FoodOptionTypeDto? SelectedFoodType { get; set; }

    private bool ShowDeleteModal { get; set; } = false;
    private FoodOptionTypeDto? FoodTypeToDelete { get; set; }

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await FoodTypeVM.InitializeFoodTypesAsync();
        IsInitialized = true;
    }

    private void ShowCreateModal()
    {
        SelectedFoodType = new FoodOptionTypeDto();
        ShowModal = true;
    }

    private void ShowEditModal(FoodOptionTypeDto foodType)
    {
        SelectedFoodType = new FoodOptionTypeDto
        {
            Id = foodType.Id,
            FoodOptionTypeName = foodType.FoodOptionTypeName,
            NumIncluded = foodType.NumIncluded,
            MaxAmount = foodType.MaxAmount,
            FoodOptionPrice = foodType.FoodOptionPrice,
            EntreeId = foodType.EntreeId,
            SideId = foodType.SideId
        };
        ShowModal = true;
    }

    private async Task HandleSave()
    {
        try
        {
            var isEdit = SelectedFoodType?.Id > 0;
            var foodTypeName = SelectedFoodType?.FoodOptionTypeName ?? "Food Type";

            await FoodTypeVM.InitializeFoodTypesAsync();

            toastMessage = isEdit
                ? $"'{foodTypeName}' has been updated successfully."
                : $"'{foodTypeName}' has been created successfully.";
            toastType = ToastType.Success;
            showToast = true;

            ShowModal = false;
            SelectedFoodType = null;
            StateHasChanged();
        }
        catch
        {
            ShowModal = false;
            SelectedFoodType = null;
        }
    }

    private void HandleCancel()
    {
        ShowModal = false;
        SelectedFoodType = null;
    }

    private void ShowDeleteConfirmation(FoodOptionTypeDto foodType)
    {
        FoodTypeToDelete = foodType;
        ShowDeleteModal = true;
    }

    private async Task ConfirmDelete()
    {
        ShowDeleteModal = false;

        if (FoodTypeToDelete != null)
        {
            try
            {
                var foodTypeName = FoodTypeToDelete.FoodOptionTypeName;

                if (await FoodTypeVM.DeleteFoodTypeAsync(FoodTypeToDelete.Id))
                {
                    await FoodTypeVM.InitializeFoodTypesAsync();

                    toastMessage = $"'{foodTypeName}' has been deleted successfully.";
                    toastType = ToastType.Success;
                    showToast = true;

                    StateHasChanged();
                }
            }
            catch
            {
                // Silently handle errors to prevent circuit crashes
            }
            finally
            {
                FoodTypeToDelete = null;
            }
        }
    }

    private void CancelDelete()
    {
        ShowDeleteModal = false;
        FoodTypeToDelete = null;
    }
}
