using Cafeteria.Customer.Components.Pages.ItemSelect;
using Cafeteria.Customer.Services;

namespace Cafeteria.Customer.ViewModels.Tests;

public class ItemSelectVMTests
{
    [Fact]
    public async Task ErrorOccurredWhileParsingSelectedStation_ReturnsTrue_WhenUrlParsingFailedIsTrue()
    {
        // Arrange
        var itemSelectVM = new ItemSelectVM(new DummyMenuService()); // menu service isn't needed in this test

        // Act
        await itemSelectVM.GetDataFromRouteParameters(""); // pass a url with no query string so parsing fails
        bool result = itemSelectVM.ErrorOccurredWhileParsingSelectedStation();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ErrorOccurredWhileParsingSelectedStation_ReturnsFalse_WhenUrlParsingFailedIsFalse()
    {
        // Arrange
        var itemSelectVM = new ItemSelectVM(new DummyMenuService()); // menu service isn't needed in this test

        // Act
        // Don't parse; error occurred should be false by default
        bool result = itemSelectVM.ErrorOccurredWhileParsingSelectedStation();

        // Assert
        Assert.False(result);
    }

}
