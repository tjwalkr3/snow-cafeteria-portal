using Cafeteria.Customer.Components.ViewModels;
using Cafeteria.Customer.Services;

namespace Cafeteria.Customer.ViewModels.Tests;

public class StationSelectVMTests
{
    [Fact]
    public async Task ErrorOccurredWhileParsingSelectedLocation_ReturnsTrue_WhenUrlParsingFailedIsTrue()
    {
        // Arrange
        var stationSelectVM = new StationSelectVM(new DummyMenuService()); // menu service isn't needed in this test

        // Act
        await stationSelectVM.GetDataFromRouteParameters(""); // pass a url with no query string so parsing fails
        bool result = stationSelectVM.ErrorOccurredWhileParsingSelectedLocation();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ErrorOccurredWhileParsingSelectedLocation_ReturnsFalse_WhenUrlParsingFailedIsFalse()
    {
        // Arrange
        var stationSelectVM = new StationSelectVM(new DummyMenuService()); // menu service isn't needed in this test

        // Act
        // Don't parse; error occurred should be false by default
        bool result = stationSelectVM.ErrorOccurredWhileParsingSelectedLocation();

        // Assert
        Assert.False(result);
    }

}
