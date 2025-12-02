using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Components.Pages.Drink;

namespace Cafeteria.Management.Components.Pages.Drink;

public partial class Drink : ComponentBase
{
    [Inject]
    public IDrinkVM ViewModel { get; set; } = default!;

    private CreateOrEditDrink? modalComponent;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadDrinks();
    }

    private async Task HandleCreateClick()
    {
        await ViewModel.ShowCreateModal();
        StateHasChanged();
    }

    private async Task HandleEditClick(int id)
    {
        await ViewModel.ShowEditModal(id);
        StateHasChanged();
    }

    private async Task HandleDeleteClick(int id)
    {
        if (await ConfirmDelete())
        {
            await ViewModel.DeleteDrink(id);
            StateHasChanged();
        }
    }

    private Task<bool> ConfirmDelete()
    {
        // In a minimal implementation, using JS confirm
        return Task.FromResult(true); // In real app, use JS interop for confirmation
    }
}
