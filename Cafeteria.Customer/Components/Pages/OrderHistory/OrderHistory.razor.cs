using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Customer.Components.Pages.OrderHistory;

public partial class OrderHistory : ComponentBase
{
    [Inject]
    private IOrderHistoryVM OrderHistoryVM { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    public bool IsInitialized { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated ?? false)
        {
            await OrderHistoryVM.InitializeAsync();
        }
        
        IsInitialized = true;
    }

    private void SelectOrder(OrderDto order)
    {
        OrderHistoryVM.SelectOrder(order);
    }

    private void ApplyFilter(string? filterType)
    {
        OrderHistoryVM.SetFilter(filterType);
    }

    private void LoadMore()
    {
        OrderHistoryVM.LoadMoreOrders();
        StateHasChanged();
    }
}
