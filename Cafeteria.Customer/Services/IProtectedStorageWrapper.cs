namespace Cafeteria.Customer.Services;

public interface IProtectedStorageWrapper
{
    ValueTask<StorageResult<T>> GetAsync<T>(string key);
}

public record StorageResult<T>(bool Success, T? Value);
