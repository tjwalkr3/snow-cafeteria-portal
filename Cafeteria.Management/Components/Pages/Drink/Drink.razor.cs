using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Cafeteria.Management.Components.Pages.Drink;

namespace Cafeteria.Management.Components.Pages.Drink;

public partial class Drink : ComponentBase
{
    [Inject]
    public IDrinkVM ViewModel { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private CreateOrEditDrink? modalComponent;

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

    private async Task HandleDeleteClick(int id)
    {
        if (await ConfirmDelete())
        {
            await ViewModel.DeleteDrink(id);
            await RefreshDrinks();
        }
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

    private async Task<bool> ConfirmDelete()
    {
        return await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this drink?");
    }
}
