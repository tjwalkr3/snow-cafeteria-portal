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

public class DrinkIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private NpgsqlConnection _connection = null!;

    public DrinkIntegrationTests()
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
    public async Task CreateDrink_AddsNewDrink()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);

        var newDrink = new DrinkDto
        {
            StationId = 1,
            DrinkName = "Sprite",
            DrinkDescription = "Lemon-lime soda",
            DrinkPrice = 1.99m,
            ImageUrl = "https://picsum.photos/id/100/300/200"
        };

        var response = await _client.PostAsJsonAsync("/api/drink", newDrink);
        response.EnsureSuccessStatusCode();
        var createdDrink = await response.Content.ReadFromJsonAsync<DrinkDto>();

        Assert.NotNull(createdDrink);
        Assert.Equal(newDrink.DrinkName, createdDrink.DrinkName);
        Assert.Equal(newDrink.DrinkDescription, createdDrink.DrinkDescription);
        Assert.Equal(newDrink.DrinkPrice, createdDrink.DrinkPrice);
        Assert.True(createdDrink.Id > 0);
    }

    [Fact]
    public async Task GetDrinkByID_ReturnsCorrectDrink()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var drinkId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            Drinks[0]);

        var response = await _client.GetAsync($"/api/drink/{drinkId}");
        response.EnsureSuccessStatusCode();
        var drink = await response.Content.ReadFromJsonAsync<DrinkDto>();

        Assert.NotNull(drink);
        Assert.Equal(Drinks[0].DrinkName, drink.DrinkName);
        Assert.Equal(Drinks[0].DrinkDescription, drink.DrinkDescription);
        Assert.Equal(Drinks[0].DrinkPrice, drink.DrinkPrice);
    }

    [Fact]
    public async Task GetDrinkByID_ReturnsNotFound_WhenDrinkDoesNotExist()
    {
        var response = await _client.GetAsync("/api/drink/999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllDrinks_ReturnsAllDrinks()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertDrinkSql, Drinks[0]);
        _connection.Execute(InsertDrinkSql, Drinks[1]);

        var response = await _client.GetAsync("/api/drink");
        response.EnsureSuccessStatusCode();
        var drinks = await response.Content.ReadFromJsonAsync<List<DrinkDto>>();

        Assert.NotNull(drinks);
        Assert.Equal(2, drinks.Count);
        Assert.Equal(Drinks[0].DrinkName, drinks[0].DrinkName);
        Assert.Equal(Drinks[1].DrinkName, drinks[1].DrinkName);
    }

    [Fact]
    public async Task GetDrinksByStationID_ReturnsDrinksForStation()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertStationSql, Stations[1]);
        _connection.Execute(InsertDrinkSql, Drinks[0]);
        _connection.Execute(InsertDrinkSql, Drinks[1]);

        var response = await _client.GetAsync("/api/drink/station/1");
        response.EnsureSuccessStatusCode();
        var drinks = await response.Content.ReadFromJsonAsync<List<DrinkDto>>();

        Assert.NotNull(drinks);
        Assert.Equal(2, drinks.Count);
        Assert.All(drinks, drink => Assert.Equal(1, drink.StationId));
    }

    [Fact]
    public async Task UpdateDrinkByID_UpdatesExistingDrink()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var drinkId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            Drinks[0]);

        var updatedDrink = new DrinkDto
        {
            Id = drinkId,
            StationId = 1,
            DrinkName = "Updated Drink",
            DrinkDescription = "Updated description",
            DrinkPrice = 3.99m,
            ImageUrl = "https://picsum.photos/id/101/300/200"
        };

        var response = await _client.PutAsJsonAsync($"/api/drink/{drinkId}", updatedDrink);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<DrinkDto>();

        Assert.NotNull(result);
        Assert.Equal(updatedDrink.DrinkName, result.DrinkName);
        Assert.Equal(updatedDrink.DrinkDescription, result.DrinkDescription);
        Assert.Equal(updatedDrink.DrinkPrice, result.DrinkPrice);
    }

    [Fact]
    public async Task UpdateDrinkByID_ReturnsNotFound_WhenDrinkDoesNotExist()
    {
        var updatedDrink = new DrinkDto
        {
            Id = 999,
            StationId = 1,
            DrinkName = "Nonexistent",
            DrinkDescription = "Description",
            DrinkPrice = 1.99m,
            ImageUrl = "https://picsum.photos/id/102/300/200"
        };

        var response = await _client.PutAsJsonAsync("/api/drink/999", updatedDrink);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDrinkByID_RemovesDrink()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        var drinkId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            Drinks[0]);

        var response = await _client.DeleteAsync($"/api/drink/{drinkId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/drink/{drinkId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteDrinkByID_ReturnsNotFound_WhenDrinkDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/drink/999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
