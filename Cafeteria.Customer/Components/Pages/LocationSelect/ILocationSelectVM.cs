using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public interface ILocationSelectVM
{
    List<LocationDto> Locations { get; }
    Task InitializeLocationsAsync();
    bool ErrorOccurred();
    Task<bool> IsLocationOpenNow(int locationId);
}