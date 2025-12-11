using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class CreateOrEditSide : ComponentBase
{
    [Inject]
    public ISideService SideService { get; set; } = default!;

    [Inject]
    public ISideVM ParentVM { get; set; } = default!;

    [Inject]
    public IStationService StationService { get; set; } = default!;

    public ICreateOrEditSideVM? ViewModel { get; set; }
    private Side? parentComponent;

    protected override async Task OnInitializedAsync()
    {
        ViewModel = new CreateOrEditSideVM(SideService, ParentVM, StationService);
        if (ParentVM is SideVM sideVM)
        {
            sideVM.SetCreateOrEditVM(ViewModel);
        }

        if (ViewModel is CreateOrEditSideVM vm)
        {
            await vm.LoadStations();
        }
    }

    private async Task HandleSave()
    {
        if (ViewModel != null && parentComponent != null)
        {
            var sideName = ViewModel.CurrentSide.SideName ?? "Side";
            var isEdit = ViewModel.IsEditing;

            try
            {
                var success = await ViewModel.SaveSide();
                if (success)
                {
                    StateHasChanged();
                    await parentComponent.RefreshSidesAfterSave();
                    parentComponent.ShowSaveSuccessToast(sideName, isEdit);
                }
            }
            catch
            {
                parentComponent.ShowSaveErrorToast(sideName, isEdit);
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
