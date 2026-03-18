using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class LocationExceptionIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public LocationExceptionIntegrationTests(DatabaseFixture fixture)
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
    public async Task GetLocationExceptionsByLocationId_ReturnsExceptionsForLocation()
    {
        // Create a test location
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Exceptions",
                LocationDescription = "Testing exceptions",
                IconId = 1,
            }
        );

        // Create test exception
        var now = DateTime.UtcNow;
        _connection.Execute(
            @"
            INSERT INTO cafeteria.location_exception_hours (location_id, start_exception_datetime, end_exception_datetime, reason)
            VALUES (@LocationId, @StartDateTime, @EndDateTime, @Reason)",
            new
            {
                LocationId = locationId,
                StartDateTime = now.AddHours(1),
                EndDateTime = now.AddHours(2),
                Reason = "Test closure"
            }
        );

        var response = await _client.GetAsync($"/api/SchedulingExceptions/location/{locationId}");
        response.EnsureSuccessStatusCode();
        var exceptions = await response.Content.ReadFromJsonAsync<List<LocationExceptionHoursDto>>();

        Assert.NotNull(exceptions);
        Assert.Single(exceptions);
        Assert.Equal(locationId, exceptions[0].LocationId);
        Assert.Equal("Test closure", exceptions[0].Reason);
    }

    [Fact]
    public async Task GetLocationExceptionById_ReturnsCorrectException()
    {
        // Create a test location and exception
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Exception Detail",
                LocationDescription = "Testing exception detail",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var exceptionId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.location_exception_hours (location_id, start_exception_datetime, end_exception_datetime, reason)
            VALUES (@LocationId, @StartDateTime, @EndDateTime, @Reason)
            RETURNING id",
            new
            {
                LocationId = locationId,
                StartDateTime = now.AddHours(1),
                EndDateTime = now.AddHours(2),
                Reason = "Test closure"
            }
        );

        var response = await _client.GetAsync($"/api/SchedulingExceptions/location/{exceptionId}/detail");
        response.EnsureSuccessStatusCode();
        var exception = await response.Content.ReadFromJsonAsync<LocationExceptionHoursDto>();

        Assert.NotNull(exception);
        Assert.Equal(locationId, exception.LocationId);
        Assert.Equal("Test closure", exception.Reason);
    }

    [Fact]
    public async Task GetLocationExceptionById_ReturnsNotFound_WhenExceptionDoesNotExist()
    {
        var response = await _client.GetAsync("/api/SchedulingExceptions/location/99999/detail");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddLocationException_AddsNewException()
    {
        // Create a test location
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Add Exception",
                LocationDescription = "Testing add exception",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var newException = new
        {
            StartDateTime = now.AddHours(1),
            EndDateTime = now.AddHours(2),
            Reason = "Scheduled maintenance"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/SchedulingExceptions/location/{locationId}",
            newException
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the exception was created
        var getResponse = await _client.GetAsync($"/api/SchedulingExceptions/location/{locationId}");
        var exceptions = await getResponse.Content.ReadFromJsonAsync<List<LocationExceptionHoursDto>>();
        Assert.NotNull(exceptions);
        Assert.Single(exceptions);
        Assert.Equal("Scheduled maintenance", exceptions[0].Reason);
    }

    [Fact]
    public async Task AddLocationException_ReturnsBadRequest_WhenStartTimeIsAfterEndTime()
    {
        // Create a test location
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location Bad Times",
                LocationDescription = "Testing bad times",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var badException = new
        {
            StartDateTime = now.AddHours(2),
            EndDateTime = now.AddHours(1),
            Reason = "Invalid times"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/SchedulingExceptions/location/{locationId}",
            badException
        );
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateLocationException_UpdatesExistingException()
    {
        // Create a test location and exception
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Update",
                LocationDescription = "Testing update",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var exceptionId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.location_exception_hours (location_id, start_exception_datetime, end_exception_datetime, reason)
            VALUES (@LocationId, @StartDateTime, @EndDateTime, @Reason)
            RETURNING id",
            new
            {
                LocationId = locationId,
                StartDateTime = now.AddHours(1),
                EndDateTime = now.AddHours(2),
                Reason = "Original reason"
            }
        );

        var updatedException = new
        {
            StartDateTime = now.AddHours(3),
            EndDateTime = now.AddHours(4),
            Reason = "Updated reason"
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/SchedulingExceptions/location/{exceptionId}",
            updatedException
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var getResponse = await _client.GetAsync($"/api/SchedulingExceptions/location/{exceptionId}/detail");
        var exception = await getResponse.Content.ReadFromJsonAsync<LocationExceptionHoursDto>();
        Assert.NotNull(exception);
        Assert.Equal("Updated reason", exception.Reason);
    }

    [Fact]
    public async Task UpdateLocationException_ReturnsNotFound_WhenExceptionDoesNotExist()
    {
        var now = DateTime.UtcNow;
        var updatedException = new
        {
            StartDateTime = now.AddHours(1),
            EndDateTime = now.AddHours(2),
            Reason = "Nonexistent"
        };

        var response = await _client.PutAsJsonAsync(
            "/api/SchedulingExceptions/location/99999",
            updatedException
        );
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteLocationException_RemovesException()
    {
        // Create a test location and exception
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Delete",
                LocationDescription = "Testing delete",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var exceptionId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.location_exception_hours (location_id, start_exception_datetime, end_exception_datetime, reason)
            VALUES (@LocationId, @StartDateTime, @EndDateTime, @Reason)
            RETURNING id",
            new
            {
                LocationId = locationId,
                StartDateTime = now.AddHours(1),
                EndDateTime = now.AddHours(2),
                Reason = "To be deleted"
            }
        );

        var response = await _client.DeleteAsync($"/api/SchedulingExceptions/location/{exceptionId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the deletion
        var getResponse = await _client.GetAsync($"/api/SchedulingExceptions/location/{exceptionId}/detail");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteLocationException_ReturnsNotFound_WhenExceptionDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/SchedulingExceptions/location/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
