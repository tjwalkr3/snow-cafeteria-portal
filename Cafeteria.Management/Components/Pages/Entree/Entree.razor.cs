using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.Pages.Entree;

public partial class Entree : ComponentBase
{
    [Inject]
    public EntreeVM ViewModel { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}
