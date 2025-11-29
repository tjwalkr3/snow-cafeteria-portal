using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Cafeteria.Management.Services;

public class HttpClientAuth : IHttpClientAuth
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpClientAuth(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task AddAuthenticationHeaderAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var accessToken = await httpContext.GetTokenAsync("access_token");
            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }

    public async Task<T?> GetAsync<T>(string requestUri)
    {
        await AddAuthenticationHeaderAsync();
        return await _httpClient.GetFromJsonAsync<T>(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string requestUri, T content)
    {
        await AddAuthenticationHeaderAsync();
        return await _httpClient.PostAsJsonAsync(requestUri, content);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string requestUri, T content)
    {
        await AddAuthenticationHeaderAsync();
        return await _httpClient.PutAsJsonAsync(requestUri, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync<T>(string requestUri)
    {
        await AddAuthenticationHeaderAsync();
        return await _httpClient.DeleteAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PatchAsync<T>(string requestUri, T content)
    {
        await AddAuthenticationHeaderAsync();
        return await _httpClient.PatchAsJsonAsync(requestUri, content);
    }
}
