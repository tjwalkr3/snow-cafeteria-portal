using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public interface IStationSelectVM
{
    List<StationDto>? Stations { get; }
    Task InitializeStations(int locationId);
    bool ErrorOccurredWhileParsingSelectedLocation();
    Task<bool> IsStationOpenNow(int stationId);
}