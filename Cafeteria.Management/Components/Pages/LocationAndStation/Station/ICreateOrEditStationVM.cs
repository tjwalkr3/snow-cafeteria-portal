using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Station;

public interface ICreateOrEditStationVM
{
    StationDto CurrentStation { get; set; }
    List<LocationDto> Locations { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    bool ShowToast { get; set; }
    string ToastMessage { get; set; }
    Toast.ToastType ToastType { get; set; }
    Task<bool> SaveStation();
    Task LoadLocations();
}
