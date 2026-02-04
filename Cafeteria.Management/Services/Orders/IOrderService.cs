using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Management.Services.Orders;

public interface IOrderService
{
    Task<List<OrderWithCustomerDto>> GetAllOrdersWithCustomer();
}
