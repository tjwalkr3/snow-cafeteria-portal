using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public interface ISideVM
{
    List<SideDto> Sides { get; }
    Task LoadSidesAsync();
    Task DeleteSideAsync(int id);
    string GetStationName(int stationId);
}
