using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Management.Services.Auth;

namespace Cafeteria.Management.Services.Orders;

public class OrderService(IHttpClientAuth client) : IOrderService
{
    public async Task<List<OrderWithCustomerDto>> GetAllOrdersWithCustomer()
    {
        return await client.GetAsync<List<OrderWithCustomerDto>>("api/order/with-customer") ?? [];
    }
}
