using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public interface IStationSelectVM
{
    List<StationDto>? Stations { get; }
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingSelectedLocation();
}