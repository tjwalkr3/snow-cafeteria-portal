using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Management.Services.Orders;

namespace Cafeteria.Management.Components.Pages.Order;

public partial class OrderDetail : ComponentBase
{
    [Parameter]
    public int OrderId { get; set; }

    [Inject]
    public IOrderService OrderService { get; set; } = default!;

    private OrderWithCustomerDto? Order { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Order = await OrderService.GetOrderWithCustomerById(OrderId);
    }
}
