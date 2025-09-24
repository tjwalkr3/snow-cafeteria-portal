using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IStationSelectVM
{
    List<StationDto> Stations { get; }
}