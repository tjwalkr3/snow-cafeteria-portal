using Microsoft.JSInterop;
using System.Text.Json;

namespace Cafeteria.Customer.Services;

public class StorageWrapper : IStorageWrapper
{
    private readonly IJSRuntime _jsRuntime;

    public StorageWrapper(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask<StorageResult<T>> GetAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
            {
                return new StorageResult<T>(false, default);
            }
            var value = JsonSerializer.Deserialize<T>(json);
            return new StorageResult<T>(true, value);
        }
        catch
        {
            return new StorageResult<T>(false, default);
        }
    }

    public async ValueTask SetAsync<T>(string key, T value)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting storage key '{key}': {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async ValueTask DeleteAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}