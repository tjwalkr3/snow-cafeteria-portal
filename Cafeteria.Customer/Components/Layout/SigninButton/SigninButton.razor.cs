using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Cafeteria.Customer.Components.Layout.SigninButton;

public partial class SigninButton : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private void SignIn()
    {
        NavigationManager.NavigateTo("/signin?returnUrl=/");
    }

    private async Task SignOut()
    {
        await JSRuntime.InvokeVoidAsync("window.location.assign", "/signout");
    }
}
