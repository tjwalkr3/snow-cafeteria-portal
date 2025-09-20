using Cafeteria.Shared;

namespace Cafeteria.Customer.Components.Pages
{
    public class LocationSelectVM : ILocationSelectViewModel
    {
        public List<Location> Locations { get; private set; } = new();

        public LocationSelectVM()
        {
            InitializeLocations();
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
}
