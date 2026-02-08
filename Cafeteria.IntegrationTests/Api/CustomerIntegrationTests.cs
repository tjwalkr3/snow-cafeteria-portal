using System.Net.Http.Json;
using Dapper;
using Npgsql;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class CustomerIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public CustomerIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
        _connection = _fixture.GetConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    private async Task CleanupCustomerAndRelatedDataAsync(string email)
    {
        // Delete in reverse order of dependencies to respect foreign key constraints
        const string deleteOptionsQuery = @"
            DELETE FROM cafeteria.food_item_option 
            WHERE food_item_order_id IN (
                SELECT fi.id FROM cafeteria.food_item fi 
                INNER JOIN cafeteria.order o ON fi.order_id = o.id 
                WHERE o.customer_badger_id = (SELECT badger_id FROM cafeteria.customer WHERE email = @Email)
            )";
        await _connection.ExecuteAsync(deleteOptionsQuery, new { Email = email });
        
        const string deleteFoodItemsQuery = @"
            DELETE FROM cafeteria.food_item 
            WHERE order_id IN (
                SELECT id FROM cafeteria.order 
                WHERE customer_badger_id = (SELECT badger_id FROM cafeteria.customer WHERE email = @Email)
            )";
        await _connection.ExecuteAsync(deleteFoodItemsQuery, new { Email = email });
        
        const string deleteSaleCardQuery = @"
            DELETE FROM cafeteria.sale_card 
            WHERE order_id IN (
                SELECT id FROM cafeteria.order 
                WHERE customer_badger_id = (SELECT badger_id FROM cafeteria.customer WHERE email = @Email)
            )";
        await _connection.ExecuteAsync(deleteSaleCardQuery, new { Email = email });
        
        const string deleteSaleSwipeQuery = @"
            DELETE FROM cafeteria.sale_swipe 
            WHERE order_id IN (
                SELECT id FROM cafeteria.order 
                WHERE customer_badger_id = (SELECT badger_id FROM cafeteria.customer WHERE email = @Email)
            )";
        await _connection.ExecuteAsync(deleteSaleSwipeQuery, new { Email = email });
        
        const string deleteOrdersQuery = "DELETE FROM cafeteria.order WHERE customer_badger_id = (SELECT badger_id FROM cafeteria.customer WHERE email = @Email)";
        await _connection.ExecuteAsync(deleteOrdersQuery, new { Email = email });
        
        const string deleteCustomerQuery = "DELETE FROM cafeteria.customer WHERE email = @Email";
        await _connection.ExecuteAsync(deleteCustomerQuery, new { Email = email });
    }

    [Fact]
    public async Task RegisterOrUpdate_CreatesNewCustomer_WhenCustomerDoesNotExist()
    {
        // The mock authentication handler provides email "test@example.com" and name "Test User"
        // First, ensure this customer doesn't exist
        await CleanupCustomerAndRelatedDataAsync("test@example.com");

        // Call the endpoint
        var response = await _client.PostAsync("/api/customer/check", null);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(result);

        // Verify the customer was created in the database
        const string checkSql = @"
            SELECT email, badger_id AS BadgerId, cust_name AS CustName
            FROM cafeteria.customer
            WHERE email = @Email";

        var customer = await _connection.QuerySingleOrDefaultAsync(
            checkSql,
            new { Email = "test@example.com" }
        );

        Assert.NotNull(customer);
        Assert.Equal("test@example.com", customer!.email);
        Assert.Equal("Test User", customer.custname);
        Assert.True(customer.badgerid > 0, "badger_id should be auto-generated and greater than 0");
    }

    [Fact]
    public async Task RegisterOrUpdate_DoesNotCreateDuplicate_WhenCustomerAlreadyExists()
    {
        // Ensure the customer exists (let badger_id auto-generate)
        const string insertSql = @"
            INSERT INTO cafeteria.customer (email, cust_name)
            VALUES (@Email, @CustName)
            ON CONFLICT (email) DO NOTHING";

        await _connection.ExecuteAsync(insertSql, new
        {
            Email = "test@example.com",
            CustName = "Test User"
        });

        // Get the count before calling the endpoint
        const string countSql = "SELECT COUNT(*) FROM cafeteria.customer WHERE email = @Email";
        var countBefore = await _connection.ExecuteScalarAsync<int>(
            countSql,
            new { Email = "test@example.com" }
        );

        // Call the endpoint
        var response = await _client.PostAsync("/api/customer/check", null);
        response.EnsureSuccessStatusCode();

        // Verify the count hasn't changed
        var countAfter = await _connection.ExecuteScalarAsync<int>(
            countSql,
            new { Email = "test@example.com" }
        );

        Assert.Equal(countBefore, countAfter);
        Assert.Equal(1, countAfter);
    }

    [Fact]
    public async Task RegisterOrUpdate_ReturnsOk_WithSuccessMessage()
    {
        // Clean up any existing customer
        await CleanupCustomerAndRelatedDataAsync("test@example.com");

        // Call the endpoint
        var response = await _client.PostAsync("/api/customer/check", null);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Customer registered or already exists", content);
    }

    [Fact]
    public async Task RegisterOrUpdate_UsesDefaultBadgerId_WhenNotProvided()
    {
        // Clean up any existing customer
        await CleanupCustomerAndRelatedDataAsync("test@example.com");

        // Call the endpoint
        var response = await _client.PostAsync("/api/customer/check", null);
        response.EnsureSuccessStatusCode();

        // Verify badger_id is auto-generated (should be greater than 0)
        const string checkSql = @"
            SELECT badger_id AS BadgerId
            FROM cafeteria.customer
            WHERE email = @Email";

        var badgerId = await _connection.ExecuteScalarAsync<int>(
            checkSql,
            new { Email = "test@example.com" }
        );

        Assert.True(badgerId > 0, "badger_id should be auto-generated and greater than 0");
    }

    [Fact]
    public async Task RegisterOrUpdate_ExtractsCorrectClaimsFromToken()
    {
        // Clean up any existing customer
        await CleanupCustomerAndRelatedDataAsync("test@example.com");

        // Call the endpoint
        var response = await _client.PostAsync("/api/customer/check", null);
        response.EnsureSuccessStatusCode();

        // Verify the correct email and name were extracted from the mock authentication handler
        const string checkSql = @"
            SELECT email, cust_name AS CustName
            FROM cafeteria.customer
            WHERE email = @Email";

        var customer = await _connection.QuerySingleOrDefaultAsync(
            checkSql,
            new { Email = "test@example.com" }
        );

        Assert.NotNull(customer);
        Assert.Equal("test@example.com", customer!.email);
        Assert.Equal("Test User", customer.custname);
    }
}
