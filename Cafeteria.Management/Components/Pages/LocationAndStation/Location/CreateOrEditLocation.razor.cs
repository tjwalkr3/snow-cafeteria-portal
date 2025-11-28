using Microsoft.AspNetCore.Components;
namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public partial class CreateOrEditLocation : ComponentBase
{
    [Inject]
    public ICreateOrEditLocationVM ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}