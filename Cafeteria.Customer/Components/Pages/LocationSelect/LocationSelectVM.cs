using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Services;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public class LocationSelectVM : ILocationSelectVM
{
    private readonly IApiMenuService _menuService;
    public List<LocationDto> Locations { get; private set; } = new();

    public LocationSelectVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task InitializeLocationsAsync()
    {
        Locations = await _menuService.GetAllLocations();
    }
    public bool ErrorOccurred()
    {
        return false; // TODO: check for errors getting locations from Menu Service
    }
}

