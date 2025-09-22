using Cafeteria.Shared;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface ILocationSelectViewModel
{
    List<Location> Locations { get; }
    void OnLocationSelected(Location location);
}