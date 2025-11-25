using Microsoft.AspNetCore.Components;

namespace Cafeteria.Management.Components.entree;

public partial class CreateOrEditEntree : ComponentBase
{
    [Inject]
    public CreateOrEditEntreeVM ViewModel { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}
