namespace Cafeteria.Customer.Components.Pages.Stations.Configuration;

public interface IStationConfigurationProvider
{
    StationConfiguration GetConfiguration(StationType stationType);
    StationType ParseStationType(string stationName);
    bool TryParseStationType(string stationName, out StationType stationType);
}
