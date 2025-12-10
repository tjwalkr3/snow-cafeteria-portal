using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class CreateOrEditSide : ComponentBase
{
    [Inject]
    public ICreateOrEditSideVM ViewModel { get; set; } = default!;

    [Inject]
    public ISideVM SideVM { get; set; } = default!;

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

    private bool ShowToast;
    private string ToastMessage = string.Empty;
    private Toast.ToastType ToastType;

    protected override async Task OnInitializedAsync()
    {
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
            SideModel.StationId = 0;
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
        if (!ViewModel.ValidateSide(SideVM.Sides, SideModel))
        {
            ShowToast = true;
            ToastMessage = "A side with this name already exists in this station.";
            ToastType = Toast.ToastType.Error;
            return;
        }

        IsSaving = true;
        ErrorMessage = null;
        try
        {
            if (IsEditMode)
            {
                await ViewModel.UpdateSideAsync(SideModel);
            }
            else
            {
                await ViewModel.CreateSideAsync(SideModel);
            }
            await OnSave.InvokeAsync();
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
