using Cafeteria.Shared.DTOs.Customer;

public interface ICustomerService
{
    Task EnsureCustomerExists(string email, string custName);
    Task<List<CustomerRoleDto>> GetAllCustomersWithRoles(string? search = null);
    Task<bool> ToggleFoodServiceRole(string email);
}