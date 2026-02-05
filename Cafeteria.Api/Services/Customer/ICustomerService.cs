public interface ICustomerService
{
    Task EnsureCustomerExists(string email, string custName);
}