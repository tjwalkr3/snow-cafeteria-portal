using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

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
                CardOnly = false,
                SwipeOnly = false
            }
        );

        var updatedDrink = new DrinkDto
        {
            Id = drinkId,
            LocationId = 1,
            DrinkName = "Updated Drink",
            DrinkDescription = "Updated description",
            DrinkPrice = 3.99m,
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
                CardOnly = false,
                SwipeOnly = false
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

    [Fact]
    public async Task SetStockStatusById_UpdatesInStockStatus()
    {
        // Create a new drink for this test
        var drinkId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Drink To Stock Toggle",
                DrinkDescription = "Testing stock status",
                DrinkPrice = 1.99m,
                CardOnly = false,
                SwipeOnly = false
            }
        );

        // Set stock status to false
        var response = await _client.PutAsJsonAsync($"/api/drink/{drinkId}/stock", false);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the stock status was updated by retrieving the drink
        var getResponse = await _client.GetAsync($"/api/drink/{drinkId}");
        getResponse.EnsureSuccessStatusCode();
        var drink = await getResponse.Content.ReadFromJsonAsync<DrinkDto>();

        Assert.NotNull(drink);
        Assert.False(drink.InStock);
    }

    [Fact]
    public async Task GetSwipeDrinksByLocationID_ReturnsOnlySwipeEligibleDrinks()
    {
        // Create test drinks with different card/swipe flags
        var swipeOnlyId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Swipe Only Drink",
                DrinkDescription = "Available for swipe orders only",
                DrinkPrice = 1.49m,
                CardOnly = false,
                SwipeOnly = true
            }
        );

        var cardOnlyId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Card Only Drink",
                DrinkDescription = "Available for card orders only",
                DrinkPrice = 1.99m,
                CardOnly = true,
                SwipeOnly = false
            }
        );

        var bothId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Both Cards And Swipes Drink",
                DrinkDescription = "Available for both payment types",
                DrinkPrice = 2.49m,
                CardOnly = false,
                SwipeOnly = false
            }
        );

        // Get swipe-eligible drinks
        var response = await _client.GetAsync("/api/drink/location/1/swipe");
        response.EnsureSuccessStatusCode();
        var drinks = await response.Content.ReadFromJsonAsync<List<DrinkDto>>();

        Assert.NotNull(drinks);
        // Should include swipe-only and both
        Assert.Contains(drinks, d => d.Id == swipeOnlyId && d.SwipeOnly);
        Assert.Contains(drinks, d => d.Id == bothId && !d.CardOnly && !d.SwipeOnly);
        // Should NOT include card-only
        Assert.DoesNotContain(drinks, d => d.Id == cardOnlyId);
    }

    [Fact]
    public async Task GetCardDrinksByLocationID_ReturnsOnlyCardEligibleDrinks()
    {
        // Create test drinks with different card/swipe flags
        var swipeOnlyId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Swipe Only For Card Filter Test",
                DrinkDescription = "Available for swipe orders only",
                DrinkPrice = 1.49m,
                CardOnly = false,
                SwipeOnly = true
            }
        );

        var cardOnlyId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Card Only For Card Filter Test",
                DrinkDescription = "Available for card orders only",
                DrinkPrice = 1.99m,
                CardOnly = true,
                SwipeOnly = false
            }
        );

        var bothId = _connection.ExecuteScalar<int>(
            InsertDrinkSql + " RETURNING id",
            new
            {
                LocationId = 1,
                DrinkName = "Both For Card Filter Test",
                DrinkDescription = "Available for both payment types",
                DrinkPrice = 2.49m,
                CardOnly = false,
                SwipeOnly = false
            }
        );

        // Get card-eligible drinks
        var response = await _client.GetAsync("/api/drink/location/1/card");
        response.EnsureSuccessStatusCode();
        var drinks = await response.Content.ReadFromJsonAsync<List<DrinkDto>>();

        Assert.NotNull(drinks);
        // Should include card-only and both
        Assert.Contains(drinks, d => d.Id == cardOnlyId && d.CardOnly);
        Assert.Contains(drinks, d => d.Id == bothId && !d.CardOnly && !d.SwipeOnly);
        // Should NOT include swipe-only
        Assert.DoesNotContain(drinks, d => d.Id == swipeOnlyId);
    }

    [Fact]
    public async Task SetStockStatusById_ReturnsNotFound_WhenDrinkDoesNotExist()
    {
        var response = await _client.PutAsJsonAsync("/api/drink/99999/stock", false);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
