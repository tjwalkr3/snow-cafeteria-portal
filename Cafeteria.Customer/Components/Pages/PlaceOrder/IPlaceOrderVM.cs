using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public interface IPlaceOrderVM
{
    decimal CalculateTotalPrice(BrowserOrder order);
    void ValidateParameters(int location, string? payment);
    Task InitializeLocations();
    LocationDto? GetLocationById(int locationId);
    bool ErrorOccurred();
}