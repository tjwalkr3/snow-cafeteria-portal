using Cafeteria.Customer.Components.Pages.PlaceOrder;
using Cafeteria.Customer.Services;
using Moq;

namespace Cafeteria.Customer.Tests;

public class PlaceOrderVMTests
{
    private Mock<IApiMenuService> MockMenuService;

    public PlaceOrderVMTests()
    {
        MockMenuService = new Mock<IApiMenuService>();
    }

    [Fact]
    public async Task ErrorOccurredWhileParsingSelectedFoodItem_ReturnsTrue_WhenUrlParsingFails()
    {
        var viewModel = new PlaceOrderVM(MockMenuService.Object);
        string invalidUri = "https://example.com/placeorder?food-item=invalid-json";

        await viewModel.GetDataFromRouteParameters(invalidUri);

        Assert.True(viewModel.ErrorOccurredWhileParsingSelectedFoodItem());
    }
}
