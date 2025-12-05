using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.Drink;

public partial class Drink : ComponentBase
{
    [Inject]
    public IDrinkVM ViewModel { get; set; } = default!;

    private CreateOrEditDrink? modalComponent;
    private bool ShowDeleteModal { get; set; } = false;
    private int DrinkIdToDelete { get; set; }
    private string DeleteMessage { get; set; } = string.Empty;

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadDrinks();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && modalComponent != null)
        {
            modalComponent.SetParentComponent(this);
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleCreateClick()
    {
        await ViewModel.ShowCreateModal();
        modalComponent?.Refresh();
    }

    private async Task HandleEditClick(int id)
    {
        await ViewModel.ShowEditModal(id);
        modalComponent?.Refresh();
    }

    private void HandleDeleteClick(int id)
    {
        var drink = ViewModel.Drinks.FirstOrDefault(d => d.Id == id);
        var drinkName = drink?.DrinkName ?? "this drink";

        DrinkIdToDelete = id;
        DeleteMessage = $"Are you sure you want to delete '{drinkName}'? This action cannot be undone.";
        ShowDeleteModal = true;
    }

    private async Task ConfirmDelete()
    {
        ShowDeleteModal = false;

        if (DrinkIdToDelete > 0)
        {
            var drink = ViewModel.Drinks.FirstOrDefault(d => d.Id == DrinkIdToDelete);
            var drinkName = drink?.DrinkName ?? "Drink";

            try
            {
                await ViewModel.DeleteDrink(DrinkIdToDelete);
                await RefreshDrinks();

                toastMessage = $"'{drinkName}' has been deleted successfully.";
                toastType = ToastType.Success;
                showToast = true;
            }
            catch
            {
                toastMessage = $"Failed to delete '{drinkName}'. Please try again.";
                toastType = ToastType.Error;
                showToast = true;
            }
            finally
            {
                DrinkIdToDelete = 0;
            }
        }
    }

    private void CancelDelete()
    {
        ShowDeleteModal = false;
        DrinkIdToDelete = 0;
    }

    private async Task RefreshDrinks()
    {
        await ViewModel.LoadDrinks();
        StateHasChanged();
    }

    public async Task RefreshDrinksAfterSave()
    {
        await RefreshDrinks();
    }

    public void ShowSaveSuccessToast(string drinkName, bool isEdit)
    {
        toastMessage = isEdit
            ? $"'{drinkName}' has been updated successfully."
            : $"'{drinkName}' has been created successfully.";
        toastType = ToastType.Success;
        showToast = true;
        StateHasChanged();
    }

    public void ShowSaveErrorToast(string drinkName, bool isEdit)
    {
        toastMessage = isEdit
            ? $"Failed to update '{drinkName}'. Please try again."
            : $"Failed to create '{drinkName}'. Please try again.";
        toastType = ToastType.Error;
        showToast = true;
        StateHasChanged();
    }
}
