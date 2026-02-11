using Cafeteria.Shared.DTOs.Swipe;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.Customers;

public class CustomerService(IHttpClientAuth client) : ICustomerService
{
    public async Task<List<CustomerSwipeDto>> GetAllCustomers()
    {
        return await client.GetAsync<List<CustomerSwipeDto>>("api/swipe/all-customers") ?? [];
    }
}
