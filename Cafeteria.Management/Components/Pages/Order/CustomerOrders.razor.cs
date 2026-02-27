using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Management.Services.Orders;

namespace Cafeteria.Management.Components.Pages.Order;

public partial class CustomerOrders : ComponentBase
{
    [Parameter]
    public int BadgerId { get; set; }

    [Inject]
    public IOrderService OrderService { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    private List<OrderWithCustomerDto> Orders { get; set; } = [];
    private string searchText = string.Empty;
    private string paymentTypeFilter = string.Empty;

    private List<OrderWithCustomerDto> FilteredOrders
    {
        get
        {
            var orders = Orders.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                orders = orders.Where(o =>
                    o.Id.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (!string.IsNullOrWhiteSpace(paymentTypeFilter))
            {
                orders = orders.Where(o => o.PaymentType == paymentTypeFilter);
            }

            return orders.ToList();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Orders = await OrderService.GetOrdersByCustomer(BadgerId);
    }

    private void OnSearchChanged(ChangeEventArgs e)
    {
        searchText = e.Value?.ToString() ?? string.Empty;
        StateHasChanged();
    }

    private void OnPaymentTypeChanged(ChangeEventArgs e)
    {
        paymentTypeFilter = e.Value?.ToString() ?? string.Empty;
        StateHasChanged();
    }

    private void NavigateToOrder(int orderId)
    {
        Navigation.NavigateTo($"orders/{orderId}");
    }
}
