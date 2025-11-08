using Cafeteria.Customer.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Cafeteria.Customer.Components.Pages.Error;

public partial class Error : ComponentBase
{
    [Inject]
    private ICartService CartService { get; set; } = default!;

    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    private string? RequestId { get; set; }

    protected override void OnInitialized() =>
        RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CartService.ClearOrder("test");
        }
    }
}
