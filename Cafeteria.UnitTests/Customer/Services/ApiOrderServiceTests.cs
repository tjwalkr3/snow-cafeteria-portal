using Cafeteria.Shared.Services.Auth;
using Cafeteria.Customer.Services.Order;
using Cafeteria.Shared.DTOs.Order;
using Moq;
using System.Net;
using System.Text.Json;

namespace Cafeteria.UnitTests.Customer.Services;

public class ApiOrderServiceTests
{
    private Mock<IHttpClientAuth> CreateMockHttpClient<T>(T responseData)
    {
        var mockClient = new Mock<IHttpClientAuth>();
        mockClient.Setup(x => x.GetAsync<T>(It.IsAny<string>()))
            .ReturnsAsync(responseData);
        return mockClient;
    }

    [Fact]
    public async Task CreateOrder_ReturnsOrderDto_WhenSuccessful()
    {
        var browserOrder = new BrowserOrder { IsCardOrder = true };

        var expectedOrder = new OrderDto
        {
            Id = 1,
            OrderTime = DateTime.Now
        };

        var mockClient = new Mock<IHttpClientAuth>();
        mockClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<BrowserOrder>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedOrder))
            });

        var service = new ApiOrderService(mockClient.Object);

        var result = await service.CreateOrder(browserOrder);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task CreateOrder_PostsBrowserOrderToOrderEndpoint()
    {
        var browserOrder = new BrowserOrder { IsCardOrder = true };
        var expectedOrder = new OrderDto
        {
            Id = 2,
            OrderTime = DateTime.Now
        };

        var mockClient = new Mock<IHttpClientAuth>();
        mockClient.Setup(x => x.PostAsync("order", browserOrder))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedOrder))
            });

        var service = new ApiOrderService(mockClient.Object);

        var result = await service.CreateOrder(browserOrder);

        Assert.Equal(2, result.Id);
        mockClient.Verify(x => x.PostAsync("order", browserOrder), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_ThrowsException_WhenResponseIsUnsuccessful()
    {
        var browserOrder = new BrowserOrder { IsCardOrder = true };

        var mockClient = new Mock<IHttpClientAuth>();
        mockClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<BrowserOrder>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

        var service = new ApiOrderService(mockClient.Object);

        await Assert.ThrowsAsync<HttpRequestException>(async () => await service.CreateOrder(browserOrder));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetOrderById_ValidatesIdAndReturnsOrderWhenValid(int id)
    {
        var expectedOrder = new OrderDto { Id = 1, OrderTime = DateTime.Now };
        var mockClient = CreateMockHttpClient(expectedOrder);
        var service = new ApiOrderService(mockClient.Object);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetOrderById(id));
        else
        {
            var result = await service.GetOrderById(id);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }
    }

    [Fact]
    public async Task GetOrderById_ReturnsNull_WhenOrderNotFound()
    {
        var mockClient = CreateMockHttpClient<OrderDto?>(null);
        var service = new ApiOrderService(mockClient.Object);

        var result = await service.GetOrderById(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsListOfOrders()
    {
        var expectedOrders = new List<OrderDto>
        {
            new OrderDto { Id = 1, OrderTime = DateTime.Now },
            new OrderDto { Id = 2, OrderTime = DateTime.Now }
        };

        var mockClient = CreateMockHttpClient(expectedOrders);
        var service = new ApiOrderService(mockClient.Object);

        var result = await service.GetAllOrders();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsEmptyList_WhenNoOrders()
    {
        var mockClient = CreateMockHttpClient<List<OrderDto>?>(null);
        var service = new ApiOrderService(mockClient.Object);

        var result = await service.GetAllOrders();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
