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
    Task DeleteLocation(int locationId);
    void ToggleLocation(int locationId);
}