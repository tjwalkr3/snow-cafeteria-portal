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

        if (Payment == "card")
            return QueryHelpers.AddQueryString("/card-menu", queryParameters);
        return QueryHelpers.AddQueryString("/swipe-menu", queryParameters);
    }

    protected override async Task OnInitializedAsync()
    {
        StationSelectVM.ValidateParameters(Location, Payment);
        await StationSelectVM.InitializeStations(Location);
        IsInitialized = true;
    }
}