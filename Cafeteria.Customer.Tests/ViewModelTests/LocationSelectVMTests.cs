using Cafeteria.Customer.Components.Pages.LocationSelect;
using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Services;
using Moq;

namespace Cafeteria.Customer.ViewModels.Tests;

public class LocationSelectVMTests
{
    private readonly Mock<IApiMenuService> _mockMenuService;

    public LocationSelectVMTests()
    {
        _mockMenuService = new Mock<IApiMenuService>();
    }

    [Fact]
    public void ValidatePaymentParameter_SetsPaymentParameterMissing_WhenPaymentIsNull()
    {
        var locationSelectVM = new LocationSelectVM(_mockMenuService.Object);

        locationSelectVM.ValidatePaymentParameter(null);

        Assert.True(locationSelectVM.ErrorOccurred());
    }

    [Fact]
    public void ValidatePaymentParameter_SetsPaymentParameterMissing_WhenPaymentIsEmpty()
    {
        var locationSelectVM = new LocationSelectVM(_mockMenuService.Object);

        locationSelectVM.ValidatePaymentParameter(string.Empty);

        Assert.True(locationSelectVM.ErrorOccurred());
    }

    [Fact]
    public async Task ValidatePaymentParameter_DoesNotSetError_WhenPaymentIsValid()
    {
        var expectedLocations = new List<LocationDto>
        {
            new LocationDto { Id = 1, LocationName = "Location 1" }
        };

        _mockMenuService.Setup(m => m.GetAllLocations())
            .ReturnsAsync(expectedLocations);

        var locationSelectVM = new LocationSelectVM(_mockMenuService.Object);
        await locationSelectVM.InitializeLocationsAsync();

        locationSelectVM.ValidatePaymentParameter("valid-payment");

        Assert.False(locationSelectVM.ErrorOccurred());
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
