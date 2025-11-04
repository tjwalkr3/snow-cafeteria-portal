using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOsOld;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.PaymentSelect;

public partial class PaymentSelect
{
    public string CreateUrl(string value)
    {
        Dictionary<string, string?> queryParameter = new() { { "payment", value } };
        return QueryHelpers.AddQueryString("/location-select", queryParameter);
    }
}