using Cafeteria.Management.Services.Locations;
using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public class LocationAndStationVM(ILocationService locationService) : ILocationAndStationVM
{
    public List<LocationDto> Locations { get; private set; } = [];

    public async Task LoadStationsAsync()
    {
        Locations = await locationService.GetAllLocations();
    }
}