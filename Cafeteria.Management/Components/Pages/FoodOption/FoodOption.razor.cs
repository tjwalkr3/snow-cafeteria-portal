using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.FoodOption;

public partial class FoodOption : ComponentBase
{
    [Inject]
    private IFoodOptionVM FoodOptionVM { get; set; } = default!;

    public bool IsInitialized { get; set; } = false;
    private bool ShowModal { get; set; } = false;
    private FoodOptionDto? SelectedFoodOption { get; set; }

    private bool ShowDeleteModal { get; set; } = false;
    private FoodOptionDto? FoodOptionToDelete { get; set; }

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await FoodOptionVM.InitializeFoodOptionsAsync();
        IsInitialized = true;
    }

    private void ShowCreateModal()
    {
        SelectedFoodOption = new FoodOptionDto();
        ShowModal = true;
    }

    private void ShowEditModal(FoodOptionDto foodOption)
    {
        SelectedFoodOption = new FoodOptionDto
        {
            Id = foodOption.Id,
            FoodOptionName = foodOption.FoodOptionName,
            InStock = foodOption.InStock,
            ImageUrl = foodOption.ImageUrl
        };
        ShowModal = true;
    }

    private async Task HandleSave()
    {
        try
        {
            var isEdit = SelectedFoodOption?.Id > 0;
            var foodOptionName = SelectedFoodOption?.FoodOptionName ?? "Food Option";

            await FoodOptionVM.InitializeFoodOptionsAsync();

            toastMessage = isEdit
                ? $"'{foodOptionName}' has been updated successfully."
                : $"'{foodOptionName}' has been created successfully.";
            toastType = ToastType.Success;
            showToast = true;

            ShowModal = false;
            SelectedFoodOption = null;
            StateHasChanged();
        }
        catch
        {
            ShowModal = false;
            SelectedFoodOption = null;
        }
    }

    private void HandleCancel()
    {
        ShowModal = false;
        SelectedFoodOption = null;
    }

    private void ShowDeleteConfirmation(FoodOptionDto foodOption)
    {
        FoodOptionToDelete = foodOption;
        ShowDeleteModal = true;
    }

    private async Task ConfirmDelete()
    {
        ShowDeleteModal = false;

        if (FoodOptionToDelete != null)
        {
            try
            {
                var foodOptionName = FoodOptionToDelete.FoodOptionName;

                if (await FoodOptionVM.DeleteFoodOptionAsync(FoodOptionToDelete.Id))
                {
                    await FoodOptionVM.InitializeFoodOptionsAsync();

                    toastMessage = $"'{foodOptionName}' has been deleted successfully.";
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
                FoodOptionToDelete = null;
            }
        }
    }

    private void CancelDelete()
    {
        ShowDeleteModal = false;
        FoodOptionToDelete = null;
    }
}
