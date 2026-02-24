using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Cafeteria.Customer.Services.Cart;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public partial class StationSelect : ComponentBase
{
    [Inject]
    private IStationSelectVM StationSelectVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    public bool IsInitialized { get; set; } = false;

    public async Task HandleStationSelected(int stationId)
    {
        var station = StationSelectVM.Stations?.FirstOrDefault(s => s.Id == stationId);
        if (station == null) return;

        await Cart.SetStation("order", station.Id, station.StationName);

        Dictionary<string, string?> queryParameters = new();
        if (!string.IsNullOrEmpty(Payment))
            queryParameters.Add("payment", Payment);
        queryParameters.Add("location", Location.ToString());

        string route = GetStationRoute(station.StationName);
        Navigation.NavigateTo(QueryHelpers.AddQueryString(route, queryParameters));
    }

    private string GetStationRoute(string stationName)
    {
        return stationName.ToLower() switch
        {
            "breakfast" or "breakfast station" => "/breakfast",
            "deli" or "deli station" or "sandwich station" => "/deli",
            "pizza" or "pizza station" => "/pizza",
            "grill" or "grill station" => "/grill",
            "wraps" or "wraps station" => "/wrap",
            _ => "/swipe-menu"
        };
    }

    private string GetStationIcon(string stationName)
    {
        return stationName.ToLower() switch
        {
            "breakfast" or "breakfast station" => "bi-egg-fried",
            "deli" or "deli station" or "sandwich station" => "bi-cup-straw",
            "pizza" or "pizza station" => "bi-pie-chart",
            "grill" or "grill station" => "bi-fire",
            "wraps" or "wraps station" => "bi-tornado",
            _ => "bi-shop"
        };
    }

    public string CreateBackUrl()
    {
        Dictionary<string, string?> queryParameters = new();
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