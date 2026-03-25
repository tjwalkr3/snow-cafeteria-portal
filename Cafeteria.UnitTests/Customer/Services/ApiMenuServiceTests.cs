using Cafeteria.Shared.Services.Auth;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;
using Moq;

namespace Cafeteria.UnitTests.Customer.Services;

public class ApiMenuServiceTests
{
    private Mock<IHttpClientAuth> CreateMockHttpClient<T>(T responseData)
    {
        var mockClient = new Mock<IHttpClientAuth>();
        mockClient.Setup(x => x.GetAsync<T>(It.IsAny<string>()))
            .ReturnsAsync(responseData);
        return mockClient;
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetStationsByLocation_ValidatesIdAndReturnsListWhenValid(int id)
    {
        var mockClient = CreateMockHttpClient(new List<StationDto> { new StationDto { Id = 1 } });
        var service = new ApiMenuService(mockClient.Object);

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
        var mockClient = CreateMockHttpClient(new List<EntreeDto> { new EntreeDto { Id = 1 } });
        var service = new ApiMenuService(mockClient.Object);

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
        var mockClient = CreateMockHttpClient(new List<SideWithOptionsDto>
        {
            new SideWithOptionsDto
            {
                Side = new SideDto { Id = 1 },
                OptionTypes = new List<FoodOptionTypeWithOptionsDto>()
            }
        });
        var service = new ApiMenuService(mockClient.Object);

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
        var mockClient = CreateMockHttpClient(new List<DrinkDto> { new DrinkDto { Id = 1 } });
        var service = new ApiMenuService(mockClient.Object);

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
        var mockClient = CreateMockHttpClient(new List<FoodOptionDto> { new FoodOptionDto { Id = 1 } });
        var service = new ApiMenuService(mockClient.Object);

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
        var mockClient = CreateMockHttpClient(new List<FoodOptionDto> { new FoodOptionDto { Id = 1 } });
        var service = new ApiMenuService(mockClient.Object);

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
        var mockClient = CreateMockHttpClient(expectedLocations);
        var service = new ApiMenuService(mockClient.Object);

        var result = await service.GetAllLocations();

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task GetLocationById_ValidatesIdAndReturnsLocationWhenValid(int id)
    {
        var expectedLocation = new LocationDto { Id = 1, LocationName = "Test Location" };
        var mockClient = CreateMockHttpClient(expectedLocation);
        var service = new ApiMenuService(mockClient.Object);

        if (id < 1)
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await service.GetLocationById(id));
        else
        {
            var result = await service.GetLocationById(id);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Location", result.LocationName);
        }
    }

}
