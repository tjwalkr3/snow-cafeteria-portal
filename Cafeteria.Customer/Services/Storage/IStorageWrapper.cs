namespace Cafeteria.Customer.Services.Storage;

public interface IStorageWrapper
{
    ValueTask<StorageResult<T>> GetAsync<T>(string key);
    ValueTask SetAsync<T>(string key, T value);
    ValueTask DeleteAsync(string key);
}

public record StorageResult<T>(bool Success, T? Value);
