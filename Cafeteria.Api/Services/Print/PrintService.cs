using System.Data;
using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Order;
using Dapper;

namespace Cafeteria.Api.Services.Print;

public class PrintService(IDbConnection dbConnection, IHttpClientFactory httpClientFactory, ILogger<PrintService> logger) : IPrintService
{
    private readonly IDbConnection _dbConnection = dbConnection;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<PrintService> _logger = logger;

    public async Task PrintOrder(BrowserOrder browserOrder, int orderId)
    {
        try
        {
            var locationId = browserOrder.Location?.Id;
            if (!locationId.HasValue || locationId.Value < 1)
                return;

            var printerUrl = await GetPrinterUrl(locationId.Value);
            if (string.IsNullOrWhiteSpace(printerUrl))
                return;

            var client = _httpClientFactory.CreateClient();
            var fullUrl = printerUrl.TrimEnd('/') + "/print-order";
            var payload = new
            {
                BrowserOrder = browserOrder,
                OrderId = orderId
            };
            var response = await client.PostAsJsonAsync(fullUrl, payload);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Printer endpoint returned status code {StatusCode} for location {LocationId}",
                    (int)response.StatusCode,
                    locationId.Value
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to print order after persistence");
        }
    }

    private async Task<string?> GetPrinterUrl(int locationId)
    {
        const string sql = @"
                        SELECT printer_url
                        FROM cafeteria.cafeteria_location
                        WHERE id = @LocationId";

        return await _dbConnection.QuerySingleOrDefaultAsync<string?>(sql, new { LocationId = locationId });
    }
}