using Cafeteria.Shared.DTOsOld;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public interface IPlaceOrderVM
{
    decimal CalculateTotalPrice(BrowserOrder order);
}