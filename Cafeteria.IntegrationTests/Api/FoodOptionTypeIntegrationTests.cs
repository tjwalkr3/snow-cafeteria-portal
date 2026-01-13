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
    public async Task GetFoodOptionByID_ReturnsCorrectFoodOption()
    {
        // Use pre-loaded food option with ID 1
        var response = await _client.GetAsync("/api/manager/food-options/1");
        response.EnsureSuccessStatusCode();
        var option = await response.Content.ReadFromJsonAsync<FoodOptionDto>();

        Assert.NotNull(option);
        Assert.Equal("Lettuce", option.FoodOptionName);
    }

    [Fact]
    public async Task GetFoodOptionByID_ReturnsNotFound_WhenFoodOptionDoesNotExist()
    {
        var response = await _client.GetAsync("/api/manager/food-options/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFoodOption_UpdatesExistingFoodOption()
    {
        // Create a new food option for this test
        var optionId = _connection.ExecuteScalar<int>(
            InsertFoodOptionSql + " RETURNING id",
            new
            {
                FoodOptionName = "Option To Update",
                InStock = true,
                ImageUrl = "https://example.com/img.jpg",
            }
        );

        var updatedOption = new FoodOptionDto
        {
            Id = optionId,
            FoodOptionName = "Updated Option Name",
            InStock = false,
            ImageUrl = "https://picsum.photos/id/20/300/200",
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/manager/food-options/{optionId}",
            updatedOption
        );
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FoodOptionDto>();

        Assert.NotNull(result);
        Assert.Equal(updatedOption.FoodOptionName, result.FoodOptionName);
        Assert.Equal(updatedOption.InStock, result.InStock);
    }

    [Fact]
    public async Task UpdateFoodOption_ReturnsNotFound_WhenFoodOptionDoesNotExist()
    {
        var updatedOption = new FoodOptionDto
        {
            Id = 99999,
            FoodOptionName = "Nonexistent",
            InStock = true,
            ImageUrl = "https://example.com/img.jpg",
        };

        var response = await _client.PutAsJsonAsync(
            "/api/manager/food-options/99999",
            updatedOption
        );
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodOption_RemovesFoodOption()
    {
        // Create a new food option for deletion
        var optionId = _connection.ExecuteScalar<int>(
            InsertFoodOptionSql + " RETURNING id",
            new
            {
                FoodOptionName = "Option To Delete",
                InStock = true,
                ImageUrl = "https://example.com/img.jpg",
            }
        );

        var response = await _client.DeleteAsync($"/api/manager/food-options/{optionId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/manager/food-options/{optionId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodOption_ReturnsNotFound_WhenFoodOptionDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/manager/food-options/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateFoodType_AddsNewFoodType()
    {
        var newType = new FoodOptionTypeDto
        {
            FoodOptionTypeName = "Test Sauces",
            NumIncluded = 1,
            MaxAmount = 3,
            FoodOptionPrice = 0.50m,
            EntreeId = 1,
            SideId = null,
        };

        var response = await _client.PostAsJsonAsync("/api/manager/food-types", newType);
        response.EnsureSuccessStatusCode();
        var createdType = await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();

        Assert.NotNull(createdType);
        Assert.Equal(newType.FoodOptionTypeName, createdType.FoodOptionTypeName);
        Assert.Equal(newType.NumIncluded, createdType.NumIncluded);
        Assert.True(createdType.Id > 0);
    }

    [Fact]
    public async Task GetFoodTypeByID_ReturnsCorrectFoodType()
    {
        // Use pre-loaded food type with ID 1
        var response = await _client.GetAsync("/api/manager/food-types/1");
        response.EnsureSuccessStatusCode();
        var type = await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();

        Assert.NotNull(type);
        Assert.Equal("Toppings", type.FoodOptionTypeName);
    }

    [Fact]
    public async Task GetFoodTypeByID_ReturnsNotFound_WhenFoodTypeDoesNotExist()
    {
        var response = await _client.GetAsync("/api/manager/food-types/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFoodType_UpdatesExistingFoodType()
    {
        // Create a new food type for this test
        var typeId = _connection.ExecuteScalar<int>(
            InsertFoodOptionTypeSql + " RETURNING id",
            new
            {
                FoodOptionTypeName = "Type To Update",
                NumIncluded = 1,
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
            NumIncluded = 2,
            MaxAmount = 10,
            FoodOptionPrice = 1.00m,
            EntreeId = 1,
            SideId = null,
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/manager/food-types/{typeId}",
            updatedType
        );
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FoodOptionTypeDto>();

        Assert.NotNull(result);
        Assert.Equal(updatedType.FoodOptionTypeName, result.FoodOptionTypeName);
        Assert.Equal(updatedType.NumIncluded, result.NumIncluded);
        Assert.Equal(updatedType.MaxAmount, result.MaxAmount);
    }

    [Fact]
    public async Task UpdateFoodType_ReturnsNotFound_WhenFoodTypeDoesNotExist()
    {
        var updatedType = new FoodOptionTypeDto
        {
            Id = 99999,
            FoodOptionTypeName = "Nonexistent",
            NumIncluded = 1,
            MaxAmount = 5,
            FoodOptionPrice = 0.00m,
            EntreeId = 1,
            SideId = null,
        };

        var response = await _client.PutAsJsonAsync("/api/manager/food-types/99999", updatedType);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodType_RemovesFoodType()
    {
        // Create a new food type for deletion
        var typeId = _connection.ExecuteScalar<int>(
            InsertFoodOptionTypeSql + " RETURNING id",
            new
            {
                FoodOptionTypeName = "Type To Delete",
                NumIncluded = 1,
                MaxAmount = 5,
                FoodOptionPrice = 0.00m,
                EntreeId = 1,
                SideId = (int?)null,
            }
        );

        var response = await _client.DeleteAsync($"/api/manager/food-types/{typeId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/manager/food-types/{typeId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodType_ReturnsNotFound_WhenFoodTypeDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/manager/food-types/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateOptionOptionType_AddsNewRelationship()
    {
        var newRelation = new OptionOptionTypeDto { FoodOptionId = 1, FoodOptionTypeId = 2 };

        var response = await _client.PostAsJsonAsync(
            "/api/manager/option-option-types",
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
        var response = await _client.GetAsync("/api/manager/option-option-types/1");
        response.EnsureSuccessStatusCode();
        var relation = await response.Content.ReadFromJsonAsync<OptionOptionTypeDto>();

        Assert.NotNull(relation);
        Assert.Equal(1, relation.FoodOptionId);
        Assert.Equal(1, relation.FoodOptionTypeId);
    }

    [Fact]
    public async Task GetOptionOptionTypeByID_ReturnsNotFound_WhenRelationshipDoesNotExist()
    {
        var response = await _client.GetAsync("/api/manager/option-option-types/99999");
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
            $"/api/manager/option-option-types/{relationId}",
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
            "/api/manager/option-option-types/99999",
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

        var response = await _client.DeleteAsync($"/api/manager/option-option-types/{relationId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/manager/option-option-types/{relationId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteOptionOptionType_ReturnsNotFound_WhenRelationshipDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/manager/option-option-types/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
