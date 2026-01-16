using System.Net.Http.Json;
using Cafeteria.Shared.DTOs;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SampleMenuData;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class StationIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public StationIntegrationTests(DatabaseFixture fixture)
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
    public async Task GetAllStations_ReturnsAllStations()
    {
        // Use pre-loaded sample data (IDs 1-3)
        var response = await _client.GetAsync("/api/station");
        response.EnsureSuccessStatusCode();
        var stations = await response.Content.ReadFromJsonAsync<List<StationDto>>();

        Assert.NotNull(stations);
        Assert.True(stations.Count >= 3);
        Assert.Contains(stations, s => s.StationName == "Sandwich Station");
        Assert.Contains(stations, s => s.StationName == "Grill Station");
    }

    [Fact]
    public async Task GetStationsByLocation_ReturnsStationsForLocation()
    {
        // Location 1 has stations 1 and 2
        var response = await _client.GetAsync("/api/station/station/1");
        response.EnsureSuccessStatusCode();
        var stations = await response.Content.ReadFromJsonAsync<List<StationDto>>();

        Assert.NotNull(stations);
        Assert.True(stations.Count >= 2);
        Assert.All(stations, station => Assert.Equal(1, station.LocationId));
    }

    [Fact]
    public async Task GetStationById_ReturnsCorrectStation()
    {
        // Use pre-loaded station with ID 1
        var response = await _client.GetAsync("/api/station/1");
        response.EnsureSuccessStatusCode();
        var station = await response.Content.ReadFromJsonAsync<StationDto>();

        Assert.NotNull(station);
        Assert.Equal("Sandwich Station", station.StationName);
    }

    [Fact]
    public async Task GetStationById_ReturnsNotFound_WhenStationDoesNotExist()
    {
        var response = await _client.GetAsync("/api/station/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateStationForLocation_AddsNewStation()
    {
        var newStation = new
        {
            Name = "Test New Station",
            Description = "A brand new test station",
        };

        var response = await _client.PostAsJsonAsync("/api/station/station/1", newStation);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync("/api/station/station/1");
        var stations = await getResponse.Content.ReadFromJsonAsync<List<StationDto>>();
        Assert.NotNull(stations);
        Assert.Contains(stations, s => s.StationName == newStation.Name);
    }

    [Fact]
    public async Task UpdateStation_UpdatesExistingStation()
    {
        // Create a new station for this test
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station To Update",
                StationDescription = "Original description",
            }
        );

        var updatedStation = new
        {
            Name = "Updated Station Name",
            Description = "Updated description text",
        };

        var response = await _client.PutAsJsonAsync($"/api/station/{stationId}", updatedStation);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/station/{stationId}");
        var station = await getResponse.Content.ReadFromJsonAsync<StationDto>();
        Assert.NotNull(station);
        Assert.Equal(updatedStation.Name, station.StationName);
        Assert.Equal(updatedStation.Description, station.StationDescription);
    }

    [Fact]
    public async Task DeleteStation_RemovesStation()
    {
        // Create a new station for deletion
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station To Delete",
                StationDescription = "Will be deleted",
            }
        );

        var response = await _client.DeleteAsync($"/api/station/{stationId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/station/{stationId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetStationBusinessHours_ReturnsBusinessHoursForStation()
    {
        // Create test station and hours
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station With Hours",
                StationDescription = "Has business hours",
            }
        );

        _connection.Execute(
            @"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')",
            new { StationId = stationId }
        );

        var response = await _client.GetAsync($"/api/station/{stationId}/hours");
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<List<StationBusinessHoursDto>>();

        Assert.NotNull(hours);
        Assert.Single(hours);
        Assert.Equal(stationId, hours[0].StationId);
    }

    [Fact]
    public async Task GetStationBusinessHoursById_ReturnsCorrectBusinessHours()
    {
        // Create test data
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station For Hours Test",
                StationDescription = "Testing hours",
            }
        );

        var hoursId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { StationId = stationId }
        );

        var response = await _client.GetAsync($"/api/station/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<StationBusinessHoursDto>();

        Assert.NotNull(hours);
        Assert.Equal(stationId, hours.StationId);
    }

    [Fact]
    public async Task GetStationBusinessHoursById_ReturnsNotFound_WhenHoursDoNotExist()
    {
        var response = await _client.GetAsync("/api/station/hours/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddStationHours_AddsNewBusinessHours()
    {
        // Create test station
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station For Add Hours",
                StationDescription = "Adding hours",
            }
        );

        var newHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T08:00:00"),
            EndTime = DateTime.Parse("2025-01-01T17:00:00"),
            WeekdayId = 1,
        };

        var response = await _client.PostAsJsonAsync($"/api/station/{stationId}/hours", newHours);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/station/{stationId}/hours");
        var hours = await getResponse.Content.ReadFromJsonAsync<List<StationBusinessHoursDto>>();
        Assert.NotNull(hours);
        Assert.Single(hours);
    }

    [Fact]
    public async Task AddStationHours_ReturnsBadRequest_WhenWeekdayIdIsInvalid()
    {
        // Create test station
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station Bad Weekday",
                StationDescription = "Testing bad weekday",
            }
        );

        var newHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T08:00:00"),
            EndTime = DateTime.Parse("2025-01-01T17:00:00"),
            WeekdayId = 999,
        };

        var response = await _client.PostAsJsonAsync($"/api/station/{stationId}/hours", newHours);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStationHours_UpdatesExistingBusinessHours()
    {
        // Create test data
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station Update Hours",
                StationDescription = "Updating hours",
            }
        );

        var hoursId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { StationId = stationId }
        );

        var updatedHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T09:00:00"),
            EndTime = DateTime.Parse("2025-01-01T18:00:00"),
            WeekdayId = 2,
        };

        var response = await _client.PutAsJsonAsync($"/api/station/hours/{hoursId}", updatedHours);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/station/hours/{hoursId}");
        var hours = await getResponse.Content.ReadFromJsonAsync<StationBusinessHoursDto>();
        Assert.NotNull(hours);
        Assert.Equal(2, hours.WeekdayId);
    }

    [Fact]
    public async Task UpdateStationHours_ReturnsBadRequest_WhenWeekdayIdIsInvalid()
    {
        // Create test data
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station Bad Update",
                StationDescription = "Testing bad update",
            }
        );

        var hoursId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { StationId = stationId }
        );

        var updatedHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T09:00:00"),
            EndTime = DateTime.Parse("2025-01-01T18:00:00"),
            WeekdayId = 999,
        };

        var response = await _client.PutAsJsonAsync($"/api/station/hours/{hoursId}", updatedHours);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStationHours_RemovesBusinessHours()
    {
        // Create test data
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = 1,
                StationName = "Station Delete Hours",
                StationDescription = "Deleting hours",
            }
        );

        var hoursId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { StationId = stationId }
        );

        var response = await _client.DeleteAsync($"/api/station/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/station/hours/{hoursId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
