using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public interface ICreateOrEditLocationVM
{
    LocationDto CurrentLocation { get; set; }
    bool IsVisible { get; set; }
    bool IsEditing { get; set; }
    bool ShowToast { get; set; }
    string ToastMessage { get; set; }
    Toast.ToastType ToastType { get; set; }
    Task<bool> SaveLocation();
}
