using Cafeteria.Shared.DTOsOld;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public interface IStationSelectVM
{
    List<StationDtoOld>? Stations { get; }
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingSelectedLocation();
}