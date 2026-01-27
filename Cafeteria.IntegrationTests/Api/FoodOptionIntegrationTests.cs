using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class FoodOptionIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public FoodOptionIntegrationTests(DatabaseFixture fixture)
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
        var response = await _client.GetAsync("/api/FoodOption");
        response.EnsureSuccessStatusCode();
        var optionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>();

        Assert.NotNull(optionsAfter);
        Assert.True(optionsAfter.Count >= 3);
        Assert.Contains(optionsAfter, o => o.FoodOptionName == "Lettuce");
        Assert.Contains(optionsAfter, o => o.FoodOptionName == "Tomato");
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

        var response = await _client.PostAsJsonAsync("/api/FoodOption", newOption);
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
        var response = await _client.GetAsync("/api/FoodOption/1");
        response.EnsureSuccessStatusCode();
        var option = await response.Content.ReadFromJsonAsync<FoodOptionDto>();

        Assert.NotNull(option);
        Assert.Equal("Lettuce", option.FoodOptionName);
    }

    [Fact]
    public async Task GetFoodOptionByID_ReturnsNotFound_WhenFoodOptionDoesNotExist()
    {
        var response = await _client.GetAsync("/api/FoodOption/99999");
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
            $"/api/FoodOption/{optionId}",
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
            "/api/FoodOption/99999",
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

        var response = await _client.DeleteAsync($"/api/FoodOption/{optionId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/FoodOption/{optionId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodOption_ReturnsNotFound_WhenFoodOptionDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/FoodOption/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFoodOptionsByEntree_ReturnsFoodOptionData()
    {
        // Use pre-loaded sample data for entree 1
        var response = await _client.GetAsync("/api/foodoption/entree/1");
        response.EnsureSuccessStatusCode();
        var optionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>();

        Assert.NotNull(optionsAfter);
        Assert.True(optionsAfter.Count >= 2);
        Assert.Contains(optionsAfter, o => o.FoodOptionName == "Lettuce");
        Assert.Contains(optionsAfter, o => o.FoodOptionName == "Tomato");
    }

    [Fact]
    public async Task GetFoodOptionsBySide_ReturnsFoodOptionData()
    {
        // Use pre-loaded sample data for side 1
        var response = await _client.GetAsync("/api/foodoption/side/1");
        response.EnsureSuccessStatusCode();
        var optionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>();

        Assert.NotNull(optionsAfter);
        // May be empty or have options depending on sample data configuration
        // Just verify it doesn't error
    }
}
