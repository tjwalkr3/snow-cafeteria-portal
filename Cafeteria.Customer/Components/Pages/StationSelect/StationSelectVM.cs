using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Shared.Interfaces;
using System.Text.Json;

namespace Cafeteria.Customer.Components.ViewModels;
public class StationSelectVM : IStationSelectVM
{
    private readonly IMenuService _menuService;
    private bool urlParsingFailed = false;
    public bool IsInitialized { get; private set; } = false;
    public LocationDto? SelectedLocation { get; private set; }
    public List<StationDto>? Stations { get; private set; }

    public StationSelectVM(IMenuService menuService)
    {
        _menuService = menuService;
        Stations = new List<StationDto>(); // Start with empty list
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
            Stations = await _menuService.GetStationsByLocation(SelectedLocation.Id);
        }
        catch
        {
            urlParsingFailed = true;
        }
    }

    public bool ErrorOccurredWhileParsingSelectedLocation()
    {
        return urlParsingFailed;
    }
}