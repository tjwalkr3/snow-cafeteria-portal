using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Services;

public class PrinterService(HttpClient client) : IPrinterService
{
    public async Task<LocationDto?> GetLocationById(int locationId)
    {
        if (locationId < 1)
            throw new ArgumentOutOfRangeException(nameof(locationId));

        try
        {
            var response = await client.GetAsync($"menu/locations");
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
            var content = JsonContent.Create(orderData);
            var response = await client.PostAsync(printerUrl, content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}