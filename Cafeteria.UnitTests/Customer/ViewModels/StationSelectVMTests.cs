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

        var stationSelectVM = new StationSelectVM(_mockMenuService.Object);

        await stationSelectVM.InitializeStations(1);

        Assert.Equal(expectedStations, stationSelectVM.Stations);
        Assert.False(stationSelectVM.ErrorOccurredWhileParsingSelectedLocation());
    }
}
