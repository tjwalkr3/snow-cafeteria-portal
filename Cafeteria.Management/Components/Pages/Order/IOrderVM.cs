using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Management.Components.Pages.Order;

public interface IOrderVM
{
    List<OrderWithCustomerDto> Orders { get; set; }
    Task LoadOrders();
}
