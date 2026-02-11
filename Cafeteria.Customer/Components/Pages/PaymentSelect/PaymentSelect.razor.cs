using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using Cafeteria.Customer.Services.Swipe;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Cafeteria.Customer.Components.Pages.PaymentSelect;

public partial class PaymentSelect : ComponentBase
{
    [Inject]
    private IApiSwipeService SwipeService { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private ILogger<PaymentSelect> Logger { get; set; } = default!;

    public int SwipeBalance { get; set; } = 0;
    public bool HasSwipes => SwipeBalance > 0;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated ?? false)
        {
            // Try mapped claim type first, then fall back to raw OIDC claim name
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value;

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
                catch (Exception ex)
                {
                    // If API call fails, swipes remain at 0
                    Logger.LogError(ex, "Failed to retrieve swipe data for email: {Email}", email);
                    SwipeBalance = 0;
                }
            }
            else
            {
                Logger.LogWarning("Email claim is missing for authenticated user");
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