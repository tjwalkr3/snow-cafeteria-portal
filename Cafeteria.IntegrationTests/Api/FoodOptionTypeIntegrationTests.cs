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

public class FoodOptionTypeIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private NpgsqlConnection _connection = null!;

    public FoodOptionTypeIntegrationTests()
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

    [Fact]
    public async Task GetAllFoodOptions_ReturnsFoodOptionData()
    {
        _connection.Execute(InsertFoodOptionSql, FoodOptions[0]);
        _connection.Execute(InsertFoodOptionSql, FoodOptions[1]);

        var response = await _client.GetAsync("/api/manager/food-options");
        response.EnsureSuccessStatusCode();
        var optionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>();

        Assert.NotNull(optionsAfter);
        Assert.Equal(2, optionsAfter.Count);
        Assert.Equal(FoodOptions[0].FoodOptionName, optionsAfter[0].FoodOptionName);
        Assert.Equal(FoodOptions[0].InStock, optionsAfter[0].InStock);
        Assert.Equal(FoodOptions[1].FoodOptionName, optionsAfter[1].FoodOptionName);
        Assert.Equal(FoodOptions[1].InStock, optionsAfter[1].InStock);
    }

    [Fact]
    public async Task GetAllFoodTypes_ReturnsFoodTypeData()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertEntreeSql, Entrees[0]);
        _connection.Execute(InsertFoodOptionTypeSql, FoodOptionTypes[0]);

        var response = await _client.GetAsync("/api/manager/food-types");
        response.EnsureSuccessStatusCode();
        var typesAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionTypeDto>>();

        Assert.NotNull(typesAfter);
        Assert.Single(typesAfter);
        Assert.Equal(FoodOptionTypes[0].FoodOptionTypeName, typesAfter[0].FoodOptionTypeName);
        Assert.Equal(FoodOptionTypes[0].NumIncluded, typesAfter[0].NumIncluded);
        Assert.Equal(FoodOptionTypes[0].MaxAmount, typesAfter[0].MaxAmount);
    }

    [Fact]
    public async Task GetAllOptionOptionTypes_ReturnsRelationshipData()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertEntreeSql, Entrees[0]);
        _connection.Execute(InsertFoodOptionSql, FoodOptions[0]);
        _connection.Execute(InsertFoodOptionSql, FoodOptions[1]);
        _connection.Execute(InsertFoodOptionTypeSql, FoodOptionTypes[0]);
        _connection.Execute(InsertOptionOptionTypeSql, OptionOptionTypes[0]);
        _connection.Execute(InsertOptionOptionTypeSql, OptionOptionTypes[1]);

        var response = await _client.GetAsync("/api/manager/option-option-types");
        response.EnsureSuccessStatusCode();
        var relationsAfter = await response.Content.ReadFromJsonAsync<List<OptionOptionTypeDto>>();

        Assert.NotNull(relationsAfter);
        Assert.Equal(2, relationsAfter.Count);
        Assert.Equal(1, relationsAfter[0].FoodOptionId);
        Assert.Equal(1, relationsAfter[0].FoodOptionTypeId);
        Assert.Equal(2, relationsAfter[1].FoodOptionId);
        Assert.Equal(1, relationsAfter[1].FoodOptionTypeId);
    }

    [Fact]
    public async Task CreateFoodOption_AddsNewFoodOption()
    {
        var newOption = new FoodOptionDto
        {
            FoodOptionName = "Pickles",
            InStock = true,
            ImageUrl = "https://picsum.photos/id/10/300/200"
        };

        var response = await _client.PostAsJsonAsync("/api/manager/food-options", newOption);
        response.EnsureSuccessStatusCode();
        var createdOption = await response.Content.ReadFromJsonAsync<FoodOptionDto>();

        Assert.NotNull(createdOption);
        Assert.Equal(newOption.FoodOptionName, createdOption.FoodOptionName);
        Assert.Equal(newOption.InStock, createdOption.InStock);
        Assert.True(createdOption.Id > 0);
    }

    [Fact]
    public async Task DeleteOptionOptionType_RemovesRelationship()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertEntreeSql, Entrees[0]);
        _connection.Execute(InsertFoodOptionSql, FoodOptions[0]);
        _connection.Execute(InsertFoodOptionTypeSql, FoodOptionTypes[0]);
        var result = _connection.ExecuteScalar<int>(
            InsertOptionOptionTypeSql + " RETURNING id",
            OptionOptionTypes[0]);

        var response = await _client.DeleteAsync($"/api/manager/option-option-types/{result}");
        response.EnsureSuccessStatusCode();
        var deleteResult = await response.Content.ReadFromJsonAsync<bool>();

        Assert.True(deleteResult);
        var remaining = await _client.GetAsync("/api/manager/option-option-types");
        var relations = await remaining.Content.ReadFromJsonAsync<List<OptionOptionTypeDto>>();
        Assert.NotNull(relations);
        Assert.Empty(relations);
    }
}
