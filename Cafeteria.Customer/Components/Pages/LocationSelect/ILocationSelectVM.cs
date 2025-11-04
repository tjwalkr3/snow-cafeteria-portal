using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.LocationSelect;

public interface ILocationSelectVM
{
    List<LocationDto> Locations { get; }
    Task InitializeLocationsAsync();
    void ValidatePaymentParameter(string? payment);
    bool ErrorOccurred();
}