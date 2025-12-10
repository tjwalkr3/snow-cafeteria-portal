using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class CreateOrEditSide : ComponentBase
{
    [Inject]
    public ISideService SideService { get; set; } = default!;

    [Inject]
    public ISideVM ParentVM { get; set; } = default!;

    public ICreateOrEditSideVM? ViewModel { get; set; }
    private Side? parentComponent;

    protected override void OnInitialized()
    {
        ViewModel = new CreateOrEditSideVM(SideService, ParentVM);
        if (ParentVM is SideVM sideVM)
        {
            sideVM.SetCreateOrEditVM(ViewModel);
        }
    }

    private async Task HandleSave()
    {
        if (ViewModel != null)
        {
            await ViewModel.SaveSide();
            StateHasChanged();
            if (parentComponent != null)
            {
                await parentComponent.RefreshSidesAfterSave();
            }
        }
    }

    private void HandleClose()
    {
        if (ViewModel != null)
        {
            ViewModel.IsVisible = false;
            StateHasChanged();
        }
    }

    public void Refresh()
    {
        StateHasChanged();
    }

    public void SetParentComponent(Side parent)
    {
        parentComponent = parent;
    }
}
