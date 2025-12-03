using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Components.Pages.Entree;

namespace Cafeteria.Management.Components.Pages.Entree;

public partial class Entree : ComponentBase
{
    [Inject]
    public IEntreeVM ViewModel { get; set; } = default!;

    private CreateOrEditEntree? modalComponent;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadEntrees();
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

    private async Task HandleDeleteClick(int id)
    {
        await ViewModel.DeleteEntree(id);
        await RefreshEntrees();
    }

    private async Task RefreshEntrees()
    {
        await ViewModel.LoadEntrees();
        StateHasChanged();
    }

    public async Task RefreshEntreesAfterSave()
    {
        await RefreshEntrees();
    }
}
