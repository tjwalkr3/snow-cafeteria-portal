using Cafeteria.Shared.DTOs.Swipe;

namespace Cafeteria.Management.Services.Customers;

public interface ICustomerService
{
    Task<List<CustomerSwipeDto>> GetAllCustomers();
}
