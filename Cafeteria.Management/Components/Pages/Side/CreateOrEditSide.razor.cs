using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class CreateOrEditSide : ComponentBase
{
    [Inject]
    public ICreateOrEditSideVM ViewModel { get; set; } = default!;

    [Parameter]
    public SideDto SideModel { get; set; } = new();

    [Parameter]
    public bool IsEditMode { get; set; }

    [Parameter]
    public EventCallback OnSave { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    private List<LocationDto> Locations = new();
    private List<StationDto> Stations = new();
    private int SelectedLocationId;
    private bool IsSaving;
    private string? ErrorMessage;

    protected override async Task OnInitializedAsync()
    {
        ViewModel.CurrentSide = SideModel;
        ViewModel.IsEditMode = IsEditMode;
        ViewModel.ShowToast = false;

        Locations = await ViewModel.GetLocationsAsync();

        if (IsEditMode && SideModel.StationId > 0)
        {
            var station = await ViewModel.GetStationByIdAsync(SideModel.StationId);
            if (station != null)
            {
                SelectedLocationId = station.LocationId;
                await LoadStations(SelectedLocationId);
            }
        }
    }

    private async Task OnLocationChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out int locationId))
        {
            SelectedLocationId = locationId;
            await LoadStations(locationId);
            ViewModel.CurrentSide.StationId = 0;
        }
    }

    private async Task LoadStations(int locationId)
    {
        if (locationId > 0)
        {
            Stations = await ViewModel.GetStationsByLocationAsync(locationId);
        }
        else
        {
            Stations.Clear();
        }
    }

    private async Task HandleValidSubmit()
    {
        IsSaving = true;
        ErrorMessage = null;
        try
        {
            bool success = await ViewModel.SaveSideAsync();
            if (success)
            {
                await OnSave.InvokeAsync();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsSaving = false;
        }
    }
}
