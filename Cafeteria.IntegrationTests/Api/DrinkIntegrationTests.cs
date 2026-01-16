using System.Net.Http.Json;
using Cafeteria.Shared.DTOs;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SampleMenuData;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class DrinkIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public DrinkIntegrationTests(DatabaseFixture fixture)
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
    public async Task CreateDrink_AddsNewDrink()
    {
        // Use pre-loaded location with ID 1
        var newDrink = new DrinkDto
        {
            LocationId = 1,
            DrinkName = "Test Sprite",
            DrinkDescription = "Lemon-lime soda for testing",
            DrinkPrice = 1.99m,
            ImageUrl = "https://picsum.photos/id/100/300/200",
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
        // Use pre-loaded drink with ID 1
        var response = await _client.GetAsync("/api/drink/1");
        response.EnsureSuccessStatusCode();
        var drink = await response.Content.ReadFromJsonAsync<DrinkDto>();

        Assert.NotNull(drink);
        Assert.Equal("Coca-Cola", drink.DrinkName);
    }

    [Fact]
    public async Task GetDrinkByID_ReturnsNotFound_WhenDrinkDoesNotExist()
    {
        var response = await _client.GetAsync("/api/drink/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllDrinks_ReturnsAllDrinks()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/drink");
        response.EnsureSuccessStatusCode();
        var drinks = await response.Content.ReadFromJsonAsync<List<DrinkDto>>();

        Assert.NotNull(drinks);
        Assert.True(drinks.Count >= 3);
        Assert.Contains(drinks, d => d.DrinkName == "Coca-Cola");
        Assert.Contains(drinks, d => d.DrinkName == "Lemonade");
    }

    [Fact]
    public async Task GetDrinksByLocationID_ReturnsDrinksForLocation()
    {
        // Location 1 has drinks 1 and 2
        var response = await _client.GetAsync("/api/drink/location/1");
        response.EnsureSuccessStatusCode();
        var drinks = await response.Content.ReadFromJsonAsync<List<DrinkDto>>();

        Assert.NotNull(drinks);
        Assert.True(drinks.Count >= 2);
        Assert.All(drinks, drink => Assert.Equal(1, drink.LocationId));
    }

    [Fact]
    public async Task UpdateDrinkByID_UpdatesExistingDrink()
    {
        // Create a new drink for this test
        var drinkId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Drink To Update",
                DrinkDescription = "Original",
                DrinkPrice = 1.99m,
                ImageUrl = "https://example.com/img.jpg",
            }
        );

        var updatedDrink = new DrinkDto
        {
            Id = drinkId,
            LocationId = 1,
            DrinkName = "Updated Drink",
            DrinkDescription = "Updated description",
            DrinkPrice = 3.99m,
            ImageUrl = "https://picsum.photos/id/101/300/200",
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
            Id = 99999,
            LocationId = 1,
            DrinkName = "Nonexistent",
            DrinkDescription = "Description",
            DrinkPrice = 1.99m,
            ImageUrl = "https://picsum.photos/id/102/300/200",
        };

        var response = await _client.PutAsJsonAsync("/api/drink/99999", updatedDrink);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDrinkByID_RemovesDrink()
    {
        // Create a new drink for deletion
        var drinkId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Drink To Delete",
                DrinkDescription = "Will be deleted",
                DrinkPrice = 1.99m,
                ImageUrl = "https://example.com/img.jpg",
            }
        );

        var response = await _client.DeleteAsync($"/api/drink/{drinkId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/drink/{drinkId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteDrinkByID_ReturnsNotFound_WhenDrinkDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/drink/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
