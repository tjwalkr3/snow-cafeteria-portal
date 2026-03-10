using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Swipe;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class SwipeIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public SwipeIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
        _connection = _fixture.GetConnection();
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    [Fact]
    public async Task GetSwipesByUserID_ReturnsCorrectSwipeBalance()
    {
        // Use pre-loaded customer with badge ID 1001234 (John Doe with 7 swipes)
        var response = await _client.GetAsync("/api/swipe/1001234");
        response.EnsureSuccessStatusCode();
        var swipe = await response.Content.ReadFromJsonAsync<SwipeDto>();

        Assert.NotNull(swipe);
        Assert.Equal(1001234, swipe.BadgerId);
        Assert.Equal(7, swipe.SwipeBalance);
    }

    [Fact]
    public async Task GetSwipesByUserID_WithDifferentUser_ReturnsCorrectBalance()
    {
        // Use pre-loaded customer with badge ID 1005678 (Jane Smith with 21 swipes)
        var response = await _client.GetAsync("/api/swipe/1005678");
        response.EnsureSuccessStatusCode();
        var swipe = await response.Content.ReadFromJsonAsync<SwipeDto>();

        Assert.NotNull(swipe);
        Assert.Equal(1005678, swipe.BadgerId);
        Assert.Equal(21, swipe.SwipeBalance);
    }

    [Fact]
    public async Task GetSwipesByUserID_WithZeroBalance_ReturnsZero()
    {
        // Use pre-loaded customer with badge ID 1007890 (Charlie Brown with 0 swipes)
        var response = await _client.GetAsync("/api/swipe/1007890");
        response.EnsureSuccessStatusCode();
        var swipe = await response.Content.ReadFromJsonAsync<SwipeDto>();

        Assert.NotNull(swipe);
        Assert.Equal(1007890, swipe.BadgerId);
        Assert.Equal(0, swipe.SwipeBalance);
    }

    [Fact]
    public async Task GetSwipesByUserID_WithNonExistentUser_ReturnsNotFound()
    {
        // Use a badge ID that doesn't exist in the database
        var response = await _client.GetAsync("/api/swipe/9999999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSwipesByUserID_RetrievesFromDatabase()
    {
        // Verify data is actually coming from the database by checking multiple users
        const string sql = @"
            SELECT badger_id AS BadgerId, swipe_balance AS SwipeBalance
            FROM cafeteria.customer_swipe
            WHERE badger_id = @UserId";

        var dbSwipe = await _connection.QuerySingleOrDefaultAsync<SwipeDto>(
            sql,
            new { UserId = 1009012 }
        );

        Assert.NotNull(dbSwipe);
        Assert.Equal(1009012, dbSwipe.BadgerId);
        Assert.Equal(8, dbSwipe.SwipeBalance);

        // Now verify the API returns the same data
        var response = await _client.GetAsync("/api/swipe/1009012");
        response.EnsureSuccessStatusCode();
        var apiSwipe = await response.Content.ReadFromJsonAsync<SwipeDto>();

        Assert.NotNull(apiSwipe);
        Assert.Equal(dbSwipe.BadgerId, apiSwipe.BadgerId);
        Assert.Equal(dbSwipe.SwipeBalance, apiSwipe.SwipeBalance);
    }

    [Fact]
    public async Task GetSwipesByEmail_ReturnsCorrectSwipeBalance()
    {
        // Use pre-loaded customer john.doe@snow.edu (badge ID 1001234 with 7 swipes)
        var response = await _client.GetAsync("/api/swipe/email/john.doe@snow.edu");
        response.EnsureSuccessStatusCode();
        var swipe = await response.Content.ReadFromJsonAsync<SwipeDto>();

        Assert.NotNull(swipe);
        Assert.Equal(1001234, swipe.BadgerId);
        Assert.Equal(7, swipe.SwipeBalance);
    }

    [Fact]
    public async Task GetSwipesByEmail_WithDifferentEmail_ReturnsCorrectBalance()
    {
        // Use pre-loaded customer alice.williams@snow.edu (badge ID 1003456 with 9 swipes)
        var response = await _client.GetAsync("/api/swipe/email/alice.williams@snow.edu");
        response.EnsureSuccessStatusCode();
        var swipe = await response.Content.ReadFromJsonAsync<SwipeDto>();

        Assert.NotNull(swipe);
        Assert.Equal(1003456, swipe.BadgerId);
        Assert.Equal(9, swipe.SwipeBalance);
    }

    [Fact]
    public async Task GetSwipesByEmail_WithNonExistentEmail_ReturnsNoContent()
    {
        // Use an email that doesn't exist in the database
        var response = await _client.GetAsync("/api/swipe/email/nonexistent@snow.edu");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetAllCustomers_ReturnsAllCustomers()
    {
        var response = await _client.GetAsync("/api/swipe/all-customers");
        response.EnsureSuccessStatusCode();
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerSwipeDto>>();

        Assert.NotNull(customers);
        Assert.Equal(6, customers.Count); // 6 pre-loaded customers

        // Verify data structure and some sample data
        var johnDoe = customers.FirstOrDefault(c => c.BadgerId == 1001234);
        Assert.NotNull(johnDoe);
        Assert.Equal("John Doe", johnDoe.CustName);
        Assert.Equal("john.doe@snow.edu", johnDoe.Email);
        Assert.Equal(7, johnDoe.SwipeCount);
        Assert.Equal("Active", johnDoe.Status);

        var janeSmith = customers.FirstOrDefault(c => c.BadgerId == 1005678);
        Assert.NotNull(janeSmith);
        Assert.Equal("Jane Smith", janeSmith.CustName);
        Assert.Equal(21, janeSmith.SwipeCount);
        Assert.Equal("Active", janeSmith.Status);
    }

    [Fact]
    public async Task GetAllCustomers_IncludesCustomersWithZeroBalance()
    {
        var response = await _client.GetAsync("/api/swipe/all-customers");
        response.EnsureSuccessStatusCode();
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerSwipeDto>>();

        Assert.NotNull(customers);

        // Verify Charlie Brown with 0 swipes is included
        var charlieBrown = customers.FirstOrDefault(c => c.BadgerId == 1007890);
        Assert.NotNull(charlieBrown);
        Assert.Equal("Charlie Brown", charlieBrown.CustName);
        Assert.Equal(0, charlieBrown.SwipeCount);
        Assert.Equal("Active", charlieBrown.Status);
    }

    [Fact]
    public async Task GetAllCustomers_WithExpiredSwipes_ReturnsExpiredStatus()
    {
        // Setup: Insert a customer with expired swipes
        const string insertCustomerSql = @"
            INSERT INTO cafeteria.customer (badger_id, cust_name, email)
            VALUES (@BadgerId, @CustName, @Email)
            ON CONFLICT DO NOTHING";

        const string insertExpiredSwipeSql = @"
            INSERT INTO cafeteria.customer_swipe (badger_id, swipe_balance, end_date)
            VALUES (@BadgerId, @SwipeBalance, @EndDate)
            ON CONFLICT DO NOTHING";

        var expiredBadgerId = 9999001;
        var expiredDate = DateTime.UtcNow.AddDays(-7); // 7 days in the past

        await _connection.ExecuteAsync(insertCustomerSql, new
        {
            BadgerId = expiredBadgerId,
            CustName = "Expired Customer",
            Email = "expired@test.edu"
        });

        await _connection.ExecuteAsync(insertExpiredSwipeSql, new
        {
            BadgerId = expiredBadgerId,
            SwipeBalance = 5,
            EndDate = expiredDate
        });

        // Act
        var response = await _client.GetAsync("/api/swipe/all-customers");
        response.EnsureSuccessStatusCode();
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerSwipeDto>>();

        // Assert
        Assert.NotNull(customers);
        var expiredCustomer = customers.FirstOrDefault(c => c.BadgerId == expiredBadgerId);
        Assert.NotNull(expiredCustomer);
        Assert.Null(expiredCustomer.SwipeCount);
        Assert.Equal("Expired", expiredCustomer.Status);

        // Cleanup
        await _connection.ExecuteAsync("DELETE FROM cafeteria.customer_swipe WHERE badger_id = @BadgerId", new { BadgerId = expiredBadgerId });
        await _connection.ExecuteAsync("DELETE FROM cafeteria.customer WHERE badger_id = @BadgerId", new { BadgerId = expiredBadgerId });
    }

    [Fact]
    public async Task GetAllCustomers_WithNoSwipeEntry_ReturnsNullCount()
    {
        // Setup: Insert a customer with no swipe entry
        const string insertCustomerSql = @"
            INSERT INTO cafeteria.customer (badger_id, cust_name, email)
            VALUES (@BadgerId, @CustName, @Email)
            ON CONFLICT DO NOTHING";

        var noSwipeBadgerId = 9999002;

        await _connection.ExecuteAsync(insertCustomerSql, new
        {
            BadgerId = noSwipeBadgerId,
            CustName = "No Swipes Customer",
            Email = "noswipes@test.edu"
        });

        // Act
        var response = await _client.GetAsync("/api/swipe/all-customers");
        response.EnsureSuccessStatusCode();
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerSwipeDto>>();

        // Assert
        Assert.NotNull(customers);
        var noSwipeCustomer = customers.FirstOrDefault(c => c.BadgerId == noSwipeBadgerId);
        Assert.NotNull(noSwipeCustomer);
        Assert.Null(noSwipeCustomer.SwipeCount);
        Assert.Equal("Not Enrolled", noSwipeCustomer.Status);

        // Cleanup
        await _connection.ExecuteAsync("DELETE FROM cafeteria.customer WHERE badger_id = @BadgerId", new { BadgerId = noSwipeBadgerId });
    }
}