using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using Cafeteria.Management.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using Xunit;

namespace Cafeteria.UnitTests.Management.Services;

public class HttpClientAuthTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly HttpClientAuth _httpClientAuth;
    private const string Token = "test-token";

    public HttpClientAuthTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://test.com")
        };

        SetupHttpContextWithToken(Token);
        _httpClientAuth = new HttpClientAuth(_httpClient, _mockHttpContextAccessor.Object);
    }

    private void SetupHttpContextWithToken(string token)
    {
        var context = new DefaultHttpContext();
        var authResult = AuthenticateResult.Success(
            new AuthenticationTicket(
                new ClaimsPrincipal(),
                new AuthenticationProperties(new Dictionary<string, string?>
                {
                    { ".Token.access_token", token }
                }),
                "TestScheme"));

        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        context.RequestServices = serviceProviderMock.Object;
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
    }

    private void SetupHandler(HttpMethod method)
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method &&
                    req.Headers.Authorization != null &&
                    req.Headers.Authorization.Scheme == "Bearer" &&
                    req.Headers.Authorization.Parameter == Token),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });
    }

    private void VerifyHandler(HttpMethod method)
    {
        _mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == method &&
                req.Headers.Authorization != null &&
                req.Headers.Authorization.Scheme == "Bearer" &&
                req.Headers.Authorization.Parameter == Token),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAsync_AddsAuthenticationHeader()
    {
        SetupHandler(HttpMethod.Get);
        await _httpClientAuth.GetAsync<object>("test-url");
        VerifyHandler(HttpMethod.Get);
    }

    [Fact]
    public async Task PostAsync_AddsAuthenticationHeader()
    {
        SetupHandler(HttpMethod.Post);
        await _httpClientAuth.PostAsync<object>("test-url", new { });
        VerifyHandler(HttpMethod.Post);
    }

    [Fact]
    public async Task PutAsync_AddsAuthenticationHeader()
    {
        SetupHandler(HttpMethod.Put);
        await _httpClientAuth.PutAsync<object>("test-url", new { });
        VerifyHandler(HttpMethod.Put);
    }

    [Fact]
    public async Task DeleteAsync_AddsAuthenticationHeader()
    {
        SetupHandler(HttpMethod.Delete);
        await _httpClientAuth.DeleteAsync<object>("test-url");
        VerifyHandler(HttpMethod.Delete);
    }

    [Fact]
    public async Task PatchAsync_AddsAuthenticationHeader()
    {
        SetupHandler(HttpMethod.Patch);
        await _httpClientAuth.PatchAsync<object>("test-url", new { });
        VerifyHandler(HttpMethod.Patch);
    }
}
