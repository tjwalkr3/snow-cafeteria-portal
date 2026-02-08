using Cafeteria.Customer.Services.Auth;
using Cafeteria.Shared.DTOs.Order;


namespace Cafeteria.Customer.Services.Order;

public class ApiOrderService(IHttpClientAuth client) : IApiOrderService
{
    public async Task<OrderDto> CreateOrder(CreateOrderDto createOrderDto)
    {
        var response = await client.PostAsync("order", createOrderDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDto>()
            ?? throw new InvalidOperationException("Failed to create order");
    }

    public async Task<OrderDto?> GetOrderById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        return await client.GetAsync<OrderDto>($"order/{id}");
    }

    public async Task<List<OrderDto>> GetAllOrders()
    {
        return await client.GetAsync<List<OrderDto>>("order") ?? new List<OrderDto>();
    }

    public async Task<List<OrderDto>> GetOrdersByCustomerEmail()
    {
        return await client.GetAsync<List<OrderDto>>("order/customer-email") ?? new List<OrderDto>();
    }
}
