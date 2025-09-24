using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Customer.Components.Data;

namespace Cafeteria.Customer.Components.ViewModels;
public class StationSelectVM : IStationSelectVM
{
    public List<StationDto> Stations { get; private set; } = new();

    public StationSelectVM()
    {
        Stations = DummyData.GetStationList;
    }
}