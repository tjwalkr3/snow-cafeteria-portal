using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SampleMenuData;

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
            ImageUrl = "https://picsum.photos/id/300/300/200",
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
                ImageUrl = "https://example.com/img.jpg",
            }
        );

        var updatedSide = new SideDto
        {
            Id = sideId,
            StationId = 1,
            SideName = "Updated Side",
            SideDescription = "Updated description",
            SidePrice = 4.49m,
            ImageUrl = "https://picsum.photos/id/301/300/200",
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
            ImageUrl = "https://picsum.photos/id/302/300/200",
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
                SidePrice = 2.99m,
                ImageUrl = "https://example.com/img.jpg",
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
}
