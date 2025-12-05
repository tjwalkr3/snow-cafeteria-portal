using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Drink;

public partial class Drink : ComponentBase
{
    [Inject]
    public IDrinkVM ViewModel { get; set; } = default!;

    private CreateOrEditDrink? modalComponent;
    private bool ShowDeleteModal { get; set; } = false;
    private int DrinkIdToDelete { get; set; }
    private string DeleteMessage { get; set; } = string.Empty;

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
            try
            {
                await ViewModel.DeleteDrink(DrinkIdToDelete);
                await RefreshDrinks();
            }
            catch
            {
                // Silently handle errors to prevent circuit crashes
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
}
