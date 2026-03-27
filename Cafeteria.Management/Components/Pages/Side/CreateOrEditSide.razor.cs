using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services.Sides;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class CreateOrEditSide : ComponentBase
{
    [Inject]
    public ISideService SideService { get; set; } = default!;

    [Inject]
    public ISideVM ParentVM { get; set; } = default!;

    [Inject]
    public IStationService StationService { get; set; } = default!;

    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    public ICreateOrEditSideVM? ViewModel { get; set; }
    private Side? parentComponent;

    protected override async Task OnInitializedAsync()
    {
        ViewModel = new CreateOrEditSideVM(SideService, ParentVM, StationService, LocationService);
        if (ParentVM is SideVM sideVM)
        {
            sideVM.SetCreateOrEditVM(ViewModel);
        }

        if (ViewModel is CreateOrEditSideVM vm)
        {
            await vm.LoadStations();
            await vm.LoadLocations();
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

    private void SetItemType(SideDto side, string itemType)
    {
        side.CardOnly = false;
        side.SwipeOnly = false;

        switch (itemType)
        {
            case "cardonly":
                side.CardOnly = true;
                break;
            case "swipeonly":
                side.SwipeOnly = true;
                break;
            case "both":
            default:
                // Both means neither flag is set
                break;
        }
    }

    private void OnLocationChanged(int locationId)
    {
        if (ViewModel != null)
        {
            ViewModel.SelectedLocationId = locationId;
            // Reset station selection when location changes
            ViewModel.CurrentSide.StationId = 0;
            StateHasChanged();
        }
    }

    private void OnLocationSelectionChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out int locationId))
        {
            OnLocationChanged(locationId);
        }
    }
}
