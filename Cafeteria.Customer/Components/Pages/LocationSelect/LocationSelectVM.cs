using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Shared.Interfaces;

namespace Cafeteria.Customer.Components.ViewModels;

public class LocationSelectVM : ILocationSelectVM
{
    private readonly IMenuService _menuService;
    public List<LocationDto> Locations { get; private set; } = new();

    public LocationSelectVM(IMenuService menuService)
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

