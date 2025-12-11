using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Cafeteria.Management.Components.Pages.Side;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class Side : ComponentBase
{
    [Inject]
    public ISideVM ViewModel { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private CreateOrEditSide? modalComponent;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadSides();
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
            await ViewModel.DeleteSide(id);
            await RefreshSides();
        }
    }

    private async Task RefreshSides()
    {
        await ViewModel.LoadSides();
        StateHasChanged();
    }

    public async Task RefreshSidesAfterSave()
    {
        await RefreshSides();
    }

    private async Task<bool> ConfirmDelete()
    {
        return await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this side?");
    }
}
