using Cafeteria.Shared.DTOsOld;
using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Services;
using System.Text.Json;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public class PlaceOrderVM : IPlaceOrderVM
{
    private readonly IApiMenuService _menuService;
    private bool locationParameterInvalid = false;
    private bool paymentParameterMissing = false;
    private bool locationFetchFailed = false;
    private List<LocationDto> _locations = new();

    public PlaceOrderVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public decimal CalculateTotalPrice(BrowserOrder order)
    {
        if (order == null)
            return 0m;

        decimal total = 0m;

        foreach (var entreeItem in order.Entrees)
        {
            total += entreeItem.Entree.EntreePrice;
            total += entreeItem.SelectedOptions.Sum(opt => opt.OptionType.FoodOptionPrice);
        }

        foreach (var sideItem in order.Sides)
        {
            total += sideItem.Side.SidePrice;
            total += sideItem.SelectedOptions.Sum(opt => opt.OptionType.FoodOptionPrice);
        }

        total += order.Drinks.Sum(drink => drink.DrinkPrice);

        return total;
    }

    public void ValidateParameters(int location, string? payment)
    {
        locationParameterInvalid = location <= 0;
        paymentParameterMissing = string.IsNullOrEmpty(payment)
            && payment != "card"
            && payment != "swipe";
    }

    public async Task InitializeLocations()
    {
        try
        {
            _locations = await _menuService.GetAllLocations();
        }
        catch
        {
            locationFetchFailed = true;
        }
    }

    public LocationDto? GetLocationById(int locationId)
    {
        return _locations.FirstOrDefault(l => l.Id == locationId);
    }

    public bool ErrorOccurred()
    {
        return locationParameterInvalid || paymentParameterMissing || locationFetchFailed;
    }
}