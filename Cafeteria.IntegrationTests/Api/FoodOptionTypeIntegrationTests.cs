using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class FoodOptionTypeIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public FoodOptionTypeIntegrationTests(DatabaseFixture fixture)
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
    public async Task GetAllFoodOptionTypes_ReturnsFoodOptionTypeData()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/FoodOptionType");
        response.EnsureSuccessStatusCode();
        var typesAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionTypeDto>>();

        Assert.NotNull(typesAfter);
        Assert.True(typesAfter.Count >= 2);
        Assert.Contains(typesAfter, t => t.FoodOptionTypeName == "Toppings");
        Assert.Contains(typesAfter, t => t.FoodOptionTypeName == "Condiments");
    }

    [Fact]
    public async Task CreateFoodOptionType_AddsNewFoodOptionType()
    {
        var newType = new FoodOptionTypeDto
        {
            FoodOptionTypeName = "Test Sauces",
            RequiredAmount = 0,
            IncludedAmount = 1,
            MaxAmount = 3,
            FoodOptionPrice = 0.50m,
            EntreeId = 1,
            SideId = null,
        };

        var response = await _client.PostAsJsonAsync("/api/FoodOptionType", newType);
        response.EnsureSuccessStatusCode();
        var createdType = await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();

        Assert.NotNull(createdType);
        Assert.Equal(newType.FoodOptionTypeName, createdType.FoodOptionTypeName);
        Assert.Equal(newType.IncludedAmount, createdType.IncludedAmount);
        Assert.True(createdType.Id > 0);
    }

    [Fact]
    public async Task GetFoodOptionTypeByID_ReturnsCorrectFoodOptionType()
    {
        // Use pre-loaded food type with ID 1
        var response = await _client.GetAsync("/api/FoodOptionType/1");
        response.EnsureSuccessStatusCode();
        var type = await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();

        Assert.NotNull(type);
        Assert.Equal("Toppings", type.FoodOptionTypeName);
    }

    [Fact]
    public async Task GetFoodOptionTypeByID_ReturnsNotFound_WhenFoodOptionTypeDoesNotExist()
    {
        var response = await _client.GetAsync("/api/FoodOptionType/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFoodOptionType_UpdatesExistingFoodOptionType()
    {
        // Create a new food type for this test
        var typeId = _connection.ExecuteScalar<int>(
            InsertFoodOptionTypeSql + " RETURNING id",
            new
            {
                FoodOptionTypeName = "Type To Update",
                RequiredAmount = 0,
                IncludedAmount = 1,
                MaxAmount = 5,
                FoodOptionPrice = 0.00m,
                EntreeId = 1,
                SideId = (int?)null,
            }
        );

        var updatedType = new FoodOptionTypeDto
        {
            Id = typeId,
            FoodOptionTypeName = "Updated Type Name",
            RequiredAmount = 1,
            IncludedAmount = 2,
            MaxAmount = 10,
            FoodOptionPrice = 1.00m,
            EntreeId = 1,
            SideId = null,
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/FoodOptionType/{typeId}",
            updatedType
        );
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();

        Assert.NotNull(result);
        Assert.Equal(updatedType.FoodOptionTypeName, result.FoodOptionTypeName);
        Assert.Equal(updatedType.IncludedAmount, result.IncludedAmount);
        Assert.Equal(updatedType.MaxAmount, result.MaxAmount);
    }

    [Fact]
    public async Task UpdateFoodOptionType_ReturnsNotFound_WhenFoodOptionTypeDoesNotExist()
    {
        var updatedType = new FoodOptionTypeDto
        {
            Id = 99999,
            FoodOptionTypeName = "Nonexistent",
            RequiredAmount = 0,
            IncludedAmount = 1,
            MaxAmount = 5,
            FoodOptionPrice = 0.00m,
            EntreeId = 1,
            SideId = null,
        };

        var response = await _client.PutAsJsonAsync("/api/FoodOptionType/99999", updatedType);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodOptionType_RemovesFoodOptionType()
    {
        // Create a new food type for deletion
        var typeId = _connection.ExecuteScalar<int>(
            InsertFoodOptionTypeSql + " RETURNING id",
            new
            {
                FoodOptionTypeName = "Type To Delete",
                RequiredAmount = 0,
                IncludedAmount = 1,
                MaxAmount = 5,
                FoodOptionPrice = 0.00m,
                EntreeId = 1,
                SideId = (int?)null,
            }
        );

        var response = await _client.DeleteAsync($"/api/FoodOptionType/{typeId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/FoodOptionType/{typeId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodOptionType_ReturnsNotFound_WhenFoodOptionTypeDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/FoodOptionType/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFoodOptionTypesByEntree_ReturnsFoodOptionTypeData()
    {
        // Use pre-loaded sample data for entree 1
        var response = await _client.GetAsync("/api/foodoptiontype/entree/1");
        response.EnsureSuccessStatusCode();
        var optionTypesAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionTypeDto>>();

        Assert.NotNull(optionTypesAfter);
        Assert.True(optionTypesAfter.Count >= 1);
    }

    [Fact]
    public async Task GetFoodOptionTypesWithOptionsByEntree_ReturnsFoodOptionTypeWithOptionsData()
    {
        // Use pre-loaded sample data for entree 1
        var response = await _client.GetAsync("/api/foodoptiontype/with-options/entree/1");
        response.EnsureSuccessStatusCode();
        var optionTypesWithOptionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionTypeWithOptionsDto>>();

        Assert.NotNull(optionTypesWithOptionsAfter);
        Assert.True(optionTypesWithOptionsAfter.Count >= 1);
        // Verify each option type has options list
        Assert.All(optionTypesWithOptionsAfter, optionType => Assert.NotNull(optionType.Options));
    }
}
