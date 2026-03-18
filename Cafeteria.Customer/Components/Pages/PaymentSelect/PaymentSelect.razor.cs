using System.Security.Claims;
using Cafeteria.Customer.Services.Cart;
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

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    public int SwipeBalance { get; set; } = 0;
    public bool HasSwipes => SwipeBalance > 0;

    public bool? CurrentIsCardOrder { get; private set; }
    public bool? PendingIsCardOrder { get; private set; }
    public bool ShowConfirmModal { get; set; }

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

    public async Task HandlePaymentSelected(bool isCard)
    {
        if (CurrentIsCardOrder.HasValue && CurrentIsCardOrder.Value != isCard)
        {
            PendingIsCardOrder = isCard;
            ShowConfirmModal = true;
            return;
        }
        CurrentIsCardOrder = isCard;
        await Cart.SetIsCardOrder("order", isCard);
        Navigation.NavigateTo("/location-select");
    }

    public async Task ConfirmPaymentChange()
    {
        if (!PendingIsCardOrder.HasValue) return;
        await Cart.ClearOrder("order");
        CurrentIsCardOrder = PendingIsCardOrder.Value;
        await Cart.SetIsCardOrder("order", PendingIsCardOrder.Value);
        PendingIsCardOrder = null;
        Navigation.NavigateTo("/location-select");
    }

    public void CancelPaymentChange()
    {
        PendingIsCardOrder = null;
        ShowConfirmModal = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(async () =>
            {
                var order = await Cart.GetOrder("order");
                if (order != null)
                {
                    CurrentIsCardOrder = order.IsCardOrder;
                }
                StateHasChanged();
            });
        }
    }

    private async Task ShowNoSwipesAlert()
    {
        await JSRuntime.InvokeVoidAsync("alert", "You are out of swipes, please order with a credit/debit card instead");
    }
}