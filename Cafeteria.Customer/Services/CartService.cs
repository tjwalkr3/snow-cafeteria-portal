namespace Cafeteria.Customer.Services;

public class CartService : ICartService
{
    private readonly IProtectedStorageWrapper _protectedStorage;

    public CartService(IProtectedStorageWrapper protectedStorage)
    {
        _protectedStorage = protectedStorage;
    }

    public async Task<BrowserOrder?> GetOrder(string key)
    {
        var result = await _protectedStorage.GetAsync<BrowserOrder>(key);
        return result.Success ? result.Value : null;
    }
}