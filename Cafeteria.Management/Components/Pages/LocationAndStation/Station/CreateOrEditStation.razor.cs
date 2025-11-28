using Microsoft.AspNetCore.Components;
namespace Cafeteria.Management.Components.Pages.LocationAndStation.Station;

public partial class CreateOrEditStation : ComponentBase
{
    [Inject]
    public ICreateOrEditStationVM ViewModel { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}