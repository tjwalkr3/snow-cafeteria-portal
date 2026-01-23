namespace Cafeteria.Customer.Services;

using Cafeteria.Shared.DTOs;

public interface IPrinterService
{
    Task<LocationDto?> GetLocationById(int locationId);
    Task<string?> GetPrinterUrl(int locationId);
    Task<bool> PrintOrder(string printerUrl, PrintOrderDto orderData);
}
