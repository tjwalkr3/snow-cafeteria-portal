using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Npgsql;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class MenuIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public MenuIntegrationTests(DatabaseFixture fixture)
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
    public async Task GetAllLocations_ReturnsLocationData()
    {
        // Use pre-loaded sample data
        var response = await _client.GetAsync("/api/menu/locations");
        response.EnsureSuccessStatusCode();
        var locationsAfter = await response.Content.ReadFromJsonAsync<List<LocationDto>>();

        Assert.NotNull(locationsAfter);
        Assert.True(locationsAfter.Count >= 3);
        Assert.Contains(locationsAfter, l => l.LocationName == "Badger Den");
        Assert.Contains(locationsAfter, l => l.LocationName == "Busters Bistro");
    }

    [Fact]
    public async Task GetStationsByLocation_ReturnsStationData()
    {
        // Use pre-loaded sample data for location 1
        var response = await _client.GetAsync("/api/menu/stations/location/1");
        response.EnsureSuccessStatusCode();
        var stationsAfter = await response.Content.ReadFromJsonAsync<List<StationDto>>();

        Assert.NotNull(stationsAfter);
        Assert.True(stationsAfter.Count >= 2);
        Assert.Contains(stationsAfter, s => s.StationName == "Sandwich Station");
        Assert.Contains(stationsAfter, s => s.StationName == "Grill Station");
    }

    [Fact]
    public async Task GetEntreesByStation_ReturnsEntreeData()
    {
        // Use pre-loaded sample data for station 1
        var response = await _client.GetAsync("/api/menu/entrees/station/1");
        response.EnsureSuccessStatusCode();
        var entreesAfter = await response.Content.ReadFromJsonAsync<List<EntreeDto>>();

        Assert.NotNull(entreesAfter);
        Assert.True(entreesAfter.Count >= 2);
        Assert.Contains(entreesAfter, e => e.EntreeName == "Grilled Chicken");
        Assert.Contains(entreesAfter, e => e.EntreeName == "Burger");
    }

    [Fact]
    public async Task GetSidesByStation_ReturnsSideData()
    {
        // Use pre-loaded sample data for station 1
        var response = await _client.GetAsync("/api/menu/sides/station/1");
        response.EnsureSuccessStatusCode();
        var sidesAfter = await response.Content.ReadFromJsonAsync<List<SideDto>>();

        Assert.NotNull(sidesAfter);
        Assert.True(sidesAfter.Count >= 2);
        Assert.Contains(sidesAfter, s => s.SideName == "French Fries");
        Assert.Contains(sidesAfter, s => s.SideName == "Coleslaw");
    }

    [Fact]
    public async Task GetDrinksByLocation_ReturnsDrinkData()
    {
        // Use pre-loaded sample data for location 1
        var response = await _client.GetAsync("/api/menu/drinks/location/1");
        response.EnsureSuccessStatusCode();
        var drinksAfter = await response.Content.ReadFromJsonAsync<List<DrinkDto>>();

        Assert.NotNull(drinksAfter);
        Assert.True(drinksAfter.Count >= 2);
        Assert.Contains(drinksAfter, d => d.DrinkName == "Coca-Cola");
        Assert.Contains(drinksAfter, d => d.DrinkName == "Lemonade");
    }

    [Fact]
    public async Task GetFoodOptionsByEntree_ReturnsFoodOptionData()
    {
        // Use pre-loaded sample data for entree 1
        var response = await _client.GetAsync("/api/menu/options/entree/1");
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
        var response = await _client.GetAsync("/api/menu/options/side/1");
        response.EnsureSuccessStatusCode();
        var optionsAfter = await response.Content.ReadFromJsonAsync<List<FoodOptionDto>>();

        Assert.NotNull(optionsAfter);
        // May be empty or have options depending on sample data configuration
        // Just verify it doesn't error
    }
}
