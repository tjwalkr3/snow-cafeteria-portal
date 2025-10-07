using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Cafeteria.Customer.Tests;

public class MenuServiceTests
{
    private Mock<HttpMessageHandler> CreateMockHttpHandler<T>(List<T> responseData)
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
    public void GetStationsByLocation_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<StationDto> { new StationDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            Assert.Throws<ArgumentOutOfRangeException>(() => service.GetStationsByLocation(id));
        else
        {
            var result = service.GetStationsByLocation(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void GetFoodItemsByStation_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<FoodItemDto> { new FoodItemDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            Assert.Throws<ArgumentOutOfRangeException>(() => service.GetFoodItemsByStation(id));
        else
        {
            var result = service.GetFoodItemsByStation(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void GetIngredientTypesByFoodItem_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<IngredientTypeDto> { new IngredientTypeDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            Assert.Throws<ArgumentOutOfRangeException>(() => service.GetIngredientTypesByFoodItem(id));
        else
        {
            var result = service.GetIngredientTypesByFoodItem(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void GetIngredientsByType_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<IngredientDto> { new IngredientDto { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            Assert.Throws<ArgumentOutOfRangeException>(() => service.GetIngredientsByType(id));
        else
        {
            var result = service.GetIngredientsByType(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Fact]
    public void GetAllLocations_ReturnsListOfLocations()
    {
        var expectedLocations = new List<LocationDto> { new LocationDto { Id = 1 } };
        var mockHandler = CreateMockHttpHandler(expectedLocations);
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        var result = service.GetAllLocations();

        Assert.NotNull(result);
        Assert.Single(result);
    }
}
