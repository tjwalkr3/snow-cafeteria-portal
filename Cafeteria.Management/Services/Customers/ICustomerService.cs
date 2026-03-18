using Cafeteria.Shared.DTOs.Customer;
using Cafeteria.Shared.DTOs.Swipe;

namespace Cafeteria.Management.Services.Customers;

public interface ICustomerService
{
    Task<List<CustomerSwipeDto>> GetAllCustomers();
    Task<UserRoleDto?> GetCurrentUserRole();
    Task<List<CustomerRoleDto>> GetAllCustomersWithRoles(string? search = null);
    Task<bool> ToggleFoodServiceRole(string email);
}
