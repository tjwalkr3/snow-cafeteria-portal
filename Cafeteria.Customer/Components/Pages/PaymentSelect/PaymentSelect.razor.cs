using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using Cafeteria.Customer.Services.Swipe;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Cafeteria.Customer.Components.Pages.PaymentSelect;

public partial class PaymentSelect : ComponentBase
{
    [Inject]
    private IApiSwipeService SwipeService { get; set; } = default!;

    [Inject]
    private HttpContext? HttpContext { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    public int SwipeBalance { get; set; } = 0;
    public bool HasSwipes => SwipeBalance > 0;

    protected override async Task OnInitializedAsync()
    {
        if (HttpContext?.User?.Identity?.IsAuthenticated ?? false)
        {
            var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    var swipeData = await SwipeService.GetSwipesByEmail(email);
                    if (swipeData != null)
                    {
                        SwipeBalance = swipeData.SwipeBalance;
                    }
                }
                catch (Exception)
                {
                    // If API call fails, swipes remain at 0
                    SwipeBalance = 0;
                }
            }
        }
    }

    public string CreateUrl(string value)
    {
        Dictionary<string, string?> queryParameter = new() { { "payment", value } };
        return QueryHelpers.AddQueryString("/location-select", queryParameter);
    }

    private async Task ShowNoSwipesAlert()
    {
        await JSRuntime.InvokeVoidAsync("alert", "You are out of swipes, please order with a credit/debit card instead");
    }
}