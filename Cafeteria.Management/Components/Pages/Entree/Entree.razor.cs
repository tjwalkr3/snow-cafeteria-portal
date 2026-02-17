using Microsoft.AspNetCore.Components;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Shared.DTOs.Menu;
using static Cafeteria.Management.Components.Shared.Toast;

namespace Cafeteria.Management.Components.Pages.Entree;

public partial class Entree : ComponentBase
{
    [Inject]
    public IEntreeVM ViewModel { get; set; } = default!;

    [Inject]
    public IStationService StationService { get; set; } = default!;

    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    private CreateOrEditEntree? modalComponent;
    private bool ShowDeleteModal { get; set; } = false;
    private int EntreeIdToDelete { get; set; }
    private string DeleteMessage { get; set; } = string.Empty;

    private bool showToast = false;
    private string toastMessage = string.Empty;
    private ToastType toastType = ToastType.Success;

    private Dictionary<int, string> stationNames = new();
    private List<LocationDto> locations = new();
    private List<StationDto> allStations = new();
    private List<StationDto> filteredStations = new();

    private int? selectedLocationId = null;
    private int? selectedStationId = null;
    private string searchText = string.Empty;

    private List<EntreeDto> FilteredEntrees
    {
        get
        {
            var entrees = ViewModel.Entrees.AsEnumerable();

            if (selectedStationId.HasValue)
            {
                entrees = entrees.Where(e => e.StationId == selectedStationId.Value);
            }
            else if (selectedLocationId.HasValue)
            {
                var stationIdsInLocation = allStations
                    .Where(s => s.LocationId == selectedLocationId.Value)
                    .Select(s => s.Id)
                    .ToList();
                entrees = entrees.Where(e => stationIdsInLocation.Contains(e.StationId));
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                entrees = entrees.Where(e =>
                    e.EntreeName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (e.EntreeDescription != null && e.EntreeDescription.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                );
            }

            return entrees.ToList();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadLocations();
        await LoadStations();
        await ViewModel.LoadEntrees();
    }

    private async Task LoadLocations()
    {
        try
        {
            locations = await LocationService.GetAllLocations();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading locations: {ex.Message}");
        }
    }

    private async Task LoadStations()
    {
        try
        {
            allStations = await StationService.GetAllStations();
            filteredStations = allStations;
            stationNames = allStations.ToDictionary(s => s.Id, s => s.StationName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading stations: {ex.Message}");
        }
    }

    private string GetStationName(int stationId)
    {
        return stationNames.TryGetValue(stationId, out var name) ? name : "Unknown Station";
    }

    private void OnLocationChanged(ChangeEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Value?.ToString()))
        {
            selectedLocationId = null;
            selectedStationId = null;
            filteredStations = allStations;
        }
        else
        {
            selectedLocationId = int.Parse(e.Value.ToString()!);
            selectedStationId = null;
            filteredStations = allStations.Where(s => s.LocationId == selectedLocationId.Value).ToList();
        }
        StateHasChanged();
    }

    private void OnStationChanged(ChangeEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Value?.ToString()))
        {
            selectedStationId = null;
        }
        else
        {
            selectedStationId = int.Parse(e.Value.ToString()!);
        }
        StateHasChanged();
    }

    private void OnSearchChanged(ChangeEventArgs e)
    {
        searchText = e.Value?.ToString() ?? string.Empty;
        StateHasChanged();
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

    private void HandleDeleteClick(int id)
    {
        var entree = ViewModel.Entrees.FirstOrDefault(e => e.Id == id);
        var entreeName = entree?.EntreeName ?? "this entree";

        EntreeIdToDelete = id;
        DeleteMessage = $"Are you sure you want to delete '{entreeName}'? This action cannot be undone.";
        ShowDeleteModal = true;
    }

    private async Task ConfirmDelete()
    {
        ShowDeleteModal = false;

        if (EntreeIdToDelete > 0)
        {
            var entree = ViewModel.Entrees.FirstOrDefault(e => e.Id == EntreeIdToDelete);
            var entreeName = entree?.EntreeName ?? "Entree";

            try
            {
                await ViewModel.DeleteEntree(EntreeIdToDelete);
                await RefreshEntrees();

                toastMessage = $"'{entreeName}' has been deleted successfully.";
                toastType = ToastType.Success;
                showToast = true;
            }
            catch
            {
                toastMessage = $"Failed to delete '{entreeName}'. Please try again.";
                toastType = ToastType.Error;
                showToast = true;
            }
            finally
            {
                EntreeIdToDelete = 0;
            }
        }
    }

    private void CancelDelete()
    {
        ShowDeleteModal = false;
        EntreeIdToDelete = 0;
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

    public void ShowSaveSuccessToast(string entreeName, bool isEdit)
    {
        toastMessage = isEdit
            ? $"'{entreeName}' has been updated successfully."
            : $"'{entreeName}' has been created successfully.";
        toastType = ToastType.Success;
        showToast = true;
        StateHasChanged();
    }

    public void ShowSaveErrorToast(string entreeName, bool isEdit)
    {
        toastMessage = isEdit
            ? $"Failed to update '{entreeName}'. Please try again."
            : $"Failed to create '{entreeName}'. Please try again.";
        toastType = ToastType.Error;
        showToast = true;
        StateHasChanged();
    }
}
