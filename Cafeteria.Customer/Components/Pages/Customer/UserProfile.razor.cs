using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Cafeteria.Customer.Services.Swipe;

namespace Cafeteria.Customer.Components.Pages.Customer;

public partial class UserProfile : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    [Inject]
    private IApiSwipeService SwipeService { get; set; } = default!;

    public string UserName { get; set; } = "Unknown";
    public string UserEmail { get; set; } = "Unknown";
    public int SwipeBalance { get; set; } = 0;
    private bool _isLoaded = false;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState != null)
        {
            var authState = await AuthenticationState;
            var user = authState.User;

            if (user.Identity?.IsAuthenticated ?? false)
            {
                UserName = user.FindFirst("name")?.Value ?? user.FindFirst("preferred_username")?.Value ?? "Unknown";
                UserEmail = user.FindFirst("email")?.Value ?? user.FindFirst("preferred_username")?.Value ?? "Unknown";

                if (!string.IsNullOrEmpty(UserEmail))
                {
                    try
                    {
                        var swipeData = await SwipeService.GetSwipesByEmail(UserEmail);
                        if (swipeData != null)
                        {
                            SwipeBalance = swipeData.SwipeBalance;
                        }
                    }
                    catch (Exception)
                    {
                        SwipeBalance = 0;
                    }
                }
            }
        }
        _isLoaded = true;
    }
}
