using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
namespace Cafeteria.Management.Components.Pages.LocationAndStation.Location;

public partial class CreateOrEditLocation : ComponentBase
{
    [Inject]
    public ICreateOrEditLocationVM ViewModel { get; set; } = null!;
}