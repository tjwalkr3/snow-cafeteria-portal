using Cafeteria.Shared;

namespace Cafeteria.Customer.Components.Pages
{
    public class StationSelectVM : IStationSelectViewModel
    {
        public List<Station> Stations { get; private set; } = new();

        public StationSelectVM()
        {
            InitializeStations();
        }

        private void InitializeStations()
        {
            Stations = new List<Station>
            {
                new Station(
                    name: "Grill Station",
                    description: "Fresh burgers, fries, and grilled items"
                ),
                new Station(
                    name: "Pizza Station",
                    description: "Oven-fired pizzas and calzones"
                ),
                new Station(
                    name: "Sandwich Station",
                    description: "Fresh sandwiches and wraps"
                )
            };
        }
    }
}
