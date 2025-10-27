using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public interface ILocationSelectVM
{
    List<LocationDtoOld> Locations { get; }
    Task InitializeLocationsAsync();
    bool ErrorOccurred();
}