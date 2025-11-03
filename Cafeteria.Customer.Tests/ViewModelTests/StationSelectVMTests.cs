using Cafeteria.Customer.Components.Pages.StationSelect;
using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;
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
    public void ValidateLocationParameter_SetsLocationParameterInvalid_WhenLocationIsZero()
    {
        var stationSelectVM = new StationSelectVM(_mockMenuService.Object);

        stationSelectVM.ValidateParameters(0, "card");

        Assert.True(stationSelectVM.ErrorOccurredWhileParsingSelectedLocation());
    }

    [Fact]
    public void ValidateLocationParameter_SetsLocationParameterInvalid_WhenLocationIsNegative()
    {
        var stationSelectVM = new StationSelectVM(_mockMenuService.Object);

        stationSelectVM.ValidateParameters(-1, "card");

        Assert.True(stationSelectVM.ErrorOccurredWhileParsingSelectedLocation());
    }

    [Fact]
    public void ValidateLocationParameter_SetsPaymentParameterMissing_WhenPaymentIsNull()
    {
        var stationSelectVM = new StationSelectVM(_mockMenuService.Object);

        stationSelectVM.ValidateParameters(1, null);

        Assert.True(stationSelectVM.ErrorOccurredWhileParsingSelectedLocation());
    }

    [Fact]
    public void ValidateLocationParameter_SetsPaymentParameterMissing_WhenPaymentIsEmpty()
    {
        var stationSelectVM = new StationSelectVM(_mockMenuService.Object);

        stationSelectVM.ValidateParameters(1, string.Empty);

        Assert.True(stationSelectVM.ErrorOccurredWhileParsingSelectedLocation());
    }

    [Fact]
    public void ValidateLocationParameter_DoesNotSetErrors_WhenParametersAreValid()
    {
        var stationSelectVM = new StationSelectVM(_mockMenuService.Object);

        stationSelectVM.ValidateParameters(1, "card");

        Assert.False(stationSelectVM.ErrorOccurredWhileParsingSelectedLocation());
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
