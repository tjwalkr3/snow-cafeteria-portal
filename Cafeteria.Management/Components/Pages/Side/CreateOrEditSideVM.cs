using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public class CreateOrEditSideVM : ICreateOrEditSideVM
{
    private readonly ISideService _sideService;
    private readonly ILocationService _locationService;
    private readonly IStationService _stationService;
    private readonly ISideVM _sideVM;

    public SideDto CurrentSide { get; set; } = new();
    public bool IsEditMode { get; set; }
    public bool ShowToast { get; set; }
    public string ToastMessage { get; set; } = string.Empty;
    public Toast.ToastType ToastType { get; set; }

    public CreateOrEditSideVM(ISideService sideService, ILocationService locationService, IStationService stationService, ISideVM sideVM)
    {
        _sideService = sideService;
        _locationService = locationService;
        _stationService = stationService;
        _sideVM = sideVM;
    }

    public async Task<bool> SaveSideAsync()
    {
        if (!ValidateSide(_sideVM.Sides, CurrentSide))
        {
            ShowToast = true;
            ToastMessage = "A side with this name already exists in this station.";
            ToastType = Toast.ToastType.Error;
            return false;
        }

        try
        {
            if (IsEditMode)
            {
                await _sideService.UpdateSide(CurrentSide);
            }
            else
            {
                await _sideService.CreateSide(CurrentSide);
            }
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving side: {ex.Message}");
            throw;
        }
    }

    public async Task<List<LocationDto>> GetLocationsAsync()
    {
        return await _locationService.GetAllLocations();
    }

    public async Task<List<StationDto>> GetStationsByLocationAsync(int locationId)
    {
        return await _stationService.GetStationsByLocation(locationId);
    }

    public async Task<StationDto?> GetStationByIdAsync(int stationId)
    {
        return await _stationService.GetStationById(stationId);
    }

    public bool ValidateSide(IEnumerable<SideDto> existingSides, SideDto newSide)
    {
        return !existingSides.Any(s => 
            s.SideName.Equals(newSide.SideName, StringComparison.OrdinalIgnoreCase) && 
            s.StationId == newSide.StationId &&
            s.Id != newSide.Id);
    }
}
