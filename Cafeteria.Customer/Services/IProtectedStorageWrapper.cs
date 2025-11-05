namespace Cafeteria.Customer.Services;

public interface IProtectedStorageWrapper
{
    ValueTask<StorageResult<T>> GetAsync<T>(string key);
    ValueTask SetAsync<T>(string key, T value);
    ValueTask DeleteAsync(string key);
}

public record StorageResult<T>(bool Success, T? Value);
