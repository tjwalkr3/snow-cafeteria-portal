using Microsoft.AspNetCore.Components;
namespace Cafeteria.Management.Components.Pages.LocationAndStation.Station;

public partial class CreateOrEditStation : ComponentBase, IDisposable
{
    [Inject]
    public ICreateOrEditStationVM ViewModel { get; set; } = null!;

    protected override void OnInitialized()
    {
        ViewModel.OnStateChanged += StateHasChanged;
        base.OnInitialized();
    }

    public void Dispose()
    {
        ViewModel.OnStateChanged -= StateHasChanged;
    }
}