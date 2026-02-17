using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Management.Services.Orders;

namespace Cafeteria.Management.Components.Pages.Order;

public class OrderVM : IOrderVM
{
    private readonly IOrderService _orderService;

    public List<OrderWithCustomerDto> Orders { get; set; } = [];

    public OrderVM(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task LoadOrders()
    {
        try
        {
            Orders = await _orderService.GetAllOrdersWithCustomer();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading orders: {ex.Message}");
        }
    }
}
