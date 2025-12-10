using Cafeteria.Shared.DTOs;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public interface ICreateOrEditSideVM
{
    SideDto CurrentSide { get; set; }
    bool IsEditMode { get; set; }
    bool ShowToast { get; set; }
    string ToastMessage { get; set; }
    Toast.ToastType ToastType { get; set; }

    Task<bool> SaveSideAsync();
    Task<List<LocationDto>> GetLocationsAsync();
    Task<List<StationDto>> GetStationsByLocationAsync(int locationId);
    Task<StationDto?> GetStationByIdAsync(int stationId);
    bool ValidateSide(IEnumerable<SideDto> existingSides, SideDto newSide);
}
