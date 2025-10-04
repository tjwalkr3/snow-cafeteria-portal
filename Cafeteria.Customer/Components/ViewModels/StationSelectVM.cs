using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Customer.Components.Data;
using System.Text.Json;

namespace Cafeteria.Customer.Components.ViewModels;
public class StationSelectVM : IStationSelectVM
{
    string errorString = "Error";
    public bool IsInitialized { get; private set; } = false;
    public LocationDto? SelectedLocation { get; private set; }
    public List<StationDto>? Stations { get; private set; }

    public StationSelectVM()
    {
        Stations = DummyData.GetStationList;
    }

    public async Task GetDataFromRouteParameters(string uri)
    {
        await Task.Delay(0); // Simulate async work

        string queryString = uri.Substring(uri.IndexOf('?') + 1);
        var queryParams = System.Web.HttpUtility.ParseQueryString(queryString);
        try
        {
            LocationDto location = JsonSerializer.Deserialize<LocationDto>(queryParams.Get("location") ?? string.Empty) ?? throw new ArgumentException("Failed to deserialize location from query parameter.");
            SelectedLocation = location;
            Stations = DummyData.GetStationsByLocation(SelectedLocation.Id);
        }
        catch
        {
            SelectedLocation = new();
            SelectedLocation.Name = errorString;
        }
    }

    public bool ErrorOccurredWhileParsingSelectedLocation()
    {
        return SelectedLocation is not null && SelectedLocation.Name == errorString;
    }
}