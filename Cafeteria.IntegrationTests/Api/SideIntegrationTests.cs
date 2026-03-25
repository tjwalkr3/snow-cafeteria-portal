using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class SideIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public SideIntegrationTests(DatabaseFixture fixture)
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
    public async Task CreateSide_AddsNewSide()
    {
        // Use pre-loaded station with ID 1
        var newSide = new SideDto
        {
            StationId = 1,
            SideName = "Test Onion Rings",
            SideDescription = "Crispy fried onion rings for testing",
            SidePrice = 3.49m,
        };

        var response = await _client.PostAsJsonAsync("/api/side", newSide);
        response.EnsureSuccessStatusCode();
        var createdSide = await response.Content.ReadFromJsonAsync<SideDto>();

        Assert.NotNull(createdSide);
        Assert.Equal(newSide.SideName, createdSide.SideName);
        Assert.Equal(newSide.SideDescription, createdSide.SideDescription);
        Assert.Equal(newSide.SidePrice, createdSide.SidePrice);
        Assert.True(createdSide.Id > 0);
    }

    [Fact]
    public async Task GetSideByID_ReturnsCorrectSide()
    {
        // Use pre-loaded side with ID 1
        var response = await _client.GetAsync("/api/side/1");
        response.EnsureSuccessStatusCode();
        var side = await response.Content.ReadFromJsonAsync<SideDto>();

        Assert.NotNull(side);
        Assert.Equal("French Fries", side.SideName);
    }

    [Fact]
    public async Task GetSideByID_ReturnsNotFound_WhenSideDoesNotExist()
    {
        var response = await _client.GetAsync("/api/side/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllSides_ReturnsAllSides()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/side");
        response.EnsureSuccessStatusCode();
        var sides = await response.Content.ReadFromJsonAsync<List<SideDto>>();

        Assert.NotNull(sides);
        Assert.True(sides.Count >= 3);
        Assert.Contains(sides, s => s.SideName == "French Fries");
        Assert.Contains(sides, s => s.SideName == "Coleslaw");
    }

    [Fact]
    public async Task GetSidesByStationID_ReturnsSidesForStation()
    {
        // Station 1 has sides 1 and 2
        var response = await _client.GetAsync("/api/side/station/1");
        response.EnsureSuccessStatusCode();
        var sides = await response.Content.ReadFromJsonAsync<List<SideDto>>();

        Assert.NotNull(sides);
        Assert.True(sides.Count >= 2);
        Assert.All(sides, side => Assert.Equal(1, side.StationId));
    }

    [Fact]
    public async Task UpdateSideByID_UpdatesExistingSide()
    {
        // Create a new side for this test
        var sideId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Side To Update",
                SideDescription = "Original",
                SidePrice = 2.99m,
                CardOnly = false,
                SwipeOnly = false
            }
        );

        var updatedSide = new SideDto
        {
            Id = sideId,
            StationId = 1,
            SideName = "Updated Side",
            SideDescription = "Updated description",
            SidePrice = 4.49m,
        };

        var response = await _client.PutAsJsonAsync($"/api/side/{sideId}", updatedSide);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SideDto>();

        Assert.NotNull(result);
        Assert.Equal(updatedSide.SideName, result.SideName);
        Assert.Equal(updatedSide.SideDescription, result.SideDescription);
        Assert.Equal(updatedSide.SidePrice, result.SidePrice);
    }

    [Fact]
    public async Task UpdateSideByID_ReturnsNotFound_WhenSideDoesNotExist()
    {
        var updatedSide = new SideDto
        {
            Id = 99999,
            StationId = 1,
            SideName = "Nonexistent",
            SideDescription = "Description",
            SidePrice = 2.99m,
        };

        var response = await _client.PutAsJsonAsync("/api/side/99999", updatedSide);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteSideByID_RemovesSide()
    {
        // Create a new side for deletion
        var sideId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Side To Delete",
                SideDescription = "Will be deleted",
                SidePrice = 1.99m,
                CardOnly = false,
                SwipeOnly = false
            }
        );

        var response = await _client.DeleteAsync($"/api/side/{sideId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/side/{sideId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteSideByID_ReturnsNotFound_WhenSideDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/side/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SetStockStatusById_UpdatesInStockStatus()
    {
        // Create a new side for this test
        var sideId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Side To Stock Toggle",
                SideDescription = "Testing stock status",
                SidePrice = 3.99m,
                CardOnly = false,
                SwipeOnly = false
            }
        );

        // Set stock status to false
        var response = await _client.PutAsJsonAsync($"/api/side/{sideId}/stock", false);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the stock status was updated by retrieving the side
        var getResponse = await _client.GetAsync($"/api/side/{sideId}");
        getResponse.EnsureSuccessStatusCode();
        var side = await getResponse.Content.ReadFromJsonAsync<SideDto>();

        Assert.NotNull(side);
        Assert.False(side.InStock);
    }

    [Fact]
    public async Task GetSwipeSidesByStationID_ReturnsOnlySwipeEligibleSides()
    {
        // Create test sides with different card/swipe flags
        var swipeOnlyId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Swipe Only Side",
                SideDescription = "Available for swipe orders only",
                SidePrice = 2.49m,
                CardOnly = false,
                SwipeOnly = true
            }
        );

        var cardOnlyId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Card Only Side",
                SideDescription = "Available for card orders only",
                SidePrice = 2.99m,
                CardOnly = true,
                SwipeOnly = false
            }
        );

        var bothId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Both Cards And Swipes Side",
                SideDescription = "Available for both payment types",
                SidePrice = 3.49m,
                CardOnly = false,
                SwipeOnly = false
            }
        );

        // Get swipe-eligible sides
        var response = await _client.GetAsync("/api/side/station/1/swipe");
        response.EnsureSuccessStatusCode();
        var sides = await response.Content.ReadFromJsonAsync<List<SideDto>>();

        Assert.NotNull(sides);
        // Should include swipe-only and both
        Assert.Contains(sides, s => s.Id == swipeOnlyId && s.SwipeOnly);
        Assert.Contains(sides, s => s.Id == bothId && !s.CardOnly && !s.SwipeOnly);
        // Should NOT include card-only
        Assert.DoesNotContain(sides, s => s.Id == cardOnlyId);
    }

    [Fact]
    public async Task GetCardSidesByStationID_ReturnsOnlyCardEligibleSides()
    {
        // Create test sides with different card/swipe flags
        var swipeOnlyId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Swipe Only For Card Filter Test",
                SideDescription = "Available for swipe orders only",
                SidePrice = 2.49m,
                CardOnly = false,
                SwipeOnly = true
            }
        );

        var cardOnlyId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Card Only For Card Filter Test",
                SideDescription = "Available for card orders only",
                SidePrice = 2.99m,
                CardOnly = true,
                SwipeOnly = false
            }
        );

        var bothId = _connection.ExecuteScalar<int>(
            InsertSideSql + " RETURNING id",
            new
            {
                StationId = 1,
                SideName = "Both For Card Filter Test",
                SideDescription = "Available for both payment types",
                SidePrice = 3.49m,
                CardOnly = false,
                SwipeOnly = false
            }
        );

        // Get card-eligible sides
        var response = await _client.GetAsync("/api/side/station/1/card");
        response.EnsureSuccessStatusCode();
        var sides = await response.Content.ReadFromJsonAsync<List<SideDto>>();

        Assert.NotNull(sides);
        // Should include card-only and both
        Assert.Contains(sides, s => s.Id == cardOnlyId && s.CardOnly);
        Assert.Contains(sides, s => s.Id == bothId && !s.CardOnly && !s.SwipeOnly);
        // Should NOT include swipe-only
        Assert.DoesNotContain(sides, s => s.Id == swipeOnlyId);
    }

    [Fact]
    public async Task SetStockStatusById_ReturnsNotFound_WhenSideDoesNotExist()
    {
        var response = await _client.PutAsJsonAsync("/api/side/99999/stock", false);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
