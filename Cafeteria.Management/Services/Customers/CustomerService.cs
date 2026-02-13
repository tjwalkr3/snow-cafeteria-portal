using Cafeteria.Shared.DTOs.Swipe;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.Customers;

public class CustomerService : ICustomerService
{
    private readonly IHttpClientAuth _client;

    public CustomerService(IHttpClientAuth client)
    {
        _client = client;
    }

    public async Task<List<CustomerSwipeDto>> GetAllCustomers()
    {
        return await _client.GetAsync<List<CustomerSwipeDto>>("swipe/all-customers") ?? [];
    }
}
