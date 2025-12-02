using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using static Cafeteria.Management.Components.Shared.Toast;
using Cafeteria.Management.Components.Pages.FoodOption;
using Cafeteria.Management.Components.Pages.FoodType;

namespace Cafeteria.Management.Components.Pages.FoodOptionType;

public partial class FoodOptionType : ComponentBase
{
    [Inject]
    private IFoodOptionVM FoodOptionVM { get; set; } = default!;

    [Inject]
    private IFoodTypeVM FoodTypeVM { get; set; } = default!;

    private string ActiveTab { get; set; } = "options";

    // Food Options
    public bool IsFoodOptionsInitialized { get; set; } = false;
    private bool ShowFoodOptionModal { get; set; } = false;
    private FoodOptionDto? SelectedFoodOption { get; set; }
    private bool ShowDeleteFoodOptionModal { get; set; } = false;
    private FoodOptionDto? FoodOptionToDelete { get; set; }

    // Food Types
    public bool IsFoodTypesInitialized { get; set; } = false;
    private bool ShowFoodTypeModal { get; set; } = false;
    private FoodOptionTypeDto? SelectedFoodType { get; set; }
    private bool ShowDeleteFoodTypeModal { get; set; } = false;
    private FoodOptionTypeDto? FoodTypeToDelete { get; set; }

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await FoodOptionVM.InitializeFoodOptionsAsync();
        IsFoodOptionsInitialized = true;
    }

    private async Task SetActiveTab(string tab)
    {
        ActiveTab = tab;

        if (tab == "types" && !IsFoodTypesInitialized)
        {
            await FoodTypeVM.InitializeFoodTypesAsync();
            IsFoodTypesInitialized = true;
        }
    }

    #region Food Options Methods

    private void ShowCreateFoodOptionModal()
    {
        SelectedFoodOption = new FoodOptionDto();
        ShowFoodOptionModal = true;
    }

    private void ShowEditFoodOptionModal(FoodOptionDto foodOption)
    {
        SelectedFoodOption = new FoodOptionDto
        {
            Id = foodOption.Id,
            FoodOptionName = foodOption.FoodOptionName,
            InStock = foodOption.InStock,
            ImageUrl = foodOption.ImageUrl
        };
        ShowFoodOptionModal = true;
    }

    private async Task HandleFoodOptionSave()
    {
        var isEdit = SelectedFoodOption?.Id > 0;
        var foodOptionName = SelectedFoodOption?.FoodOptionName ?? "Food Option";

        ShowFoodOptionModal = false;
        SelectedFoodOption = null;

        await FoodOptionVM.InitializeFoodOptionsAsync();

        toastMessage = isEdit
            ? $"'{foodOptionName}' has been updated successfully."
            : $"'{foodOptionName}' has been created successfully.";
        toastType = ToastType.Success;
        showToast = true;

        StateHasChanged();
    }

    private void HandleFoodOptionCancel()
    {
        ShowFoodOptionModal = false;
        SelectedFoodOption = null;
    }

    private void ShowDeleteFoodOptionConfirmation(FoodOptionDto foodOption)
    {
        FoodOptionToDelete = foodOption;
        ShowDeleteFoodOptionModal = true;
    }

    private async Task ConfirmDeleteFoodOption()
    {
        ShowDeleteFoodOptionModal = false;

        if (FoodOptionToDelete != null)
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

            FoodOptionToDelete = null;
        }
    }

    private void CancelDeleteFoodOption()
    {
        ShowDeleteFoodOptionModal = false;
        FoodOptionToDelete = null;
    }

    #endregion

    #region Food Types Methods

    private void ShowCreateFoodTypeModal()
    {
        SelectedFoodType = new FoodOptionTypeDto();
        ShowFoodTypeModal = true;
    }

    private void ShowEditFoodTypeModal(FoodOptionTypeDto foodType)
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
        ShowFoodTypeModal = true;
    }

    private async Task HandleFoodTypeSave()
    {
        ShowFoodTypeModal = false;
        var isEdit = SelectedFoodType?.Id > 0;
        var foodTypeName = SelectedFoodType?.FoodOptionTypeName ?? "Food Type";
        SelectedFoodType = null;

        await FoodTypeVM.InitializeFoodTypesAsync();

        toastMessage = isEdit
            ? $"'{foodTypeName}' has been updated successfully."
            : $"'{foodTypeName}' has been created successfully.";
        toastType = ToastType.Success;
        showToast = true;

        StateHasChanged();
    }

    private void HandleFoodTypeCancel()
    {
        ShowFoodTypeModal = false;
        SelectedFoodType = null;
    }

    private void ShowDeleteFoodTypeConfirmation(FoodOptionTypeDto foodType)
    {
        FoodTypeToDelete = foodType;
        ShowDeleteFoodTypeModal = true;
    }

    private async Task ConfirmDeleteFoodType()
    {
        ShowDeleteFoodTypeModal = false;

        if (FoodTypeToDelete != null)
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

            FoodTypeToDelete = null;
        }
    }

    private void CancelDeleteFoodType()
    {
        ShowDeleteFoodTypeModal = false;
        FoodTypeToDelete = null;
    }

    #endregion
}
