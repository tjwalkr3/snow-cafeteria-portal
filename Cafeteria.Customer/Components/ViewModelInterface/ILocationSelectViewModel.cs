using Cafeteria.Shared;

namespace Cafeteria.Customer.Components.Pages
{
    public interface ILocationSelectViewModel
    {
        List<Location> Locations { get; }
        void OnLocationSelected(Location location);
    }
}
