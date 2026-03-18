using System.Net;
using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Customer;
using Dapper;
using Npgsql;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class CustomerRoleIntegrationTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    private const string ToggleTargetEmail = "toggle-target@example.com";

    public CustomerRoleIntegrationTests(DatabaseFixture fixture)
    {
        _client = fixture.Client;
        _connection = fixture.GetConnection();

        _connection.Execute(@"
            INSERT INTO cafeteria.customer (email, cust_name, user_role)
            VALUES (@Email, 'Toggle Target', NULL)
            ON CONFLICT (email) DO UPDATE SET user_role = NULL",
            new { Email = ToggleTargetEmail });
    }

    public void Dispose()
    {
        _connection.Execute("DELETE FROM cafeteria.customer WHERE email = @Email", new { Email = ToggleTargetEmail });
        _connection?.Dispose();
    }

    [Fact]
    public async Task GetAllCustomers_AsAdmin_ReturnsOkWithList()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer/all");
        request.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "admin@example.com");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<CustomerRoleDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAllCustomers_AsFoodServiceUser_ReturnsUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer/all");
        request.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "test-2@example.com");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ToggleFoodServiceRole_AsAdmin_TogglesRoleOnAndOff()
    {
        using var grantRequest = new HttpRequestMessage(HttpMethod.Put,
            $"/api/customer/{Uri.EscapeDataString(ToggleTargetEmail)}/food-service-role");
        grantRequest.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "admin@example.com");

        var grantResponse = await _client.SendAsync(grantRequest);
        Assert.Equal(HttpStatusCode.OK, grantResponse.StatusCode);

        var roleAfterGrant = await _connection.QuerySingleOrDefaultAsync<string?>(
            "SELECT user_role FROM cafeteria.customer WHERE email = @Email", new { Email = ToggleTargetEmail });
        Assert.Equal("food-service", roleAfterGrant);

        using var revokeRequest = new HttpRequestMessage(HttpMethod.Put,
            $"/api/customer/{Uri.EscapeDataString(ToggleTargetEmail)}/food-service-role");
        revokeRequest.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "admin@example.com");

        var revokeResponse = await _client.SendAsync(revokeRequest);
        Assert.Equal(HttpStatusCode.OK, revokeResponse.StatusCode);

        var roleAfterRevoke = await _connection.QuerySingleOrDefaultAsync<string?>(
            "SELECT user_role FROM cafeteria.customer WHERE email = @Email", new { Email = ToggleTargetEmail });
        Assert.Null(roleAfterRevoke);
    }

    [Fact]
    public async Task ToggleFoodServiceRole_TargetingAdminUser_ReturnsBadRequest()
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, "/api/customer/admin%40example.com/food-service-role");
        request.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "admin@example.com");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ToggleFoodServiceRole_AsFoodServiceUser_ReturnsUnauthorized()
    {
        using var request = new HttpRequestMessage(HttpMethod.Put,
            $"/api/customer/{Uri.EscapeDataString(ToggleTargetEmail)}/food-service-role");
        request.Headers.Add(MockAuthenticationHandler.TestEmailHeader, "test-2@example.com");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
