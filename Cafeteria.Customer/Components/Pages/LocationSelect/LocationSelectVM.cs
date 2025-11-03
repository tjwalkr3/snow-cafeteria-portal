using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Services;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public class LocationSelectVM : ILocationSelectVM
{
    private readonly IApiMenuService _menuService;
    private bool paymentParameterMissing = false;
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

    public void ValidatePaymentParameter(string? payment)
    {
        paymentParameterMissing = string.IsNullOrEmpty(payment);
    }

    public bool ErrorOccurred()
    {
        return Locations == null || Locations.Count == 0 || paymentParameterMissing || initializationFailed;
    }
}

