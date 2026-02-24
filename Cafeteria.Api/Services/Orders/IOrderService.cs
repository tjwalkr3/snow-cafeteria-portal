using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Api.Services.Orders;

public interface IOrderService
{
    Task<OrderDto> CreateOrder(BrowserOrder browserOrder, string customerEmail);
    Task<OrderDto?> GetOrderById(int id);
    Task<List<OrderDto>> GetAllOrders();
    Task<List<OrderWithCustomerDto>> GetAllOrdersWithCustomer();
    Task<List<OrderWithCustomerDto>> GetOrdersByCustomer(int badgerId);
    Task<List<OrderDto>> GetOrdersByCustomerEmail(string email);
    Task<OrderWithCustomerDto?> GetOrderWithCustomerById(int id);
}
