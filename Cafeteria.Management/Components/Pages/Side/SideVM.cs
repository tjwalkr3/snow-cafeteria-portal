using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public class SideVM : ISideVM
{
    private readonly ISideService _sideService;
    private readonly ILocationService _locationService;
    private readonly IStationService _stationService;
    private Dictionary<int, string> _stationNames = new();

    public SideVM(ISideService sideService, ILocationService locationService, IStationService stationService)
    {
        _sideService = sideService;
        _locationService = locationService;
        _stationService = stationService;
    }

    public List<SideDto> Sides { get; private set; } = new();

    public async Task LoadSidesAsync()
    {
        Sides = await _sideService.GetAllSides();
        await LoadStationNamesAsync();
    }

    private async Task LoadStationNamesAsync()
    {
        var locations = await _locationService.GetAllLocations();
        var tasks = locations.Select(l => _stationService.GetStationsByLocation(l.Id));
        var stationsLists = await Task.WhenAll(tasks);

        _stationNames = stationsLists
            .SelectMany(s => s)
            .DistinctBy(s => s.Id)
            .ToDictionary(s => s.Id, s => s.StationName);
    }

    public string GetStationName(int stationId)
    {
        return _stationNames.TryGetValue(stationId, out var name) ? name : $"Station {stationId}";
    }

    public async Task DeleteSideAsync(int id)
    {
        await _sideService.DeleteSide(id);
        await LoadSidesAsync();
    }

    public async Task SetInStockAsync(int id, bool inStock)
    {
        await _sideService.SetStockStatusById(id, inStock);
        await LoadSidesAsync();
    }
}
