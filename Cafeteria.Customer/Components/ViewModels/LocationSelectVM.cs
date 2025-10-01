using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Customer.Components.Data;

namespace Cafeteria.Customer.Components.ViewModels;

public class LocationSelectVM : ILocationSelectVM
{
    public List<LocationDto> Locations { get; private set; } = new();

    public LocationSelectVM()
    {
        InitializeLocations();
    }

    public void OnLocationSelected(LocationDto location)
    {
        // Business logic for location selection can go here
        // For example: store selected location, log selection, etc.
        // Navigation will be handled by the view
    }

    private void InitializeLocations()
    {
        Locations = DummyData.GetLocationList;
    }
}

