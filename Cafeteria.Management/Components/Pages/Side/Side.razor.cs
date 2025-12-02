using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class Side : ComponentBase
{
    [Inject]
    public ISideVM ViewModel { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}
