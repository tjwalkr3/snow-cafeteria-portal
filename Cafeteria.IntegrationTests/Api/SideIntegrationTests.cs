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

public class SideIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private NpgsqlConnection _connection = null!;

    public SideIntegrationTests()
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
    public async Task CreateSide_AddsNewSide()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);

        var newSide = new SideDto
        {
            StationId = 1,
            SideName = "Onion Rings",
            SideDescription = "Crispy fried onion rings",
            SidePrice = 3.49m,
            ImageUrl = "https://picsum.photos/id/300/300/200"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/side", newSide);
        response.EnsureSuccessStatusCode();
        var createdSide = await response.Content.ReadFromJsonAsync<SideDto>();

        // Assert
        Assert.NotNull(createdSide);
        Assert.Equal(newSide.SideName, createdSide.SideName);
        Assert.Equal(newSide.SideDescription, createdSide.SideDescription);
        Assert.Equal(newSide.SidePrice, createdSide.SidePrice);
        Assert.True(createdSide.Id > 0);
    }

    [Fact]
    public async Task GetSideByID_ReturnsCorrectSide()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var sideId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            Sides[0]);

        // Act
        var response = await _client.GetAsync($"/api/side/{sideId}");
        response.EnsureSuccessStatusCode();
        var side = await response.Content.ReadFromJsonAsync<SideDto>();

        // Assert
        Assert.NotNull(side);
        Assert.Equal(Sides[0].SideName, side.SideName);
        Assert.Equal(Sides[0].SideDescription, side.SideDescription);
        Assert.Equal(Sides[0].SidePrice, side.SidePrice);
    }

    [Fact]
    public async Task GetSideByID_ReturnsNotFound_WhenSideDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/side/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllSides_ReturnsAllSides()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertSideSql, Sides[0]);
        _connection.Execute(InsertSideSql, Sides[1]);

        // Act
        var response = await _client.GetAsync("/api/side");
        response.EnsureSuccessStatusCode();
        var sides = await response.Content.ReadFromJsonAsync<List<SideDto>>();

        // Assert
        Assert.NotNull(sides);
        Assert.Equal(2, sides.Count);
        Assert.Equal(Sides[0].SideName, sides[0].SideName);
        Assert.Equal(Sides[1].SideName, sides[1].SideName);
    }

    [Fact]
    public async Task GetSidesByStationID_ReturnsSidesForStation()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertStationSql, Stations[1]);
        _connection.Execute(InsertSideSql, Sides[0]);
        _connection.Execute(InsertSideSql, Sides[1]);

        // Act
        var response = await _client.GetAsync("/api/side/station/1");
        response.EnsureSuccessStatusCode();
        var sides = await response.Content.ReadFromJsonAsync<List<SideDto>>();

        // Assert
        Assert.NotNull(sides);
        Assert.Equal(2, sides.Count);
        Assert.All(sides, side => Assert.Equal(1, side.StationId));
    }

    [Fact]
    public async Task UpdateSideByID_UpdatesExistingSide()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var sideId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            Sides[0]);

        var updatedSide = new SideDto
        {
            Id = sideId,
            StationId = 1,
            SideName = "Updated Side",
            SideDescription = "Updated description",
            SidePrice = 4.49m,
            ImageUrl = "https://picsum.photos/id/301/300/200"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/side/{sideId}", updatedSide);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SideDto>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedSide.SideName, result.SideName);
        Assert.Equal(updatedSide.SideDescription, result.SideDescription);
        Assert.Equal(updatedSide.SidePrice, result.SidePrice);
    }

    [Fact]
    public async Task UpdateSideByID_ReturnsNotFound_WhenSideDoesNotExist()
    {
        // Arrange
        var updatedSide = new SideDto
        {
            Id = 999,
            StationId = 1,
            SideName = "Nonexistent",
            SideDescription = "Description",
            SidePrice = 2.99m,
            ImageUrl = "https://picsum.photos/id/302/300/200"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/side/999", updatedSide);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSideByID_RemovesSide()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var sideId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            Sides[0]);

        // Act
        var response = await _client.DeleteAsync($"/api/side/{sideId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/side/{sideId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteSideByID_ReturnsNotFound_WhenSideDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/side/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
