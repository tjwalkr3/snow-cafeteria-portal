using Cafeteria.Shared.Services.Auth;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Customer.Services.Printer;

public class PrinterService(IHttpClientAuth client, IApiMenuService menuService, ILogger<PrinterService> logger) : IPrinterService
{
    public async Task<string?> GetPrinterUrl(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        var location = await menuService.GetLocationById(locationId);

        return location?.PrinterUrl;
    }

    public async Task<bool> PrintOrder(string printerUrl, PrintOrderDto orderData)
    {
        if (string.IsNullOrWhiteSpace(printerUrl))
            throw new ArgumentNullException(nameof(printerUrl));

        if (orderData == null)
            throw new ArgumentNullException(nameof(orderData));

        try
        {
            var fullUrl = printerUrl.TrimEnd('/') + "/print-order";
            var response = await client.PostAsync(fullUrl, orderData);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to print order to {Url}", printerUrl);
            return false;
        }
    }
}