using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Menu;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public class LocationSelectVM : ILocationSelectVM
{
    private readonly IApiMenuService _menuService;
    private bool initializationFailed = false;
    public List<LocationDto> Locations { get; private set; } = new();

    public LocationSelectVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task InitializeLocationsAsync()
    {
        try
        {
            Locations = await _menuService.GetAllLocations();
        }
        catch
        {
            initializationFailed = true;
        }
    }

    public bool ErrorOccurred()
    {
        return Locations == null || Locations.Count == 0 || initializationFailed;
    }
}

