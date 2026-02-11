using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.Orders;

public class OrderService(IHttpClientAuth client) : IOrderService
{
    public async Task<List<OrderWithCustomerDto>> GetAllOrdersWithCustomer()
    {
        return await client.GetAsync<List<OrderWithCustomerDto>>("api/order/with-customer") ?? [];
    }

    public async Task<List<OrderWithCustomerDto>> GetOrdersByCustomer(int badgerId)
    {
        return await client.GetAsync<List<OrderWithCustomerDto>>($"api/order/customer/{badgerId}") ?? [];
    }

    public async Task<OrderWithCustomerDto?> GetOrderWithCustomerById(int id)
    {
        return await client.GetAsync<OrderWithCustomerDto>($"api/order/with-customer/{id}");
    }
}
