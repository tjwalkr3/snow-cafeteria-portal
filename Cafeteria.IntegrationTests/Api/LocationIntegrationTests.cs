using System.Data;
using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class LocationIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public LocationIntegrationTests(DatabaseFixture fixture)
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
    public async Task GetAllLocations_ReturnsAllLocations()
    {
        // Use pre-loaded sample data (IDs 1-3)
        var response = await _client.GetAsync("/api/location");
        response.EnsureSuccessStatusCode();
        var locations = await response.Content.ReadFromJsonAsync<List<LocationDto>>();

        Assert.NotNull(locations);
        Assert.True(locations.Count >= 3); // At least our sample data
        Assert.Contains(locations, l => l.LocationName == "Badger Den");
        Assert.Contains(locations, l => l.LocationName == "Busters Bistro");
    }

    [Fact]
    public async Task GetLocationById_ReturnsCorrectLocation()
    {
        // Use pre-loaded location with ID 1
        var response = await _client.GetAsync("/api/location/1");
        response.EnsureSuccessStatusCode();
        var location = await response.Content.ReadFromJsonAsync<LocationDto>();

        Assert.NotNull(location);
        Assert.Equal("Badger Den", location.LocationName);
    }

    [Fact]
    public async Task GetLocationById_ReturnsNotFound_WhenLocationDoesNotExist()
    {
        var response = await _client.GetAsync("/api/location/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocation_AddsNewLocation()
    {
        var newLocation = new
        {
            Name = "Test New Cafeteria",
            Description = "A brand new test location",
        };

        var response = await _client.PostAsJsonAsync("/api/location", newLocation);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify it was created (should have ID >= 100)
        var getResponse = await _client.GetAsync("/api/location");
        var locations = await getResponse.Content.ReadFromJsonAsync<List<LocationDto>>();
        Assert.NotNull(locations);
        Assert.Contains(locations, l => l.LocationName == newLocation.Name);
    }

    [Fact]
    public async Task UpdateLocation_UpdatesExistingLocation()
    {
        // Create a new location specifically for this test
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location To Update",
                LocationDescription = "Original description",
                IconName = "bi-geo-alt-fill",
            }
        );

        var updatedLocation = new
        {
            Name = "Updated Location Name",
            Description = "Updated description text",
        };

        var response = await _client.PutAsJsonAsync($"/api/location/{locationId}", updatedLocation);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/location/{locationId}");
        var location = await getResponse.Content.ReadFromJsonAsync<LocationDto>();
        Assert.NotNull(location);
        Assert.Equal(updatedLocation.Name, location.LocationName);
        Assert.Equal(updatedLocation.Description, location.LocationDescription);
    }

    [Fact]
    public async Task DeleteLocation_RemovesLocation()
    {
        // Create a new location specifically for deletion
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location To Delete",
                LocationDescription = "Will be deleted",
                IconName = "bi-geo-alt-fill",
            }
        );

        var response = await _client.DeleteAsync($"/api/location/{locationId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/location/{locationId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetLocationBusinessHours_ReturnsBusinessHoursForLocation()
    {
        // Create a test location and hours
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location With Hours",
                LocationDescription = "Has business hours",
                IconName = "bi-geo-alt-fill",
            }
        );

        _connection.Execute(
            @"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')",
            new { LocationId = locationId }
        );

        var response = await _client.GetAsync($"/api/location/{locationId}/hours");
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<List<LocationBusinessHoursDto>>();

        Assert.NotNull(hours);
        Assert.Single(hours);
        Assert.Equal(locationId, hours[0].LocationId);
    }

    [Fact]
    public async Task GetLocationBusinessHoursById_ReturnsCorrectBusinessHours()
    {
        // Create test data
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Hours Test",
                LocationDescription = "Testing hours",
                IconName = "bi-geo-alt-fill",
            }
        );

        var hoursId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { LocationId = locationId }
        );

        var response = await _client.GetAsync($"/api/location/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<LocationBusinessHoursDto>();

        Assert.NotNull(hours);
        Assert.Equal(locationId, hours.LocationId);
    }

    [Fact]
    public async Task GetLocationBusinessHoursById_ReturnsNotFound_WhenHoursDoNotExist()
    {
        var response = await _client.GetAsync("/api/location/hours/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddLocationHours_AddsNewBusinessHours()
    {
        // Create test location
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Add Hours",
                LocationDescription = "Adding hours",
                IconName = "bi-geo-alt-fill",
            }
        );

        var newHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T08:00:00"),
            EndTime = DateTime.Parse("2025-01-01T17:00:00"),
            WeekdayId = 1,
        };

        var response = await _client.PostAsJsonAsync($"/api/location/{locationId}/hours", newHours);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/location/{locationId}/hours");
        var hours = await getResponse.Content.ReadFromJsonAsync<List<LocationBusinessHoursDto>>();
        Assert.NotNull(hours);
        Assert.Single(hours);
    }

    [Fact]
    public async Task AddLocationHours_ReturnsBadRequest_WhenWeekdayIdIsInvalid()
    {
        // Create test location
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location Bad Weekday",
                LocationDescription = "Testing bad weekday",
                IconName = "bi-geo-alt-fill",
            }
        );

        var newHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T08:00:00"),
            EndTime = DateTime.Parse("2025-01-01T17:00:00"),
            WeekdayId = 999,
        };

        var response = await _client.PostAsJsonAsync($"/api/location/{locationId}/hours", newHours);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocationHours_UpdatesExistingBusinessHours()
    {
        // Create test data
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location Update Hours",
                LocationDescription = "Updating hours",
                IconName = "bi-geo-alt-fill",
            }
        );

        var hoursId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { LocationId = locationId }
        );

        var updatedHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T09:00:00"),
            EndTime = DateTime.Parse("2025-01-01T18:00:00"),
            WeekdayId = 2,
        };

        var response = await _client.PutAsJsonAsync($"/api/location/hours/{hoursId}", updatedHours);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/location/hours/{hoursId}");
        var hours = await getResponse.Content.ReadFromJsonAsync<LocationBusinessHoursDto>();
        Assert.NotNull(hours);
        Assert.Equal(2, hours.WeekdayId);
    }

    [Fact]
    public async Task UpdateLocationHours_ReturnsBadRequest_WhenWeekdayIdIsInvalid()
    {
        // Create test data
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location Bad Update",
                LocationDescription = "Testing bad update",
                IconName = "bi-geo-alt-fill",
            }
        );

        var hoursId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { LocationId = locationId }
        );

        var updatedHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T09:00:00"),
            EndTime = DateTime.Parse("2025-01-01T18:00:00"),
            WeekdayId = 999,
        };

        var response = await _client.PutAsJsonAsync($"/api/location/hours/{hoursId}", updatedHours);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLocationHours_RemovesBusinessHours()
    {
        // Create test data
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location Delete Hours",
                LocationDescription = "Deleting hours",
                IconName = "bi-geo-alt-fill",
            }
        );

        var hoursId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { LocationId = locationId }
        );

        var response = await _client.DeleteAsync($"/api/location/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/location/hours/{hoursId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
