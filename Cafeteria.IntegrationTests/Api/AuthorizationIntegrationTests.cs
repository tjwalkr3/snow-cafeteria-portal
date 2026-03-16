using System.Net;
using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Swipe;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class AuthorizationIntegrationTests
{
    private readonly HttpClient _client;

    public AuthorizationIntegrationTests(DatabaseFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task RestrictedEndpoint_WithNoRoleInDatabase_ReturnsUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/swipe/all-customers");
        request.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "test-1@example.com");
        request.Headers.Add(MockAuthenticationHandler.TestRoleHeader, "admin");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RestrictedEndpoint_WithFoodServiceRole_ReturnsOk()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/swipe/all-customers");
        request.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "test-2@example.com");
        request.Headers.Add(MockAuthenticationHandler.TestRoleHeader, "admin");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<CustomerSwipeDto>>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
    }

    [Fact]
    public async Task RestrictedEndpoint_WithAdminRole_ReturnsOk()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/swipe/all-customers");
        request.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "admin@example.com");
        request.Headers.Add(MockAuthenticationHandler.TestRoleHeader, "admin");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<CustomerSwipeDto>>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
    }
}
