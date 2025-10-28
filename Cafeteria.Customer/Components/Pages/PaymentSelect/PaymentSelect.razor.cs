using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOsOld;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Customer.Components.Pages.PaymentSelect;

public partial class PaymentSelect
{
    public string cardParam = CreateUrl("/location-select", "payment", "card");
    public string swipeParam = CreateUrl("/location-select", "payment", "swipe");

    public static string CreateUrl(string path, string parameter, string value)
    {
        Dictionary<string, string?> queryParameter = new() { { parameter, value } };
        return QueryHelpers.AddQueryString(path, queryParameter);
    }
}