using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Cafeteria.Customer.Services;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public partial class PlaceOrder : ComponentBase
{
    [Inject]
    private IPlaceOrderVM PlaceOrderVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ICartService Cart { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "location")]
    public int Location { get; set; }

    [SupplyParameterFromQuery(Name = "payment")]
    public string? Payment { get; set; }

    private BrowserOrder? Order { get; set; } = null;

    private decimal Price { get; set; } = 0.0m;

    private bool _isLoading = true;

    public bool IsInitialized { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        await PlaceOrderVM.InitializeLocations();
        IsInitialized = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(async () =>
            {
                string userName = "order";

                await SavePaymentMethod(userName);
                await SaveLocation(userName);

                Order = await GetOrder(userName);

                if (Order != null)
                {
                    Price = PlaceOrderVM.CalculateTotalPrice(Order);
                }

                _isLoading = false;
                StateHasChanged();
            });
        }
    }

    private async Task SavePaymentMethod(string userName)
    {
        if (!string.IsNullOrEmpty(Payment))
        {
            await Cart.SetIsCardOrder(userName, Payment == "card");
        }
    }

    private async Task SaveLocation(string userName)
    {
        if (Location != 0)
        {
            var locationDto = PlaceOrderVM.GetLocationById(Location);
            if (locationDto != null)
            {
                await Cart.SetLocation(userName, locationDto);
            }
        }
    }

    private async Task<BrowserOrder?> GetOrder(string userName)
    {
        return await Cart.GetOrder(userName);
    }

    public string GetStationSelectUrl()
    {
        Dictionary<string, string?> queryParameters = new() { };

        if (Order != null)
        {
            string payment = Order.IsCardOrder ? "card" : "swipe";
            queryParameters.Add("payment", payment);

            if (Order.Location != null)
            {
                queryParameters.Add("location", Order.Location.Id.ToString());
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(Payment))
                queryParameters.Add("payment", Payment);

            if (Location != 0)
                queryParameters.Add("location", Location.ToString());
        }

        return QueryHelpers.AddQueryString("/station-select", queryParameters);
    }

    private async Task HandlePlaceOrder()
    {
        // TODO: Implement order submission logic
        // For now, show a simple alert or navigate to a confirmation page
        Console.WriteLine("Order placed!");
        
        // Clear the cart after placing order
        await Cart.ClearOrder("order");
        
        // Navigate to a success page or back to home
        Navigation.NavigateTo("/", true);
    }
}