using Cafeteria.Customer.Components.Pages.StationSelect;
using Cafeteria.Customer.Services;

namespace Cafeteria.Customer.ViewModels.Tests;

public class StationSelectVMTests
{
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
