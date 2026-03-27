using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.Side;

public interface ICreateOrEditSideVM
{
    SideDto CurrentSide { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    bool ShowToast { get; set; }
    string ToastMessage { get; set; }
    Toast.ToastType ToastType { get; set; }
    List<StationDto> Stations { get; set; }
    List<LocationDto> Locations { get; set; }
    int SelectedLocationId { get; set; }
    Task LoadStations();
    Task LoadLocations();
    List<StationDto> GetFilteredStations();
    Task<bool> SaveSide();
}
