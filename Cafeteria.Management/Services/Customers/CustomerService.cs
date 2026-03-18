using Cafeteria.Shared.DTOs.Customer;
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

    public async Task<UserRoleDto?> GetCurrentUserRole()
    {
        try
        {
            return await _client.GetAsync<UserRoleDto>("customer/role");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<List<CustomerRoleDto>> GetAllCustomersWithRoles(string? search = null)
    {
        var url = string.IsNullOrWhiteSpace(search)
            ? "customer/all"
            : $"customer/all?search={Uri.EscapeDataString(search)}";
        return await _client.GetAsync<List<CustomerRoleDto>>(url) ?? [];
    }

    public async Task<bool> ToggleFoodServiceRole(string email)
    {
        var response = await _client.PutAsync<object>($"customer/{Uri.EscapeDataString(email)}/food-service-role", new { });
        return response.IsSuccessStatusCode;
    }
}
