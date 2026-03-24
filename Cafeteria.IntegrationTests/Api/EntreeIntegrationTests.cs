using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class EntreeIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public EntreeIntegrationTests(DatabaseFixture fixture)
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
    public async Task CreateEntree_AddsNewEntree()
    {
        // Use pre-loaded station with ID 1
        var newEntree = new EntreeDto
        {
            StationId = 1,
            EntreeName = "Test Steak",
            EntreeDescription = "Grilled ribeye steak for testing",
            EntreePrice = 15.99m,
        };

        var response = await _client.PostAsJsonAsync("/api/entree", newEntree);
        response.EnsureSuccessStatusCode();
        var createdEntree = await response.Content.ReadFromJsonAsync<EntreeDto>();

        Assert.NotNull(createdEntree);
        Assert.Equal(newEntree.EntreeName, createdEntree.EntreeName);
        Assert.Equal(newEntree.EntreeDescription, createdEntree.EntreeDescription);
        Assert.Equal(newEntree.EntreePrice, createdEntree.EntreePrice);
        Assert.True(createdEntree.Id > 0);
    }

    [Fact]
    public async Task GetEntreeByID_ReturnsCorrectEntree()
    {
        // Use pre-loaded entree with ID 1
        var response = await _client.GetAsync("/api/entree/1");
        response.EnsureSuccessStatusCode();
        var entree = await response.Content.ReadFromJsonAsync<EntreeDto>();

        Assert.NotNull(entree);
        Assert.Equal("Grilled Chicken", entree.EntreeName);
    }

    [Fact]
    public async Task GetEntreeByID_ReturnsNotFound_WhenEntreeDoesNotExist()
    {
        var response = await _client.GetAsync("/api/entree/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllEntrees_ReturnsAllEntrees()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/entree");
        response.EnsureSuccessStatusCode();
        var entrees = await response.Content.ReadFromJsonAsync<List<EntreeDto>>();

        Assert.NotNull(entrees);
        Assert.True(entrees.Count >= 3);
        Assert.Contains(entrees, e => e.EntreeName == "Grilled Chicken");
        Assert.Contains(entrees, e => e.EntreeName == "Burger");
    }

    [Fact]
    public async Task GetEntreesByStationID_ReturnsEntreesForStation()
    {
        // Station 1 has entrees 1 and 2
        var response = await _client.GetAsync("/api/entree/station/1");
        response.EnsureSuccessStatusCode();
        var entrees = await response.Content.ReadFromJsonAsync<List<EntreeDto>>();

        Assert.NotNull(entrees);
        Assert.True(entrees.Count >= 2);
        Assert.All(entrees, entree => Assert.Equal(1, entree.StationId));
    }

    [Fact]
    public async Task UpdateEntreeByID_UpdatesExistingEntree()
    {
        // Create a new entree for this test
        var entreeId = _connection.ExecuteScalar<int>(
            InsertEntreeSql + " RETURNING id",
            new
            {
                StationId = 1,
                EntreeName = "Entree To Update",
                EntreeDescription = "Original",
                EntreePrice = 10.99m,
            }
        );

        var updatedEntree = new EntreeDto
        {
            Id = entreeId,
            StationId = 1,
            EntreeName = "Updated Entree",
            EntreeDescription = "Updated description",
            EntreePrice = 12.99m,
        };

        var response = await _client.PutAsJsonAsync($"/api/entree/{entreeId}", updatedEntree);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EntreeDto>();

        Assert.NotNull(result);
        Assert.Equal(updatedEntree.EntreeName, result.EntreeName);
        Assert.Equal(updatedEntree.EntreeDescription, result.EntreeDescription);
        Assert.Equal(updatedEntree.EntreePrice, result.EntreePrice);
    }

    [Fact]
    public async Task UpdateEntreeByID_ReturnsNotFound_WhenEntreeDoesNotExist()
    {
        var updatedEntree = new EntreeDto
        {
            Id = 99999,
            StationId = 1,
            EntreeName = "Nonexistent",
            EntreeDescription = "Description",
            EntreePrice = 9.99m,
        };

        var response = await _client.PutAsJsonAsync("/api/entree/99999", updatedEntree);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteEntreeByID_RemovesEntree()
    {
        // Create a new entree for deletion
        var entreeId = _connection.ExecuteScalar<int>(
            InsertEntreeSql + " RETURNING id",
            new
            {
                StationId = 1,
                EntreeName = "Entree To Delete",
                EntreeDescription = "Will be deleted",
                EntreePrice = 8.99m,
            }
        );

        var response = await _client.DeleteAsync($"/api/entree/{entreeId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/entree/{entreeId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteEntreeByID_ReturnsNotFound_WhenEntreeDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/entree/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SetStockStatusById_UpdatesInStockStatus()
    {
        // Create a new entree for this test
        var entreeId = _connection.ExecuteScalar<int>(
            InsertEntreeSql + " RETURNING id",
            new
            {
                StationId = 1,
                EntreeName = "Entree To Stock Toggle",
                EntreeDescription = "Testing stock status",
                EntreePrice = 13.99m,
            }
        );

        // Set stock status to false
        var response = await _client.PutAsJsonAsync($"/api/entree/{entreeId}/stock", false);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the stock status was updated by retrieving the entree
        var getResponse = await _client.GetAsync($"/api/entree/{entreeId}");
        getResponse.EnsureSuccessStatusCode();
        var entree = await getResponse.Content.ReadFromJsonAsync<EntreeDto>();

        Assert.NotNull(entree);
        Assert.False(entree.InStock);
    }

    [Fact]
    public async Task SetStockStatusById_ToggesStockStatusToTrue()
    {
        // Create a new entree for this test
        var entreeId = _connection.ExecuteScalar<int>(
            InsertEntreeSql + " RETURNING id",
            new
            {
                StationId = 1,
                EntreeName = "Entree To Stock Toggle True",
                EntreeDescription = "Testing stock status toggle",
                EntreePrice = 14.99m,
            }
        );

        // Set stock status to true
        var response = await _client.PutAsJsonAsync($"/api/entree/{entreeId}/stock", true);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the stock status was updated
        var getResponse = await _client.GetAsync($"/api/entree/{entreeId}");
        getResponse.EnsureSuccessStatusCode();
        var entree = await getResponse.Content.ReadFromJsonAsync<EntreeDto>();

        Assert.NotNull(entree);
        Assert.True(entree.InStock);
    }

    [Fact]
    public async Task SetStockStatusById_ReturnsNotFound_WhenEntreeDoesNotExist()
    {
        var response = await _client.PutAsJsonAsync("/api/entree/99999/stock", false);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
