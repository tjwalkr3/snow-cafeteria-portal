using Cafeteria.Customer.Components.ViewModels;
using Cafeteria.Shared.Services;

namespace Cafeteria.Customer.Tests;

public class PlaceOrderVMTests
{
    private DummyMenuService MenuService;

    public PlaceOrderVMTests()
    {
        MenuService = new DummyMenuService();
    }

    [Fact]
    public async Task ErrorOccurredWhileParsingSelectedFoodItem_ReturnsTrue_WhenUrlParsingFails()
    {
        var viewModel = new PlaceOrderVM(MenuService);
        string invalidUri = "https://example.com/placeorder?food-item=invalid-json";

        await viewModel.GetDataFromRouteParameters(invalidUri);

        Assert.True(viewModel.ErrorOccurredWhileParsingSelectedFoodItem());
    }
}
