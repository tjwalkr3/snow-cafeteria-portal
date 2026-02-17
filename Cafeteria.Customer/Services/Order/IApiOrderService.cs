using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Customer.Services.Order;

public interface IApiOrderService
{
    Task<OrderDto> CreateOrder(CreateOrderDto createOrderDto);
    Task<OrderDto?> GetOrderById(int id);
    Task<List<OrderDto>> GetAllOrders();
    Task<List<OrderDto>> GetOrdersByCustomerEmail();
}
