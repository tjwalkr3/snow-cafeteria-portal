namespace Cafeteria.Customer.Services;

using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Shared.DTOs.Menu;

public interface IPrinterService
{
    Task<LocationDto?> GetLocationById(int locationId);
    Task<string?> GetPrinterUrl(int locationId);
    Task<bool> PrintOrder(string printerUrl, PrintOrderDto orderData);
}
