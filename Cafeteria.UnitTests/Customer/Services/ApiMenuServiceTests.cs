using Cafeteria.Customer;
using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Cafeteria.UnitTests.Customer.Services;

public class ApiMenuServiceTests
{
    private Mock<HttpMessageHandler> CreateMockHttpHandler<T>(T responseData)
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(responseData))
            });
        return mockHandler;
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetStationsByLocation_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<StationDto> { new StationDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetStationsByLocation(id));
        else
        {
            var result = await service.GetStationsByLocation(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetEntreesByStation_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<EntreeDto> { new EntreeDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetEntreesByStation(id));
        else
        {
            var result = await service.GetEntreesByStation(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetSidesByStation_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<SideDto> { new SideDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetSidesByStation(id));
        else
        {
            var result = await service.GetSidesByStation(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetDrinksByLocation_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<DrinkDto> { new DrinkDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetDrinksByLocation(id));
        else
        {
            var result = await service.GetDrinksByLocation(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetOptionsByEntree_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<FoodOptionDto> { new FoodOptionDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetOptionsByEntree(id));
        else
        {
            var result = await service.GetOptionsByEntree(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetFoodOptionsBySide_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<FoodOptionDto> { new FoodOptionDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetOptionsBySide(id));
        else
        {
            var result = await service.GetOptionsBySide(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Fact]
    public async Task GetAllLocations_ReturnsListOfLocations()
    {
        var expectedLocations = new List<LocationDto> { new LocationDto { Id = 1 } };
        var mockHandler = CreateMockHttpHandler(expectedLocations);
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        var result = await service.GetAllLocations();

        Assert.NotNull(result);
        Assert.Single(result);
    }

}
