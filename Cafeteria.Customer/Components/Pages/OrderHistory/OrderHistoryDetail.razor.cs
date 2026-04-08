using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Customer.Components.Pages.OrderHistory;

public partial class OrderHistoryDetail : ComponentBase
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    private IOrderHistoryVM OrderHistoryVM { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    private OrderDto? _order;
    private bool _isInitialized = false;
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User?.Identity?.IsAuthenticated ?? false)
        {
            await OrderHistoryVM.InitializeAsync();
            _order = OrderHistoryVM.FindOrderById(Id);
        }

        _isLoading = false;
        _isInitialized = true;
    }
}
