using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Cafeteria.Customer.Services;

public class ProtectedStorageWrapper : IProtectedStorageWrapper
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;

    public ProtectedStorageWrapper(ProtectedLocalStorage protectedLocalStorage)
    {
        _protectedLocalStorage = protectedLocalStorage;
    }

    public async ValueTask<StorageResult<T>> GetAsync<T>(string key)
    {
        var result = await _protectedLocalStorage.GetAsync<T>(key);
        return new StorageResult<T>(result.Success, result.Value);
    }
}
