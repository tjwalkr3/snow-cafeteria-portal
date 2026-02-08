using System.Net.Http.Headers;

namespace Cafeteria.Customer.Services.Customer;

public class CustomerService(HttpClient httpClient) : ICustomerService
{
    public async Task RegisterOrUpdateCustomerAsync(string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PostAsJsonAsync("customer/check", new { });
        response.EnsureSuccessStatusCode();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
