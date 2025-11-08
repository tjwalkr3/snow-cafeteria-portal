using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Cafeteria.Shared.DTOs;
using System.Text.Json;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public partial class StationSelect : ComponentBase
{
    [Inject]
    private IStationSelectVM StationSelectVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    public bool IsInitialized { get; set; } = false;

    public string CreateUrl(int stationId)
    {
        Dictionary<string, string?> queryParameters = new() { };

        if (!string.IsNullOrEmpty(Payment))
            queryParameters.Add("payment", Payment);
        queryParameters.Add("location", Location.ToString());
        queryParameters.Add("station", stationId.ToString());

        // Map station to the appropriate page route
        var station = StationSelectVM.Stations?.FirstOrDefault(s => s.Id == stationId);
        if (station != null)
        {
            string route = GetStationRoute(station.StationName);
            return QueryHelpers.AddQueryString(route, queryParameters);
        }

        // Fallback to the original routes if station not found
        if (Payment == "card")
            return QueryHelpers.AddQueryString("/card-menu", queryParameters);
        return QueryHelpers.AddQueryString("/swipe-menu", queryParameters);
    }

    private string GetStationRoute(string stationName)
    {
        // Map station names to their corresponding routes
        return stationName.ToLower() switch
        {
            "breakfast" or "breakfast station" => "/breakfast",
            "deli" or "deli station" or "sandwich station" => "/deli",
            "pizza" or "pizza station" => "/pizza",
            "grill" or "grill station" => "/grill",
            _ => "/swipe-menu" // Default fallback
        };
    }

    public string CreateBackUrl()
    {
        Dictionary<string, string?> queryParameters = new() { };

        if (!string.IsNullOrEmpty(Payment))
            queryParameters.Add("payment", Payment);

        return QueryHelpers.AddQueryString("/location-select", queryParameters);
    }

    protected override async Task OnInitializedAsync()
    {
        StationSelectVM.ValidateParameters(Location, Payment);
        await StationSelectVM.InitializeStations(Location);
        IsInitialized = true;
    }
}