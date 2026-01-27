public interface IHttpClientAuth
{
    Task<HttpResponseMessage> DeleteAsync<T>(string requestUri);
    Task<T?> GetAsync<T>(string requestUri);
    Task<HttpResponseMessage> PatchAsync<T>(string requestUri, T content);
    Task<HttpResponseMessage> PostAsync<T>(string requestUri, T content);
    Task<HttpResponseMessage> PutAsync<T>(string requestUri, T content);
}