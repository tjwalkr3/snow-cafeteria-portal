using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public class LocationAndStationVM(ILocationService locationService) : ILocationAndStationVM
{
    public List<LocationDto> Locations { get; private set; } = [];

    public async Task LoadStationsAsync()
    {
        Locations = await locationService.GetAllLocations();
    }
}