using Cafeteria.Shared.Services.Auth;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Customer.Services.Printer;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;

namespace Cafeteria.UnitTests.Customer.Services;

public class PrinterServiceTests
{
    private readonly Mock<IHttpClientAuth> _mockHttpClient;
    private readonly Mock<IApiMenuService> _mockMenuService;
    private readonly Mock<ILogger<PrinterService>> _mockLogger;

    public PrinterServiceTests()
    {
        _mockHttpClient = new Mock<IHttpClientAuth>();
        _mockMenuService = new Mock<IApiMenuService>();
        _mockLogger = new Mock<ILogger<PrinterService>>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetPrinterUrl_ValidatesIdAndReturnsUrlWhenValid(int locationId)
    {
        var location = new LocationDto
        {
            Id = 1,
            LocationName = "Test Location",
            PrinterUrl = "http://printer.local"
        };

        _mockMenuService.Setup(x => x.GetLocationById(It.IsAny<int>()))
            .ReturnsAsync(location);

        var service = new PrinterService(_mockHttpClient.Object, _mockMenuService.Object, _mockLogger.Object);

        if (locationId < 1)
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetPrinterUrl(locationId));
        }
        else
        {
            var result = await service.GetPrinterUrl(locationId);
            Assert.NotNull(result);
            Assert.Equal("http://printer.local", result);
        }
    }

    [Fact]
    public async Task GetPrinterUrl_ReturnsNullWhenLocationNotFound()
    {
        _mockMenuService.Setup(x => x.GetLocationById(It.IsAny<int>()))
            .ReturnsAsync((LocationDto?)null);

        var service = new PrinterService(_mockHttpClient.Object, _mockMenuService.Object, _mockLogger.Object);

        var result = await service.GetPrinterUrl(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task PrintOrder_ThrowsWhenPrinterUrlIsNull()
    {
        var service = new PrinterService(_mockHttpClient.Object, _mockMenuService.Object, _mockLogger.Object);
        var orderData = new BrowserOrder();

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.PrintOrder(null!, orderData));
    }

    [Fact]
    public async Task PrintOrder_ThrowsWhenOrderDataIsNull()
    {
        var service = new PrinterService(_mockHttpClient.Object, _mockMenuService.Object, _mockLogger.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.PrintOrder("http://printer.local", null!));
    }

    [Fact]
    public async Task PrintOrder_ReturnsTrueOnSuccessfulPost()
    {
        _mockHttpClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<BrowserOrder>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var service = new PrinterService(_mockHttpClient.Object, _mockMenuService.Object, _mockLogger.Object);
        var orderData = new BrowserOrder();

        var result = await service.PrintOrder("http://printer.local", orderData);

        Assert.True(result);
    }

    [Fact]
    public async Task PrintOrder_ReturnsFalseOnFailedPost()
    {
        _mockHttpClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<BrowserOrder>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var service = new PrinterService(_mockHttpClient.Object, _mockMenuService.Object, _mockLogger.Object);
        var orderData = new BrowserOrder();

        var result = await service.PrintOrder("http://printer.local", orderData);

        Assert.False(result);
    }

    [Fact]
    public async Task PrintOrder_ReturnsFalseOnException()
    {
        _mockHttpClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<BrowserOrder>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        var service = new PrinterService(_mockHttpClient.Object, _mockMenuService.Object, _mockLogger.Object);
        var orderData = new BrowserOrder();

        var result = await service.PrintOrder("http://printer.local", orderData);

        Assert.False(result);
    }

    [Fact]
    public async Task PrintOrder_AppendsCorrectEndpointToUrl()
    {
        string? capturedUrl = null;
        _mockHttpClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<BrowserOrder>()))
            .Callback<string, BrowserOrder>((url, data) => capturedUrl = url)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var service = new PrinterService(_mockHttpClient.Object, _mockMenuService.Object, _mockLogger.Object);
        var orderData = new BrowserOrder();

        await service.PrintOrder("http://printer.local", orderData);

        Assert.NotNull(capturedUrl);
        Assert.Equal("http://printer.local/print-order", capturedUrl);
    }
}
