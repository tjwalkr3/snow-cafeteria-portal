using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOsOld;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public partial class LocationSelect : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ILocationSelectVM LocationSelectVM { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }
    public bool IsInitialized { get; set; } = false;

    public string CreateUrl(int locationId)
    {
        Dictionary<string, string?> queryParameters = new() { };

        if (!string.IsNullOrEmpty(Payment))
            queryParameters.Add("payment", Payment);
        queryParameters.Add("location", locationId.ToString());

        return QueryHelpers.AddQueryString("/station-select", queryParameters);
    }

    private string GetLocationIcon(string locationName)
    {
        // Map location names to their corresponding Bootstrap Icons
        var lowerName = locationName.ToLower();

        if (lowerName.Contains("den") || lowerName.Contains("badger"))
            return "bi-house-door-fill";
        else if (lowerName.Contains("bistro") || lowerName.Contains("busters"))
            return "bi-building-fill";
        else if (lowerName.Contains("library"))
            return "bi-book-fill";
        else if (lowerName.Contains("student") || lowerName.Contains("union"))
            return "bi-people-fill";
        else
            return "bi-geo-alt-fill"; // Default location pin icon
    }

    protected override async Task OnInitializedAsync()
    {
        LocationSelectVM.ValidatePaymentParameter(Payment);
        await LocationSelectVM.InitializeLocationsAsync();
        IsInitialized = true;
    }
}