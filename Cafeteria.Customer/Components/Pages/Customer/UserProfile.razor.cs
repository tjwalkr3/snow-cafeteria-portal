using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Cafeteria.Customer.Components.Pages.Customer;

public partial class UserProfile : ComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    public string UserName { get; set; } = "Unknown";
    public string UserEmail { get; set; } = "Unknown";
    public int SwipeBalance { get; set; } = 0;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState != null)
        {
            var authState = await AuthenticationState;
            var user = authState.User;

            if (user.Identity?.IsAuthenticated ?? false)
            {
                UserName = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                UserEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
                SwipeBalance = 0; // Hardcoded for now
            }
        }
    }
}
