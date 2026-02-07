using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.Layout.SigninButton;

public partial class SigninButton : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    private void SignIn()
    {
        var currentUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var returnUrl = Uri.EscapeDataString(currentUri.LocalPath);
        NavigationManager.NavigateTo($"/signin?returnUrl={returnUrl}", forceLoad: true);
    }

    private void SignOut()
    {
        NavigationManager.NavigateTo("/signout", forceLoad: true);
    }
}
