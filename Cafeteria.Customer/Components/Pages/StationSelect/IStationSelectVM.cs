using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public interface IStationSelectVM
{
    List<StationDto>? Stations { get; }
    void ValidateParameters(int location, string? payment);
    Task InitializeStations(int locationId);
    bool ErrorOccurredWhileParsingSelectedLocation();
}