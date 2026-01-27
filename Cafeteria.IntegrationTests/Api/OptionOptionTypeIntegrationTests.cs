using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SampleMenuData;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class OptionOptionTypeIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public OptionOptionTypeIntegrationTests(DatabaseFixture fixture)
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
    public async Task GetAllOptionOptionTypes_ReturnsRelationshipData()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/OptionOptionType");
        response.EnsureSuccessStatusCode();
        var relationsAfter = await response.Content.ReadFromJsonAsync<List<OptionOptionTypeDto>>();

        Assert.NotNull(relationsAfter);
        Assert.True(relationsAfter.Count >= 3);
    }

    [Fact]
    public async Task CreateOptionOptionType_AddsNewRelationship()
    {
        var newRelation = new OptionOptionTypeDto { FoodOptionId = 1, FoodOptionTypeId = 2 };

        var response = await _client.PostAsJsonAsync(
            "/api/OptionOptionType",
            newRelation
        );
        response.EnsureSuccessStatusCode();
        var createdRelation = await response.Content.ReadFromJsonAsync<OptionOptionTypeDto>();

        Assert.NotNull(createdRelation);
        Assert.Equal(newRelation.FoodOptionId, createdRelation.FoodOptionId);
        Assert.Equal(newRelation.FoodOptionTypeId, createdRelation.FoodOptionTypeId);
        Assert.True(createdRelation.Id > 0);
    }

    [Fact]
    public async Task GetOptionOptionTypeByID_ReturnsCorrectRelationship()
    {
        // Use pre-loaded relationship with ID 1
        var response = await _client.GetAsync("/api/OptionOptionType/1");
        response.EnsureSuccessStatusCode();
        var relation = await response.Content.ReadFromJsonAsync<OptionOptionTypeDto>();

        Assert.NotNull(relation);
        Assert.Equal(1, relation.FoodOptionId);
        Assert.Equal(1, relation.FoodOptionTypeId);
    }

    [Fact]
    public async Task GetOptionOptionTypeByID_ReturnsNotFound_WhenRelationshipDoesNotExist()
    {
        var response = await _client.GetAsync("/api/OptionOptionType/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOptionOptionType_UpdatesExistingRelationship()
    {
        // Create a new relationship for this test
        var relationId = _connection.ExecuteScalar<int>(
            InsertOptionOptionTypeSql + " RETURNING id",
            new { FoodOptionId = 1, FoodOptionTypeId = 1 }
        );

        var updatedRelation = new OptionOptionTypeDto
        {
            Id = relationId,
            FoodOptionId = 2,
            FoodOptionTypeId = 2,
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/OptionOptionType/{relationId}",
            updatedRelation
        );
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<OptionOptionTypeDto>();

        Assert.NotNull(result);
        Assert.Equal(updatedRelation.FoodOptionId, result.FoodOptionId);
        Assert.Equal(updatedRelation.FoodOptionTypeId, result.FoodOptionTypeId);
    }

    [Fact]
    public async Task UpdateOptionOptionType_ReturnsNotFound_WhenRelationshipDoesNotExist()
    {
        var updatedRelation = new OptionOptionTypeDto
        {
            Id = 99999,
            FoodOptionId = 1,
            FoodOptionTypeId = 1,
        };

        var response = await _client.PutAsJsonAsync(
            "/api/OptionOptionType/99999",
            updatedRelation
        );
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteOptionOptionType_RemovesRelationship()
    {
        // Create a new relationship for deletion
        var relationId = _connection.ExecuteScalar<int>(
            InsertOptionOptionTypeSql + " RETURNING id",
            new { FoodOptionId = 1, FoodOptionTypeId = 1 }
        );

        var response = await _client.DeleteAsync($"/api/OptionOptionType/{relationId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/OptionOptionType/{relationId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteOptionOptionType_ReturnsNotFound_WhenRelationshipDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/OptionOptionType/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
