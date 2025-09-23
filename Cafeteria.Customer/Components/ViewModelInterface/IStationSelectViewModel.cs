using Cafeteria.Shared;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IStationSelectViewModel
{
    List<Station> Stations { get; }
}