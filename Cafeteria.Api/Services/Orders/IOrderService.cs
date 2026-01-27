using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Api.Services;

public interface IOrderService
{
    Task<OrderDto> CreateOrder(CreateOrderDto createOrderDto);
    Task<OrderDto?> GetOrderById(int id);
    Task<List<OrderDto>> GetAllOrders();
}
