using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public interface ILocationAndStationVM
{
    List<LocationDto> Locations { get; }
    int? ExpandedLocationId { get; }
    event Action? OnStateChanged;
    Task LoadStationsAsync();
    void OpenCreateLocationModal();
    void EditLocation(LocationDto location);
    void ToggleLocation(int locationId);
}