using System.Net.Http.Headers;

namespace Cafeteria.Management.Services.Customers;

public class CustomerRegistrationService(HttpClient httpClient) : ICustomerRegistrationService
{
    public async Task RegisterOrUpdateCustomerAsync(string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PostAsJsonAsync("customer/check", new { });
        response.EnsureSuccessStatusCode();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
