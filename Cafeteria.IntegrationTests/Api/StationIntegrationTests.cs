using System.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Cafeteria.Shared.DTOs;
using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;
using static Cafeteria.IntegrationTests.Api.DBSql;
using static Cafeteria.IntegrationTests.Api.SampleMenuData;

namespace Cafeteria.IntegrationTests.Api;

public class StationIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private NpgsqlConnection _connection = null!;

    public StationIntegrationTests()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("cafeteria")
            .WithUsername("cafeteria_admin")
            .WithPassword("SnowCafe")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        _connection = new NpgsqlConnection(_postgresContainer.GetConnectionString());
        await _connection.OpenAsync();
        await _connection.ExecuteAsync(SqlData);

        var connectionString = _postgresContainer.GetConnectionString();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbConnection));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddScoped<IDbConnection>(_ =>
                    {
                        var conn = new NpgsqlConnection(connectionString);
                        conn.Open();
                        return conn;
                    });
                });
            });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task GetAllStations_ReturnsAllStations()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertStationSql, Stations[1]);

        var response = await _client.GetAsync("/api/station");
        response.EnsureSuccessStatusCode();
        var stations = await response.Content.ReadFromJsonAsync<List<StationDto>>();

        Assert.NotNull(stations);
        Assert.Equal(2, stations.Count);
        Assert.Contains(stations, s => s.StationName == Stations[0].StationName);
        Assert.Contains(stations, s => s.StationName == Stations[1].StationName);
    }

    [Fact]
    public async Task GetStationsByLocation_ReturnsStationsForLocation()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertLocationSql, Locations[1]);
        _connection.Execute(InsertStationSql, Stations[0]);
        _connection.Execute(InsertStationSql, Stations[1]);

        var response = await _client.GetAsync("/api/station/station/1");
        response.EnsureSuccessStatusCode();
        var stations = await response.Content.ReadFromJsonAsync<List<StationDto>>();

        Assert.NotNull(stations);
        Assert.Equal(2, stations.Count);
        Assert.All(stations, station => Assert.Equal(1, station.LocationId));
    }

    [Fact]
    public async Task GetStationById_ReturnsCorrectStation()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        var response = await _client.GetAsync($"/api/station/{stationId}");
        response.EnsureSuccessStatusCode();
        var station = await response.Content.ReadFromJsonAsync<StationDto>();

        Assert.NotNull(station);
        Assert.Equal(Stations[0].StationName, station.StationName);
        Assert.Equal(Stations[0].StationDescription, station.StationDescription);
    }

    [Fact]
    public async Task GetStationById_ReturnsNotFound_WhenStationDoesNotExist()
    {
        var response = await _client.GetAsync("/api/station/999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateStationForLocation_AddsNewStation()
    {
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        var newStation = new { Name = "New Station", Description = "A brand new station" };

        var response = await _client.PostAsJsonAsync($"/api/station/station/{locationId}", newStation);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/station/station/{locationId}");
        var stations = await getResponse.Content.ReadFromJsonAsync<List<StationDto>>();
        Assert.NotNull(stations);
        Assert.Single(stations);
        Assert.Equal(newStation.Name, stations[0].StationName);
    }

    [Fact]
    public async Task UpdateStation_UpdatesExistingStation()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        var updatedStation = new { Name = "Updated Station", Description = "Updated description" };

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
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        var response = await _client.DeleteAsync($"/api/station/{stationId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/station/{stationId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetStationBusinessHours_ReturnsBusinessHoursForStation()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");
        _connection.Execute(@"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')",
            new { StationId = stationId });

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
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");
        var hoursId = _connection.ExecuteScalar<int>(@"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { StationId = stationId });

        var response = await _client.GetAsync($"/api/station/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<StationBusinessHoursDto>();

        Assert.NotNull(hours);
        Assert.Equal(stationId, hours.StationId);
    }

    [Fact]
    public async Task GetStationBusinessHoursById_ReturnsNotFound_WhenHoursDoNotExist()
    {
        var response = await _client.GetAsync("/api/station/hours/999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddStationHours_AddsNewBusinessHours()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");

        var newHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T08:00:00"),
            EndTime = DateTime.Parse("2025-01-01T17:00:00"),
            WeekdayId = 1
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
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        var newHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T08:00:00"),
            EndTime = DateTime.Parse("2025-01-01T17:00:00"),
            WeekdayId = 999
        };

        var response = await _client.PostAsJsonAsync($"/api/station/{stationId}/hours", newHours);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStationHours_UpdatesExistingBusinessHours()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");
        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (2, 'Tuesday')");

        var hoursId = _connection.ExecuteScalar<int>(@"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { StationId = stationId });

        var updatedHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T09:00:00"),
            EndTime = DateTime.Parse("2025-01-01T18:00:00"),
            WeekdayId = 2
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
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");

        var hoursId = _connection.ExecuteScalar<int>(@"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { StationId = stationId });

        var updatedHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T09:00:00"),
            EndTime = DateTime.Parse("2025-01-01T18:00:00"),
            WeekdayId = 999
        };

        var response = await _client.PutAsJsonAsync($"/api/station/hours/{hoursId}", updatedHours);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStationHours_RemovesBusinessHours()
    {
        _connection.Execute(InsertLocationSql, Locations[0]);
        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            Stations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");

        var hoursId = _connection.ExecuteScalar<int>(@"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@StationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { StationId = stationId });

        var response = await _client.DeleteAsync($"/api/station/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/station/hours/{hoursId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
