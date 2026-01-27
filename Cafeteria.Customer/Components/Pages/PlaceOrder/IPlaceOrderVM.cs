using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public interface IPlaceOrderVM
{
    decimal CalculateTotalPrice(BrowserOrder order);
    decimal CalculateTax(BrowserOrder order);
    int CalculateTotalSwipe(BrowserOrder order);
    void ValidateParameters(int location, string? payment);
    Task InitializeLocations();
    LocationDto? GetLocationById(int locationId);
    bool ErrorOccurred();
    List<SwipeGroup> GroupItemsIntoSwipes(BrowserOrder order);
    List<EntreeGroup> GroupEntrees(BrowserOrder order);
    List<SideGroup> GroupSides(BrowserOrder order);
    List<DrinkGroup> GroupDrinks(BrowserOrder order);
    CreateOrderDto ConvertToCreateOrderDto(BrowserOrder order);
}