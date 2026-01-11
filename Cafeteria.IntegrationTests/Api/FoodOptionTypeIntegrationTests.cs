using System.Net.Http.Json;
using Cafeteria.Shared.DTOs;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SampleMenuData;

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
    public async Task GetAllFoodOptions_ReturnsFoodOptionData()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/manager/food-options");
        response.EnsureSuccessStatusCode();
        var optionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>();

        Assert.NotNull(optionsAfter);
        Assert.True(optionsAfter.Count >= 3);
        Assert.Contains(optionsAfter, o => o.FoodOptionName == "Lettuce");
        Assert.Contains(optionsAfter, o => o.FoodOptionName == "Tomato");
    }

    [Fact]
    public async Task GetAllFoodTypes_ReturnsFoodTypeData()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/manager/food-types");
        response.EnsureSuccessStatusCode();
        var typesAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionTypeDto>>();

        Assert.NotNull(typesAfter);
        Assert.True(typesAfter.Count >= 2);
        Assert.Contains(typesAfter, t => t.FoodOptionTypeName == "Toppings");
        Assert.Contains(typesAfter, t => t.FoodOptionTypeName == "Condiments");
    }

    [Fact]
    public async Task GetAllOptionOptionTypes_ReturnsRelationshipData()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/manager/option-option-types");
        response.EnsureSuccessStatusCode();
        var relationsAfter = await response.Content.ReadFromJsonAsync<List<OptionOptionTypeDto>>();

        Assert.NotNull(relationsAfter);
        Assert.True(relationsAfter.Count >= 3);
    }

    [Fact]
    public async Task CreateFoodOption_AddsNewFoodOption()
    {
        var newOption = new FoodOptionDto
        {
            FoodOptionName = "Test Pickles",
            InStock = true,
            ImageUrl = "https://picsum.photos/id/10/300/200",
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
        // Create a new relationship for deletion
        var result = _connection.ExecuteScalar<int>(
            InsertOptionOptionTypeSql + " RETURNING id",
            new { FoodOptionId = 1, FoodOptionTypeId = 1 }
        );

        var response = await _client.DeleteAsync($"/api/manager/option-option-types/{result}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var remaining = await _client.GetAsync("/api/manager/option-option-types");
        remaining.EnsureSuccessStatusCode();
        var relations = await remaining.Content.ReadFromJsonAsync<List<OptionOptionTypeDto>>();
        Assert.NotNull(relations);
        // Should still have pre-loaded data, just not the one we deleted
        Assert.DoesNotContain(relations, r => r.Id == result);
    }
}
