using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Cafeteria.Management.Components.Layout.SigninButton;

public partial class SigninButton : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

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
    }

    private void SignIn()
    {
        NavigationManager.NavigateTo($"login?returnUrl={Uri.EscapeDataString(NavigationManager.BaseUri)}", forceLoad: true);
    }

    private void SignOut()
    {
        NavigationManager.NavigateTo("logout", forceLoad: true);
    }
}
