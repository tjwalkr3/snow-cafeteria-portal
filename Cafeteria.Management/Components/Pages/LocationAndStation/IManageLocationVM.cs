using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Management.Components.Pages.LocationAndStation.Location;
using Cafeteria.Management.Components.Pages.LocationAndStation.Station;

namespace Cafeteria.Management.Components.Pages.LocationAndStation;

public interface IManageLocationVM
{
    List<LocationDto> Locations { get; set; }
    Dictionary<int, List<StationDto>> StationsByLocation { get; set; }
    void SetCreateOrEditLocationVM(ICreateOrEditLocationVM vm);
    void SetCreateOrEditStationVM(ICreateOrEditStationVM vm);
    Task LoadLocations();
    Task DeleteLocation(int id);
    Task DeleteStation(int id);
    Task ShowCreateLocationModal();
    Task ShowEditLocationModal(int id);
    Task ShowCreateStationModal(int locationId);
    Task ShowEditStationModal(int stationId);
}
