using Microsoft.AspNetCore.Components;
namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public partial class CreateOrEditLocation : ComponentBase, IDisposable
{
    [Inject]
    public ICreateOrEditLocationVM ViewModel { get; set; } = default!;

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
