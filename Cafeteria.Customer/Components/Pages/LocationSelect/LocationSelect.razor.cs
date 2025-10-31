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

    public string CreateUrl(string path, int locationId)
    {
        Dictionary<string, string?> queryParameters = new() { };

        if (!string.IsNullOrEmpty(Payment))
            queryParameters.Add("payment", Payment);
        queryParameters.Add("location", locationId.ToString());

        return QueryHelpers.AddQueryString(path, queryParameters);
    }

    protected override async Task OnInitializedAsync()
    {
        await LocationSelectVM.InitializeLocationsAsync();
        IsInitialized = true;
    }
}