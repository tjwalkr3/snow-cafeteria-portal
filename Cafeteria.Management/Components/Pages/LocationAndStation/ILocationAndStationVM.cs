using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public interface ILocationAndStationVM
{
    List<LocationDto> Locations { get; }
    Task LoadStationsAsync();
}