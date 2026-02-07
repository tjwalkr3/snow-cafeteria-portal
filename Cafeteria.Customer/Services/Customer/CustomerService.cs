using Cafeteria.Customer.Services.Auth;

namespace Cafeteria.Customer.Services.Customer;

public class CustomerService(IHttpClientAuth client) : ICustomerService
{
    public async Task RegisterOrUpdateCustomerAsync()
    {
        var response = await client.PostAsync<object>("customer/check", new { });
        response.EnsureSuccessStatusCode();
    }
}
