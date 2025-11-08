using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Cafeteria.Customer.Components.Pages.Error;

public partial class Error : ComponentBase
{
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    private string? RequestId { get; set; }

    protected override void OnInitialized() =>
        RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
}
