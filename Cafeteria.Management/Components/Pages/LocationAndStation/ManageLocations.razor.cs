using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Components.Shared;
using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Management.Components.Pages.LocationAndStation.Station;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class ManageLocations : ComponentBase
{
    [Inject]
    public IManageLocationVM ViewModel { get; set; } = default!;

    private CreateOrEditLocation? locationModalComponent;
    private CreateOrEditStation? stationModalComponent;

    private bool showDeleteLocationModal = false;
    private int locationIdToDelete = 0;
    private string deleteLocationMessage = string.Empty;

    private bool showDeleteStationModal = false;
    private int stationIdToDelete = 0;
    private string deleteStationMessage = string.Empty;

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadLocations();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            locationModalComponent?.SetParentComponent(this);
            stationModalComponent?.SetParentComponent(this);
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleCreateLocationClick()
    {
        await ViewModel.ShowCreateLocationModal();
        locationModalComponent?.Refresh();
    }

    private async Task HandleEditLocationClick(int id)
    {
        await ViewModel.ShowEditLocationModal(id);
        locationModalComponent?.Refresh();
    }

    private void HandleDeleteLocationClick(int id, string name)
    {
        locationIdToDelete = id;
        deleteLocationMessage = $"Are you sure you want to delete '{name}'? This will also delete all stations in this location. This action cannot be undone.";
        showDeleteLocationModal = true;
    }

    private async Task ConfirmDeleteLocation()
    {
        showDeleteLocationModal = false;

        if (locationIdToDelete > 0)
        {
            var location = ViewModel.Locations.FirstOrDefault(l => l.Id == locationIdToDelete);
            var locationName = location?.LocationName ?? "Location";

            try
            {
                await ViewModel.DeleteLocation(locationIdToDelete);
                await RefreshLocations();

                toastMessage = $"'{locationName}' has been deleted successfully.";
                toastType = ToastType.Success;
                showToast = true;
            }
            catch
            {
                toastMessage = $"Failed to delete '{locationName}'. Please try again.";
                toastType = ToastType.Error;
                showToast = true;
            }
            finally
            {
                locationIdToDelete = 0;
            }
        }
    }

    private void CancelDeleteLocation()
    {
        showDeleteLocationModal = false;
        locationIdToDelete = 0;
    }

    private async Task HandleCreateStationClick(int locationId)
    {
        await ViewModel.ShowCreateStationModal(locationId);
        stationModalComponent?.Refresh();
    }

    private async Task HandleEditStationClick(int stationId)
    {
        await ViewModel.ShowEditStationModal(stationId);
        stationModalComponent?.Refresh();
    }

    private void HandleDeleteStationClick(int stationId, string name)
    {
        stationIdToDelete = stationId;
        deleteStationMessage = $"Are you sure you want to delete station '{name}'? This action cannot be undone.";
        showDeleteStationModal = true;
    }

    private async Task ConfirmDeleteStation()
    {
        showDeleteStationModal = false;

        if (stationIdToDelete > 0)
        {
            string stationName = "Station";
            foreach (var stations in ViewModel.StationsByLocation.Values)
            {
                var s = stations.FirstOrDefault(st => st.Id == stationIdToDelete);
                if (s != null) { stationName = s.StationName; break; }
            }

            try
            {
                await ViewModel.DeleteStation(stationIdToDelete);
                await RefreshLocations();

                toastMessage = $"'{stationName}' has been deleted successfully.";
                toastType = ToastType.Success;
                showToast = true;
            }
            catch
            {
                toastMessage = $"Failed to delete '{stationName}'. Please try again.";
                toastType = ToastType.Error;
                showToast = true;
            }
            finally
            {
                stationIdToDelete = 0;
            }
        }
    }

    private void CancelDeleteStation()
    {
        showDeleteStationModal = false;
        stationIdToDelete = 0;
    }

    private async Task RefreshLocations()
    {
        await ViewModel.LoadLocations();
        StateHasChanged();
    }

    public async Task RefreshLocationsAfterSave()
    {
        await RefreshLocations();
    }

    public void ShowSaveSuccessToast(string name, bool isEdit)
    {
        toastMessage = isEdit
            ? $"'{name}' has been updated successfully."
            : $"'{name}' has been created successfully.";
        toastType = ToastType.Success;
        showToast = true;
        StateHasChanged();
    }

    public void ShowSaveErrorToast(string name, bool isEdit)
    {
        toastMessage = isEdit
            ? $"Failed to update '{name}'. Please try again."
            : $"Failed to create '{name}'. Please try again.";
        toastType = ToastType.Error;
        showToast = true;
        StateHasChanged();
    }
}
