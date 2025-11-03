using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public interface IStationSelectVM
{
    List<StationDto>? Stations { get; }
    void ValidateLocationParameter(int location, string? payment);
    Task InitializeStations(int locationId);
    bool ErrorOccurredWhileParsingSelectedLocation();
}