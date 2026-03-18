using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services.Locations;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public partial class CreateOrEditLocation : ComponentBase
{
    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    [Inject]
    public IManageLocationVM ParentVM { get; set; } = default!;

    public ICreateOrEditLocationVM? ViewModel { get; set; }
    private ManageLocations? parentComponent;

    protected override Task OnInitializedAsync()
    {
        ViewModel = new CreateOrEditLocationVM(LocationService, ParentVM);
        ParentVM.SetCreateOrEditLocationVM(ViewModel);
        return base.OnInitializedAsync();
    }

    private async Task HandleSave()
    {
        if (ViewModel != null && parentComponent != null)
        {
            var locationName = ViewModel.CurrentLocation.LocationName ?? "Location";
            var isEdit = ViewModel.IsEditing;

            try
            {
                var success = await ViewModel.SaveLocation();
                if (success)
                {
                    StateHasChanged();
                    await parentComponent.RefreshLocationsAfterSave();
                    parentComponent.ShowSaveSuccessToast(locationName, isEdit);
                }
            }
            catch
            {
                parentComponent.ShowSaveErrorToast(locationName, isEdit);
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
