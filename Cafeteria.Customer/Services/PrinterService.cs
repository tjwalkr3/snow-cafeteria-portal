using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.Extensions.Configuration;

namespace Cafeteria.Customer.Services;

public class PrinterService(HttpClient client, IConfiguration configuration, ILogger<PrinterService> logger) : IPrinterService
{
    public async Task<LocationDto?> GetLocationById(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        try
        {
            var apiBaseUrl = configuration["ApiBaseUrl"] ?? "http://api/api/";
            var response = await client.GetAsync($"{apiBaseUrl}menu/locations");
            response.EnsureSuccessStatusCode();
            var locations = await response.Content.ReadFromJsonAsync<List<LocationDto>>() ?? new List<LocationDto>();

            return locations.FirstOrDefault(l => l.Id == locationId);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetPrinterUrl(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        var location = await GetLocationById(locationId);
        logger.LogInformation("Location {LocationId}: {LocationName}, PrinterUrl: {PrinterUrl}", 
            locationId, location?.LocationName ?? "NOT FOUND", location?.PrinterUrl ?? "NULL");
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
            // Ensure the URL ends with /print-order
            var fullUrl = printerUrl.TrimEnd('/') + "/print-order";
            logger.LogInformation("Attempting to print order to: {Url}", fullUrl);
            var content = JsonContent.Create(orderData);
            var response = await client.PostAsync(fullUrl, content);
            logger.LogInformation("Print order response: {StatusCode}", response.StatusCode);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to print order to {Url}", printerUrl);
            return false;
        }
    }
}