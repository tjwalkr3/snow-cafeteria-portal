using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Components.Shared;

namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public class CreateOrEditLocationVM : ICreateOrEditLocationVM
{
    private readonly ILocationService _locationService;
    private readonly IManageLocationVM _parentVM;

    public LocationDto CurrentLocation { get; set; } = new();
    public bool IsVisible { get; set; }
    public bool IsEditing { get; set; }
    public bool ShowToast { get; set; }
    public string ToastMessage { get; set; } = string.Empty;
    public Toast.ToastType ToastType { get; set; }

    public CreateOrEditLocationVM(ILocationService locationService, IManageLocationVM parentVM)
    {
        _locationService = locationService;
        _parentVM = parentVM;
    }

    public async Task<bool> SaveLocation()
    {
        var isDuplicate = _parentVM.Locations.Any(l =>
            l.LocationName.Equals(CurrentLocation.LocationName, StringComparison.OrdinalIgnoreCase) &&
            l.Id != CurrentLocation.Id);

        if (isDuplicate)
        {
            ShowToast = true;
            ToastMessage = "A location with this name already exists.";
            ToastType = Toast.ToastType.Error;
            return false;
        }

        if (IsEditing)
        {
            await _locationService.UpdateLocation(CurrentLocation.Id, CurrentLocation.LocationName, CurrentLocation.LocationDescription, CurrentLocation.IconId, CurrentLocation.PrinterUrl);
        }
        else
        {
            await _locationService.CreateLocation(CurrentLocation.LocationName, CurrentLocation.LocationDescription, CurrentLocation.IconId, CurrentLocation.PrinterUrl);
        }

        IsVisible = false;
        await _parentVM.LoadLocations();
        return true;
    }
}
