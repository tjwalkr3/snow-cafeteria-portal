using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Management.Components.Pages.Order;

public partial class Order : ComponentBase
{
    [Inject]
    public IOrderVM ViewModel { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;

    private string searchText = string.Empty;
    private string paymentTypeFilter = string.Empty;

    private List<OrderWithCustomerDto> FilteredOrders
    {
        get
        {
            var orders = ViewModel.Orders.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                orders = orders.Where(o =>
                    o.Id.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (o.CustomerName != null && o.CustomerName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (o.CustomerEmail != null && o.CustomerEmail.Contains(searchText, StringComparison.OrdinalIgnoreCase))
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
        await ViewModel.LoadOrders();
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
