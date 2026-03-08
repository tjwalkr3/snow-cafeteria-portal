using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Menu;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public class StationSelectVM : IStationSelectVM
{
    private readonly IApiMenuService _menuService;
    private bool urlParsingFailed = false;
    public bool IsInitialized { get; private set; } = false;
    public List<StationDto>? Stations { get; private set; }

    public StationSelectVM(IApiMenuService menuService)
    {
        _menuService = menuService;
        Stations = new List<StationDto>();
    }

    public async Task InitializeStations(int locationId)
    {
        try
        {
            Stations = await _menuService.GetStationsByLocation(locationId);
        }
        catch
        {
            urlParsingFailed = true;
        }
    }

    public bool ErrorOccurredWhileParsingSelectedLocation()
    {
        return urlParsingFailed;
    }
}