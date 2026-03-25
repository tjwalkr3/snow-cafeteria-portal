using Cafeteria.Customer;
using Cafeteria.Customer.Components.Pages.StationSelect;
using Cafeteria.Customer.Services;
using Cafeteria.Customer.Services.Menu;
using Cafeteria.Shared.DTOs.Menu;
using Moq;

namespace Cafeteria.Customer.ViewModels.Tests;

public class StationSelectVMTests
{
    private readonly Mock<IApiMenuService> _mockMenuService;

    public StationSelectVMTests()
    {
        _mockMenuService = new Mock<IApiMenuService>();
    }

    [Fact]
    public async Task ErrorOccurredWhileParsingSelectedLocation_ReturnsTrue_WhenInitializeStationsFails()
    {
        _mockMenuService.Setup(m => m.GetStationsByLocation(It.IsAny<int>()))
            .ThrowsAsync(new Exception("API Error"));

        var stationSelectVM = new StationSelectVM(_mockMenuService.Object);

        await stationSelectVM.InitializeStations(1);

        Assert.True(stationSelectVM.ErrorOccurredWhileParsingSelectedLocation());
    }

    [Fact]
    public async Task InitializeStations_SetsStations_WhenApiCallSucceeds()
    {
        var expectedStations = new List<StationDto>
        {
            new StationDto { Id = 1, StationName = "Station 1" },
            new StationDto { Id = 2, StationName = "Station 2" }
        };

        _mockMenuService.Setup(m => m.GetStationsByLocation(1))
            .ReturnsAsync(expectedStations);

        // Mock location business hours for all days of the week (1-7 where 7 = Sunday)
        // Set hours as 00:00 to 23:59 to ensure test passes regardless of when it runs
        var locationBusinessHours = Enumerable.Range(1, 7)
            .Select(weekday => new LocationBusinessHoursDto
            {
                Id = weekday,
                LocationId = 1,
                WeekdayId = weekday,
                OpenTime = new TimeOnly(0, 0),
                CloseTime = new TimeOnly(23, 59)
            })
            .ToList();

        _mockMenuService.Setup(m => m.GetLocationBusinessHours(It.IsAny<int>()))
            .ReturnsAsync(locationBusinessHours);

        _mockMenuService.Setup(m => m.GetLocationExceptions(It.IsAny<int>()))
            .ReturnsAsync(new List<LocationExceptionHoursDto>());

        // Mock business hours for all days of the week (1-7 where 7 = Sunday)
        // Set hours as 00:00 to 23:59 to ensure test passes regardless of when it runs
        var businessHours = Enumerable.Range(1, 7)
            .Select(weekday => new StationBusinessHoursDto
            {
                Id = weekday,
                StationId = 0,
                WeekdayId = weekday,
                OpenTime = new TimeOnly(0, 0),
                CloseTime = new TimeOnly(23, 59)
            })
            .ToList();

        _mockMenuService.Setup(m => m.GetStationBusinessHours(It.IsAny<int>()))
            .ReturnsAsync(businessHours);

        _mockMenuService.Setup(m => m.GetStationExceptions(It.IsAny<int>()))
            .ReturnsAsync(new List<StationExceptionHoursDto>());

        var stationSelectVM = new StationSelectVM(_mockMenuService.Object);

        await stationSelectVM.InitializeStations(1);

        Assert.Equal(expectedStations, stationSelectVM.Stations);
        Assert.False(stationSelectVM.ErrorOccurredWhileParsingSelectedLocation());
    }
}
