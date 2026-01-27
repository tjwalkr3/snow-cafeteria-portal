using Cafeteria.Shared.DTOs.Order;


namespace Cafeteria.Customer.Services.Order;

public class ApiOrderService(HttpClient client) : IApiOrderService
{
    public async Task<OrderDto> CreateOrder(CreateOrderDto createOrderDto)
    {
        var response = await client.PostAsJsonAsync("order", createOrderDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDto>()
            ?? throw new InvalidOperationException("Failed to create order");
    }

    public async Task<OrderDto?> GetOrderById(int id)
    {
        if (id < 1)
            throw new ArgumentOutOfRangeException(nameof(id));

        var response = await client.GetAsync($"order/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }

    public async Task<List<OrderDto>> GetAllOrders()
    {
        var response = await client.GetAsync("order");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<OrderDto>>() ?? new List<OrderDto>();
    }
}
