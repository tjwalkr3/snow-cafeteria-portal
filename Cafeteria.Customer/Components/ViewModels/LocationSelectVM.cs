using Cafeteria.Shared;
using Cafeteria.Customer.Components.ViewModelInterfaces;

namespace Cafeteria.Customer.Components.ViewModels;

public class LocationSelectVM : ILocationSelectViewModel
{
    public List<Location> Locations { get; private set; } = new();

    public LocationSelectVM()
    {
        InitializeLocations();
    }

    public void OnLocationSelected(Location location)
    {
        // Business logic for location selection can go here
        // For example: store selected location, log selection, etc.
        // Navigation will be handled by the view
    }

    private void InitializeLocations()
    {
        Locations = new List<Location>
        {
            new Location(
                name: "Badger Den",
                address: "GSC Cafeteria on the Ground Floor"
            ),
            new Location(
                name: "Buster's Bistro",
                address: "Karen H. Huntsman Library Gallery"
            )
        };
    }
}

