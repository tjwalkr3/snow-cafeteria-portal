using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Cafeteria.Management.Components.Pages.LocationAndStation;
namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public partial class CreateOrEditLocation : ComponentBase
{
    [Inject]
    public ICreateOrEditLocationVM ViewModel { get; set; } = null!;
}