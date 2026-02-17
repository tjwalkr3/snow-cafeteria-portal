using Microsoft.AspNetCore.Components;
namespace Cafeteria.Shared.Components.Layout.NotSignedIn;

public partial class NotSignedIn : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    protected override void OnInitialized()
    {
        var currentUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

        if (!currentUri.LocalPath.Equals("/signin", StringComparison.OrdinalIgnoreCase))
        {
            var returnUrl = Uri.EscapeDataString(currentUri.LocalPath);
            NavigationManager.NavigateTo($"/signin?returnUrl={returnUrl}", forceLoad: true);
        }
    }
}
