namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public partial class LocationAndStation : ComponentBase
{
    [Inject]
    public ILocationAndStationVM ViewModel { get; set; } = null!;
}