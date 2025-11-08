using System.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Cafeteria.Shared.DTOs;
using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;
using static Cafeteria.Api.Tests.IntegrationTests.DBSql;
using static Cafeteria.Api.Tests.IntegrationTests.SampleMenuData;

namespace Cafeteria.Api.Tests.IntegrationTests;

public class MenuIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private NpgsqlConnection _connection = null!;

    public MenuIntegrationTests()
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
                    services.AddScoped<IDbConnection>(_ => _connection);
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

    private static void AssertPropertiesEqual<T>(T expected, T actual, params string[] propertiesToSkip)
    {
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            if (propertiesToSkip.Contains(property.Name))
                continue;

            var expectedValue = property.GetValue(expected);
            var actualValue = property.GetValue(actual);
            Assert.Equal(expectedValue, actualValue);
        }
    }

    [Fact]
    public async Task GetAllLocations_ReturnsLocationData()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertLocationSql, Locations[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/locations");
        response.EnsureSuccessStatusCode();
        var locationsAfter = await response.Content.ReadFromJsonAsync<List<LocationDto>>();

        // Assert
        Assert.NotNull(locationsAfter);
        Assert.Equal(2, locationsAfter.Count);
        AssertPropertiesEqual(Locations[0], locationsAfter[0]);
        AssertPropertiesEqual(Locations[1], locationsAfter[1]);
    }

    [Fact]
    public async Task GetStationsByLocation_ReturnsStationData()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertStationSql, Stations[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/stations/location/1");
        response.EnsureSuccessStatusCode();
        var stationsAfter = await response.Content.ReadFromJsonAsync<List<StationDto>>();

        // Assert
        Assert.NotNull(stationsAfter);
        Assert.Equal(2, stationsAfter.Count);
        AssertPropertiesEqual(Stations[0], stationsAfter[0]);
        AssertPropertiesEqual(Stations[1], stationsAfter[1]);
    }

    [Fact]
    public async Task GetEntreesByStation_ReturnsEntreeData()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertEntreeSql, Entrees[0]);
        _connection.Execute(InsertEntreeSql, Entrees[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/entrees/station/1");
        response.EnsureSuccessStatusCode();
        var entreesAfter = await response.Content.ReadFromJsonAsync<List<EntreeDto>>();

        // Assert
        Assert.NotNull(entreesAfter);
        Assert.Equal(2, entreesAfter.Count);
        AssertPropertiesEqual(Entrees[0], entreesAfter[0], "Id");
        AssertPropertiesEqual(Entrees[1], entreesAfter[1], "Id");
    }

    [Fact]
    public async Task GetSidesByStation_ReturnsSideData()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertSideSql, Sides[0]);
        _connection.Execute(InsertSideSql, Sides[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/sides/station/1");
        response.EnsureSuccessStatusCode();
        var sidesAfter = await response.Content.ReadFromJsonAsync<List<SideDto>>();

        // Assert
        Assert.NotNull(sidesAfter);
        Assert.Equal(2, sidesAfter.Count);
        AssertPropertiesEqual(Sides[0], sidesAfter[0], "Id");
        AssertPropertiesEqual(Sides[1], sidesAfter[1], "Id");
    }

    [Fact]
    public async Task GetDrinksByLocation_ReturnsDrinkData()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertDrinkSql, Drinks[0]);
        _connection.Execute(InsertDrinkSql, Drinks[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/drinks/location/1");
        response.EnsureSuccessStatusCode();
        var drinksAfter = await response.Content.ReadFromJsonAsync<List<DrinkDto>>();

        // Assert
        Assert.NotNull(drinksAfter);
        Assert.Equal(2, drinksAfter.Count);
        AssertPropertiesEqual(Drinks[0], drinksAfter[0], "Id");
        AssertPropertiesEqual(Drinks[1], drinksAfter[1], "Id");
    }

    [Fact]
    public async Task GetFoodOptionsByEntree_ReturnsFoodOptionData()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertEntreeSql, Entrees[0]);
        _connection.Execute(InsertFoodOptionSql, FoodOptions[0]);
        _connection.Execute(InsertFoodOptionSql, FoodOptions[1]);
        _connection.Execute(InsertFoodOptionTypeSql, FoodOptionTypeForEntree);
        _connection.Execute(InsertOptionOptionTypeSql, OptionOptionTypes[0]);
        _connection.Execute(InsertOptionOptionTypeSql, OptionOptionTypes[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/menu/options/entree/1");
        response.EnsureSuccessStatusCode();
        var optionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>();

        // Assert
        Assert.NotNull(optionsAfter);
        Assert.Equal(2, optionsAfter.Count);
        AssertPropertiesEqual(FoodOptions[0], optionsAfter[0], "Id");
        AssertPropertiesEqual(FoodOptions[1], optionsAfter[1], "Id");
    }

    [Fact]
    public async Task GetFoodOptionsBySide_ReturnsFoodOptionData()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertSideSql, Sides[0]);
        _connection.Execute(InsertFoodOptionSql, FoodOptions[0]);
        _connection.Execute(InsertFoodOptionSql, FoodOptions[1]);
        _connection.Execute(InsertFoodOptionTypeSql, FoodOptionTypeForSide);
        _connection.Execute(InsertOptionOptionTypeSql, OptionOptionTypes[0]);
        _connection.Execute(InsertOptionOptionTypeSql, OptionOptionTypes[1]);

        // Act
        var response = await _client.GetAsync("/api/menu/menu/options/side/1");
        response.EnsureSuccessStatusCode();
        var optionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>();

        // Assert
        Assert.NotNull(optionsAfter);
        Assert.Equal(2, optionsAfter.Count);
        AssertPropertiesEqual(FoodOptions[0], optionsAfter[0], "Id");
        AssertPropertiesEqual(FoodOptions[1], optionsAfter[1], "Id");
    }
}
