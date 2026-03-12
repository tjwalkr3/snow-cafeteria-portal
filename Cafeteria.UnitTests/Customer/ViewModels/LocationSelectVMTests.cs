using Cafeteria.Customer;
using Cafeteria.Customer.Components.Pages.LocationSelect;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Customer.Services;
using Moq;
using Cafeteria.Customer.Services.Menu;

namespace Cafeteria.Customer.ViewModels.Tests;

public class LocationSelectVMTests
{
    private readonly Mock<IApiMenuService> _mockMenuService;

    public LocationSelectVMTests()
    {
        _mockMenuService = new Mock<IApiMenuService>();
    }

    [Fact]
    public async Task ErrorOccurred_ReturnsTrue_WhenInitializeLocationsAsyncFails()
    {
        _mockMenuService.Setup(m => m.GetAllLocations())
            .ThrowsAsync(new Exception("API Error"));

        var locationSelectVM = new LocationSelectVM(_mockMenuService.Object);

        await locationSelectVM.InitializeLocationsAsync();

        Assert.True(locationSelectVM.ErrorOccurred());
    }

    [Fact]
    public async Task InitializeLocationsAsync_SetsLocations_WhenApiCallSucceeds()
    {
        var expectedLocations = new List<LocationDto>
        {
            new LocationDto { Id = 1, LocationName = "Location 1" },
            new LocationDto { Id = 2, LocationName = "Location 2" }
        };

        _mockMenuService.Setup(m => m.GetAllLocations())
            .ReturnsAsync(expectedLocations);

        // Mock business hours for all days of the week (1-7 where 7 = Sunday)
        // Set hours as 00:00 to 23:59 to ensure test passes regardless of when it runs
        var businessHours = Enumerable.Range(1, 7)
            .Select(weekday => new LocationBusinessHoursDto
            {
                Id = weekday,
                LocationId = 0,
                WeekdayId = weekday,
                OpenTime = new TimeOnly(0, 0),
                CloseTime = new TimeOnly(23, 59)
            })
            .ToList();

        _mockMenuService.Setup(m => m.GetLocationBusinessHours(It.IsAny<int>()))
            .ReturnsAsync(businessHours);

        _mockMenuService.Setup(m => m.GetLocationExceptions(It.IsAny<int>()))
            .ReturnsAsync(new List<LocationExceptionHoursDto>());

        var locationSelectVM = new LocationSelectVM(_mockMenuService.Object);

        await locationSelectVM.InitializeLocationsAsync();

        Assert.Equal(expectedLocations, locationSelectVM.Locations);
        Assert.False(locationSelectVM.ErrorOccurred());
    }

    [Fact]
    public async Task ErrorOccurred_ReturnsTrue_WhenLocationsIsEmpty()
    {
        _mockMenuService.Setup(m => m.GetAllLocations())
            .ReturnsAsync(new List<LocationDto>());

        var locationSelectVM = new LocationSelectVM(_mockMenuService.Object);

        await locationSelectVM.InitializeLocationsAsync();

        Assert.True(locationSelectVM.ErrorOccurred());
    }
}
