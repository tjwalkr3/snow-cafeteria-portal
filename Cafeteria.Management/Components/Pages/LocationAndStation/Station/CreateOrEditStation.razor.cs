using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Components.Pages.LocationAndStation;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Station;

public partial class CreateOrEditStation : ComponentBase
{
    [Inject]
    public IStationService StationService { get; set; } = default!;

    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    [Inject]
    public IManageLocationVM ParentVM { get; set; } = default!;

    public ICreateOrEditStationVM? ViewModel { get; set; }
    private ManageLocations? parentComponent;

    protected override Task OnInitializedAsync()
    {
        ViewModel = new CreateOrEditStationVM(StationService, LocationService, ParentVM);
        ParentVM.SetCreateOrEditStationVM(ViewModel);
        return base.OnInitializedAsync();
    }

    private async Task HandleSave()
    {
        if (ViewModel != null && parentComponent != null)
        {
            var stationName = ViewModel.CurrentStation.StationName ?? "Station";
            var isEdit = ViewModel.IsEditing;

            try
            {
                var success = await ViewModel.SaveStation();
                if (success)
                {
                    StateHasChanged();
                    await parentComponent.RefreshLocationsAfterSave();
                    parentComponent.ShowSaveSuccessToast(stationName, isEdit);
                }
            }
            catch
            {
                parentComponent.ShowSaveErrorToast(stationName, isEdit);
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

    public void SetParentComponent(ManageLocations parent)
    {
        parentComponent = parent;
    }
}
