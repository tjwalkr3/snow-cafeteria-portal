using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Cafeteria.Management.Components.Pages.Side;

public partial class Side : ComponentBase
{
    [Inject]
    public ISideVM ViewModel { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.LoadSidesAsync();
    }

    private async Task DeleteSide(int id)
    {
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this side?");
        if (confirmed)
        {
            await ViewModel.DeleteSideAsync(id);
        }
    }
}
