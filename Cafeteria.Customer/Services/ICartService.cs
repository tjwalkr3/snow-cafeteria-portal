namespace Cafeteria.Customer.Services;

public interface ICartService
{
    Task<BrowserOrder?> GetOrder(string key);
}