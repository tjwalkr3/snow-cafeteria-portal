using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOsOld;
using Cafeteria.Shared.DTOs;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Cafeteria.Customer.Tests.ServiceTests;

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
    public async Task GetFoodItemsByStation_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<FoodItemDtoOld> { new FoodItemDtoOld { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetFoodItemsByStation(id));
        else
        {
            var result = await service.GetFoodItemsByStation(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetIngredientTypesByFoodItem_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<IngredientTypeDtoOld> { new IngredientTypeDtoOld { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetIngredientTypesByFoodItem(id));
        else
        {
            var result = await service.GetIngredientTypesByFoodItem(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetIngredientsByType_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockHandler = CreateMockHttpHandler(new List<IngredientDtoOld> { new IngredientDtoOld { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetIngredientsByType(id));
        else
        {
            var result = await service.GetIngredientsByType(id);
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Fact]
    public async Task GetAllLocations_ReturnsListOfLocations()
    {
        var expectedLocations = new List<LocationDtoOld> { new LocationDtoOld { Id = 1 } };
        var mockHandler = CreateMockHttpHandler(expectedLocations);
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        var result = await service.GetAllLocations();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetIngredientById_ReturnsAnIngredient(int id)
    {
        var mockHandler = CreateMockHttpHandler(new IngredientDtoOld { Id = 1 });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetIngredientById(id));
        else
        {
            var result = await service.GetIngredientById(id);
            Assert.NotNull(result);
        }
    }

    public static IEnumerable<object[]> GetIngredientsOrganizedByTypeTestData =>
        [
            [null!, 0],
            [new List<IngredientTypeDtoOld>(), 0],
            [new List<IngredientTypeDtoOld> { new() { Id = 1 } }, 1]
        ];

    [Theory]
    [MemberData(nameof(GetIngredientsOrganizedByTypeTestData))]
    public async Task GetIngredientsOrganizedByType_ReturnsDictionaryBasedOnInput(List<IngredientTypeDtoOld> types, int expectedCount)
    {
        var mockHandler = CreateMockHttpHandler(new List<IngredientDtoOld> { new IngredientDtoOld { Id = 1 } });
        var httpClient = new HttpClient(mockHandler.Object) { BaseAddress = new Uri("http://test/api") };
        var service = new ApiMenuService(httpClient);

        var result = await service.GetIngredientsOrganizedByType(types);

        Assert.NotNull(result);
        Assert.Equal(expectedCount, result.Count);
    }
}
