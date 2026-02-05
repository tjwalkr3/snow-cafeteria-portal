using Cafeteria.Customer.Services.Auth;

namespace Cafeteria.Customer.Services.Customer;

public class CustomerService(IHttpClientAuth client, ILogger<CustomerService> logger) : ICustomerService
{
    public async Task RegisterOrUpdateCustomerAsync()
    {
        try
        {
            var response = await client.PostAsync<object>("customer/check", new { });
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to register or update customer.");
        }
    }
}
