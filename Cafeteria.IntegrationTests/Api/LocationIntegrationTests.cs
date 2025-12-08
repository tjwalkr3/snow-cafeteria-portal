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

public class LocationIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private NpgsqlConnection _connection = null!;

    public LocationIntegrationTests()
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
    public async Task GetAllLocations_ReturnsAllLocations()
    {
        // Arrange
        _connection.Execute(InsertLocationSql, Locations[0]);
        _connection.Execute(InsertLocationSql, Locations[1]);

        // Act
        var response = await _client.GetAsync("/api/location");
        response.EnsureSuccessStatusCode();
        var locations = await response.Content.ReadFromJsonAsync<List<LocationDto>>();

        // Assert
        Assert.NotNull(locations);
        Assert.Equal(2, locations.Count);
        Assert.Equal(Locations[0].LocationName, locations[0].LocationName);
        Assert.Equal(Locations[1].LocationName, locations[1].LocationName);
    }

    [Fact]
    public async Task GetLocationById_ReturnsCorrectLocation()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        // Act
        var response = await _client.GetAsync($"/api/location/{locationId}");
        response.EnsureSuccessStatusCode();
        var location = await response.Content.ReadFromJsonAsync<LocationDto>();

        // Assert
        Assert.NotNull(location);
        Assert.Equal(Locations[0].LocationName, location.LocationName);
        Assert.Equal(Locations[0].LocationDescription, location.LocationDescription);
    }

    [Fact]
    public async Task GetLocationById_ReturnsNotFound_WhenLocationDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/location/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocation_AddsNewLocation()
    {
        // Arrange
        var newLocation = new { Name = "New Cafeteria", Description = "A brand new location" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/location", newLocation);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify creation
        var getResponse = await _client.GetAsync("/api/location");
        var locations = await getResponse.Content.ReadFromJsonAsync<List<LocationDto>>();
        Assert.NotNull(locations);
        Assert.Single(locations);
        Assert.Equal(newLocation.Name, locations[0].LocationName);
    }

    [Fact]
    public async Task UpdateLocation_UpdatesExistingLocation()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        var updatedLocation = new { Name = "Updated Location", Description = "Updated description" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/location/{locationId}", updatedLocation);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify update
        var getResponse = await _client.GetAsync($"/api/location/{locationId}");
        var location = await getResponse.Content.ReadFromJsonAsync<LocationDto>();
        Assert.NotNull(location);
        Assert.Equal(updatedLocation.Name, location.LocationName);
        Assert.Equal(updatedLocation.Description, location.LocationDescription);
    }

    [Fact]
    public async Task DeleteLocation_RemovesLocation()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        // Act
        var response = await _client.DeleteAsync($"/api/location/{locationId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/location/{locationId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetLocationBusinessHours_ReturnsBusinessHoursForLocation()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");
        _connection.Execute(@"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')",
            new { LocationId = locationId });

        // Act
        var response = await _client.GetAsync($"/api/location/{locationId}/hours");
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<List<LocationBusinessHoursDto>>();

        // Assert
        Assert.NotNull(hours);
        Assert.Single(hours);
        Assert.Equal(locationId, hours[0].LocationId);
    }

    [Fact]
    public async Task GetLocationBusinessHoursById_ReturnsCorrectBusinessHours()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");
        var hoursId = _connection.ExecuteScalar<int>(@"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { LocationId = locationId });

        // Act
        var response = await _client.GetAsync($"/api/location/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
        var hours = await response.Content.ReadFromJsonAsync<LocationBusinessHoursDto>();

        // Assert
        Assert.NotNull(hours);
        Assert.Equal(locationId, hours.LocationId);
    }

    [Fact]
    public async Task GetLocationBusinessHoursById_ReturnsNotFound_WhenHoursDoNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/location/hours/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddLocationHours_AddsNewBusinessHours()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");

        var newHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T08:00:00"),
            EndTime = DateTime.Parse("2025-01-01T17:00:00"),
            WeekdayId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/location/{locationId}/hours", newHours);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify creation
        var getResponse = await _client.GetAsync($"/api/location/{locationId}/hours");
        var hours = await getResponse.Content.ReadFromJsonAsync<List<LocationBusinessHoursDto>>();
        Assert.NotNull(hours);
        Assert.Single(hours);
    }

    [Fact]
    public async Task AddLocationHours_ReturnsBadRequest_WhenWeekdayIdIsInvalid()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        var newHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T08:00:00"),
            EndTime = DateTime.Parse("2025-01-01T17:00:00"),
            WeekdayId = 999
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/location/{locationId}/hours", newHours);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocationHours_UpdatesExistingBusinessHours()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");
        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (2, 'Tuesday')");

        var hoursId = _connection.ExecuteScalar<int>(@"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { LocationId = locationId });

        var updatedHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T09:00:00"),
            EndTime = DateTime.Parse("2025-01-01T18:00:00"),
            WeekdayId = 2
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/location/hours/{hoursId}", updatedHours);
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify update
        var getResponse = await _client.GetAsync($"/api/location/hours/{hoursId}");
        var hours = await getResponse.Content.ReadFromJsonAsync<LocationBusinessHoursDto>();
        Assert.NotNull(hours);
        Assert.Equal(2, hours.WeekdayId);
    }

    [Fact]
    public async Task UpdateLocationHours_ReturnsBadRequest_WhenWeekdayIdIsInvalid()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");

        var hoursId = _connection.ExecuteScalar<int>(@"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { LocationId = locationId });

        var updatedHours = new
        {
            StartTime = DateTime.Parse("2025-01-01T09:00:00"),
            EndTime = DateTime.Parse("2025-01-01T18:00:00"),
            WeekdayId = 999
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/location/hours/{hoursId}", updatedHours);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLocationHours_RemovesBusinessHours()
    {
        // Arrange
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            Locations[0]);

        _connection.Execute("INSERT INTO cafeteria.week_day (id, weekday_name) VALUES (1, 'Monday')");

        var hoursId = _connection.ExecuteScalar<int>(@"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@LocationId, 1, '08:00:00', '17:00:00')
            RETURNING id",
            new { LocationId = locationId });

        // Act
        var response = await _client.DeleteAsync($"/api/location/hours/{hoursId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/location/hours/{hoursId}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
