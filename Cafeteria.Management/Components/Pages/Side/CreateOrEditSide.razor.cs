using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class CreateOrEditSide : ComponentBase
{
    [Inject]
    public CreateOrEditSideVM ViewModel { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}
