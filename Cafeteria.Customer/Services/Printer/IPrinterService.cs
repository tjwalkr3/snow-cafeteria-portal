namespace Cafeteria.Customer.Services.Printer;

using Cafeteria.Shared.DTOs.Order;

public interface IPrinterService
{
    Task<string?> GetPrinterUrl(int locationId);
    Task<bool> PrintOrder(string printerUrl, BrowserOrder orderData);
}
