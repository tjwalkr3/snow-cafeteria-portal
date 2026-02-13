namespace Cafeteria.Management.Services.Customers;

public interface ICustomerRegistrationService
{
    Task RegisterOrUpdateCustomerAsync(string accessToken);
}
