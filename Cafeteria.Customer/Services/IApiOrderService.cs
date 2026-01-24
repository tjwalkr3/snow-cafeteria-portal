using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Services;

public interface IApiOrderService
{
    Task<OrderDto> CreateOrder(CreateOrderDto createOrderDto);
    Task<OrderDto?> GetOrderById(int id);
    Task<List<OrderDto>> GetAllOrders();
}
