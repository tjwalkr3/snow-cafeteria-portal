using Cafeteria.Management.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Components.Pages.Side;

public class SideVM : ISideVM
{
    private readonly ISideService _sideService;

    public SideVM(ISideService sideService)
    {
        _sideService = sideService;
    }

    public List<SideDto> Sides { get; private set; } = new();

    public async Task LoadSidesAsync()
    {
        Sides = await _sideService.GetAllSides();
    }

    public async Task DeleteSideAsync(int id)
    {
        await _sideService.DeleteSide(id);
        await LoadSidesAsync();
    }
}
