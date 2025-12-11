using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Services;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public class CreateOrEditSideVM : ICreateOrEditSideVM
{
    private readonly ISideService _sideService;
    private readonly ISideVM _parentVM;
    private readonly IStationService _stationService;

    public SideDto CurrentSide { get; set; } = new();
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }
    public bool ShowToast { get; set; }
    public string ToastMessage { get; set; } = string.Empty;
    public Toast.ToastType ToastType { get; set; }
    public List<StationDto> Stations { get; set; } = [];

    public CreateOrEditSideVM(ISideService sideService, ISideVM parentVM, IStationService stationService)
    {
        _sideService = sideService;
        _parentVM = parentVM;
        _stationService = stationService;
    }

    public async Task LoadStations()
    {
        try
        {
            Stations = await _stationService.GetAllStations();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading stations: {ex.Message}");
        }
    }

    public async Task<bool> SaveSide()
    {
        if (!ValidateSide(_parentVM.Sides, CurrentSide))
        {
            ShowToast = true;
            ToastMessage = "A side with this name already exists in this station.";
            ToastType = Toast.ToastType.Error;
            return false;
        }

        try
        {
            if (IsEditing)
            {
                await _sideService.UpdateSide(CurrentSide);
            }
            else
            {
                await _sideService.CreateSide(CurrentSide);
            }

            IsVisible = false;
            await _parentVM.LoadSides();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving side: {ex.Message}");
            throw;
        }
    }

    public bool ValidateSide(IEnumerable<SideDto> existingSides, SideDto newSide)
    {
        return !existingSides.Any(s =>
            s.SideName.Equals(newSide.SideName, StringComparison.OrdinalIgnoreCase) &&
            s.StationId == newSide.StationId &&
            s.Id != newSide.Id);
    }
}
