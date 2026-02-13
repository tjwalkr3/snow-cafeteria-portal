namespace Cafeteria.Shared.Services.Customer;

public interface ICustomerRegistrationService
{
    Task RegisterOrUpdateCustomerAsync(string accessToken);
}
