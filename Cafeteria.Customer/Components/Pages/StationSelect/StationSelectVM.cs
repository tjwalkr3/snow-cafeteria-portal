using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Services;

namespace Cafeteria.Customer.Components.Pages.StationSelect;

public class StationSelectVM : IStationSelectVM
{
    private readonly IApiMenuService _menuService;
    private bool urlParsingFailed = false;
    private bool locationParameterInvalid = false;
    private bool paymentParameterMissing = false;
    public bool IsInitialized { get; private set; } = false;
    public List<StationDto>? Stations { get; private set; }

    public StationSelectVM(IApiMenuService menuService)
    {
        _menuService = menuService;
        Stations = new List<StationDto>();
    }

    public void ValidateLocationParameter(int location, string? payment)
    {
        locationParameterInvalid = location <= 0;
        paymentParameterMissing = string.IsNullOrEmpty(payment);
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
        return urlParsingFailed || locationParameterInvalid || paymentParameterMissing;
    }
}