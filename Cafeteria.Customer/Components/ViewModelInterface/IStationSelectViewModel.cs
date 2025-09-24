using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IStationSelectViewModel
{
    List<StationDto> Stations { get; }
}