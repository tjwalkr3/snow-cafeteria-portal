using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOsOld;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public partial class LocationSelect
{
    public bool IsInitialized { get; set; } = false;

    public static string CreateUrl(string path, string parameter, string value)
    {
        Dictionary<string, string?> queryParameter = new() { { parameter, JsonSerializer.Serialize(value) } };
        return QueryHelpers.AddQueryString(path, queryParameter);
    }

    protected override async Task OnInitializedAsync()
    {
        await LocationSelectVM.InitializeLocationsAsync();
        IsInitialized = true;
    }

    // QueryHelpers.AddQueryString(uri: "/station-select", queryString: new Dictionary<string, string?> {{ "location", JsonSerializer.Serialize(location)}})
}