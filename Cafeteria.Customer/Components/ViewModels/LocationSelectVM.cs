using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Customer.Components.Data;

namespace Cafeteria.Customer.Components.ViewModels;

public class LocationSelectVM : ILocationSelectViewModel
{
    public List<CafeteriaLocationDto> Locations { get; private set; } = new();

    public LocationSelectVM()
    {
        InitializeLocations();
    }

    public void OnLocationSelected(CafeteriaLocationDto location)
    {
        
    }

    private void InitializeLocations()
    {
        Locations = DummyData.GetLocationList;
    }
}

