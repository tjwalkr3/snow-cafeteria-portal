using System.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Cafeteria.Shared.DTOs;
using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;
using static Cafeteria.IntegrationTests.Api.DBSql;
using static Cafeteria.IntegrationTests.Api.SampleMenuData;

namespace Cafeteria.IntegrationTests.Api;

public class EntreeIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private NpgsqlConnection _connection = null!;

    public EntreeIntegrationTests()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("cafeteria")
            .WithUsername("cafeteria_admin")
            .WithPassword("SnowCafe")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        _connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        await _connection.OpenAsync();
        await _connection.ExecuteAsync(SqlData);

        var connectionString = _postgresContainer.GetConnectionString();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbConnection));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddScoped<IDbConnection>(_ =>
                    {
                        var conn = new NpgsqlConnection(connectionString);
                        conn.Open();
                        return conn;
                    });
                });
            });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task CreateEntree_AddsNewEntree()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);

        var newEntree = new EntreeDto
        {
            StationId = 1,
            EntreeName = "Steak",
            EntreeDescription = "Grilled ribeye steak",
            EntreePrice = 15.99m,
            ImageUrl = "https://picsum.photos/id/200/300/200"
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
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var entreeId = _connection.ExecuteScalar<int>(
            InsertEntreeSql + " RETURNING id",
            Entrees[0]);

        var response = await _client.GetAsync($"/api/entree/{entreeId}");
        response.EnsureSuccessStatusCode();
        var entree = await response.Content.ReadFromJsonAsync<EntreeDto>();

        Assert.NotNull(entree);
        Assert.Equal(Entrees[0].EntreeName, entree.EntreeName);
        Assert.Equal(Entrees[0].EntreeDescription, entree.EntreeDescription);
        Assert.Equal(Entrees[0].EntreePrice, entree.EntreePrice);
    }

    [Fact]
    public async Task GetEntreeByID_ReturnsNotFound_WhenEntreeDoesNotExist()
    {
        var response = await _client.GetAsync("/api/entree/999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllEntrees_ReturnsAllEntrees()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertEntreeSql, Entrees[0]);
        _connection.Execute(InsertEntreeSql, Entrees[1]);

        var response = await _client.GetAsync("/api/entree");
        response.EnsureSuccessStatusCode();
        var entrees = await response.Content.ReadFromJsonAsync<List<EntreeDto>>();

        Assert.NotNull(entrees);
        Assert.Equal(2, entrees.Count);
        Assert.Equal(Entrees[0].EntreeName, entrees[0].EntreeName);
        Assert.Equal(Entrees[1].EntreeName, entrees[1].EntreeName);
    }

    [Fact]
    public async Task GetEntreesByStationID_ReturnsEntreesForStation()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertStationSql, Stations[1]);
        _connection.Execute(InsertEntreeSql, Entrees[0]);
        _connection.Execute(InsertEntreeSql, Entrees[1]);

        var response = await _client.GetAsync("/api/entree/station/1");
        response.EnsureSuccessStatusCode();
        var entrees = await response.Content.ReadFromJsonAsync<List<EntreeDto>>();

        Assert.NotNull(entrees);
        Assert.Equal(2, entrees.Count);
        Assert.All(entrees, entree => Assert.Equal(1, entree.StationId));
    }

    [Fact]
    public async Task UpdateEntreeByID_UpdatesExistingEntree()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var entreeId = _connection.ExecuteScalar<int>(
            InsertEntreeSql + " RETURNING id",
            Entrees[0]);

        var updatedEntree = new EntreeDto
        {
            Id = entreeId,
            StationId = 1,
            EntreeName = "Updated Entree",
            EntreeDescription = "Updated description",
            EntreePrice = 12.99m,
            ImageUrl = "https://picsum.photos/id/201/300/200"
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
            Id = 999,
            StationId = 1,
            EntreeName = "Nonexistent",
            EntreeDescription = "Description",
            EntreePrice = 9.99m,
            ImageUrl = "https://picsum.photos/id/202/300/200"
        };

        var response = await _client.PutAsJsonAsync("/api/entree/999", updatedEntree);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteEntreeByID_RemovesEntree()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var entreeId = _connection.ExecuteScalar<int>(
            InsertEntreeSql + " RETURNING id",
            Entrees[0]);

        var response = await _client.DeleteAsync($"/api/entree/{entreeId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/entree/{entreeId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteEntreeByID_ReturnsNotFound_WhenEntreeDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/entree/999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
