using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Cafeteria.Customer.Services.Customer;

namespace Cafeteria.Customer.Components.Layout.SigninButton;

public partial class SigninButton : ComponentBase, IDisposable
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public ICustomerService CustomerService { get; set; } = default!;

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    private bool _hasRegistered = false;

    protected override void OnInitialized()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);

        if (query.ContainsKey("state") || query.ContainsKey("session_state") || query.ContainsKey("iss") || query.ContainsKey("code"))
        {
            var newUri = NavigationManager.GetUriWithQueryParameters(
                    new Dictionary<string, object?>
                    {
                        ["state"] = null,
                        ["session_state"] = null,
                        ["iss"] = null,
                        ["code"] = null
                    });

            NavigationManager.NavigateTo(newUri);
        }

        AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await CheckAndRegisterCustomer();
        }
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        await CheckAndRegisterCustomer();
        await InvokeAsync(StateHasChanged);
    }

    private async Task CheckAndRegisterCustomer()
    {
        if (_hasRegistered) return;

        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            _hasRegistered = true;
            await CustomerService.RegisterOrUpdateCustomerAsync();
        }
    }

    private void SignIn()
    {
        NavigationManager.NavigateTo("login?returnUrl=/", forceLoad: true);
    }

    private void SignOut()
    {
        _hasRegistered = false;
        NavigationManager.NavigateTo("logout", forceLoad: true);
    }

    public void Dispose()
    {
        AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}
