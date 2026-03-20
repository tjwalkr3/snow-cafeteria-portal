using System.Net.Http.Json;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;
using Npgsql;
using static Cafeteria.IntegrationTests.Api.SqlInsertQueries;

namespace Cafeteria.IntegrationTests.Api;

[Collection("Database")]
public class StationExceptionIntegrationTests : IDisposable
{
    private readonly DatabaseFixture _fixture;
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;

    public StationExceptionIntegrationTests(DatabaseFixture fixture)
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
    public async Task GetStationExceptionsByStationId_ReturnsExceptionsForStation()
    {
        // Create a test location and station
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Station Exceptions",
                LocationDescription = "Testing station exceptions",
                IconId = 1,
            }
        );

        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = locationId,
                StationName = "Station For Exceptions",
                StationDescription = "Testing exceptions",
                IconId = 1,
            }
        );

        // Create test exception
        var now = DateTime.UtcNow;
        _connection.Execute(
            @"
            INSERT INTO cafeteria.station_exception_hours (station_id, start_exception_datetime, end_exception_datetime, reason)
            VALUES (@StationId, @StartDateTime, @EndDateTime, @Reason)",
            new
            {
                StationId = stationId,
                StartDateTime = now.AddHours(1),
                EndDateTime = now.AddHours(2),
                Reason = "Equipment maintenance"
            }
        );

        var response = await _client.GetAsync($"/api/SchedulingExceptions/station/{stationId}");
        response.EnsureSuccessStatusCode();
        var exceptions = await response.Content.ReadFromJsonAsync<List<StationExceptionHoursDto>>();

        Assert.NotNull(exceptions);
        Assert.Single(exceptions);
        Assert.Equal(stationId, exceptions[0].StationId);
        Assert.Equal("Equipment maintenance", exceptions[0].Reason);
    }

    [Fact]
    public async Task GetStationExceptionById_ReturnsCorrectException()
    {
        // Create a test location and station
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Station Exception Detail",
                LocationDescription = "Testing station exception detail",
                IconId = 1,
            }
        );

        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = locationId,
                StationName = "Station For Exception Detail",
                StationDescription = "Testing exception detail",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var exceptionId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.station_exception_hours (station_id, start_exception_datetime, end_exception_datetime, reason)
            VALUES (@StationId, @StartDateTime, @EndDateTime, @Reason)
            RETURNING id",
            new
            {
                StationId = stationId,
                StartDateTime = now.AddHours(1),
                EndDateTime = now.AddHours(2),
                Reason = "Staff training"
            }
        );

        var response = await _client.GetAsync($"/api/SchedulingExceptions/station/{exceptionId}/detail");
        response.EnsureSuccessStatusCode();
        var exception = await response.Content.ReadFromJsonAsync<StationExceptionHoursDto>();

        Assert.NotNull(exception);
        Assert.Equal(stationId, exception.StationId);
        Assert.Equal("Staff training", exception.Reason);
    }

    [Fact]
    public async Task GetStationExceptionById_ReturnsNotFound_WhenExceptionDoesNotExist()
    {
        var response = await _client.GetAsync("/api/SchedulingExceptions/station/99999/detail");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddStationException_AddsNewException()
    {
        // Create a test location and station
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Add Station Exception",
                LocationDescription = "Testing add exception",
                IconId = 1,
            }
        );

        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = locationId,
                StationName = "Station For Add Exception",
                StationDescription = "Testing add exception",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var newException = new
        {
            StartDateTime = now.AddHours(1),
            EndDateTime = now.AddHours(2),
            Reason = "Deep cleaning"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/SchedulingExceptions/station/{stationId}",
            newException
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the exception was created
        var getResponse = await _client.GetAsync($"/api/SchedulingExceptions/station/{stationId}");
        var exceptions = await getResponse.Content.ReadFromJsonAsync<List<StationExceptionHoursDto>>();
        Assert.NotNull(exceptions);
        Assert.Single(exceptions);
        Assert.Equal("Deep cleaning", exceptions[0].Reason);
    }

    [Fact]
    public async Task AddStationException_ReturnsBadRequest_WhenStartTimeIsAfterEndTime()
    {
        // Create a test location and station
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location Station Bad Times",
                LocationDescription = "Testing bad times",
                IconId = 1,
            }
        );

        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = locationId,
                StationName = "Station Bad Times",
                StationDescription = "Testing bad times",
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
            $"/api/SchedulingExceptions/station/{stationId}",
            badException
        );
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStationException_UpdatesExistingException()
    {
        // Create a test location and station
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Station Update",
                LocationDescription = "Testing update",
                IconId = 1,
            }
        );

        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = locationId,
                StationName = "Station For Update",
                StationDescription = "Testing update",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var exceptionId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.station_exception_hours (station_id, start_exception_datetime, end_exception_datetime, reason)
            VALUES (@StationId, @StartDateTime, @EndDateTime, @Reason)
            RETURNING id",
            new
            {
                StationId = stationId,
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
            $"/api/SchedulingExceptions/station/{exceptionId}",
            updatedException
        );
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var getResponse = await _client.GetAsync($"/api/SchedulingExceptions/station/{exceptionId}/detail");
        var exception = await getResponse.Content.ReadFromJsonAsync<StationExceptionHoursDto>();
        Assert.NotNull(exception);
        Assert.Equal("Updated reason", exception.Reason);
    }

    [Fact]
    public async Task UpdateStationException_ReturnsNotFound_WhenExceptionDoesNotExist()
    {
        var now = DateTime.UtcNow;
        var updatedException = new
        {
            StartDateTime = now.AddHours(1),
            EndDateTime = now.AddHours(2),
            Reason = "Nonexistent"
        };

        var response = await _client.PutAsJsonAsync(
            "/api/SchedulingExceptions/station/99999",
            updatedException
        );
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStationException_RemovesException()
    {
        // Create a test location and station
        var locationId = _connection.ExecuteScalar<int>(
            InsertLocationSql + " RETURNING id",
            new
            {
                LocationName = "Location For Station Delete",
                LocationDescription = "Testing delete",
                IconId = 1,
            }
        );

        var stationId = _connection.ExecuteScalar<int>(
            InsertStationSql + " RETURNING id",
            new
            {
                LocationId = locationId,
                StationName = "Station For Delete",
                StationDescription = "Testing delete",
                IconId = 1,
            }
        );

        var now = DateTime.UtcNow;
        var exceptionId = _connection.ExecuteScalar<int>(
            @"
            INSERT INTO cafeteria.station_exception_hours (station_id, start_exception_datetime, end_exception_datetime, reason)
            VALUES (@StationId, @StartDateTime, @EndDateTime, @Reason)
            RETURNING id",
            new
            {
                StationId = stationId,
                StartDateTime = now.AddHours(1),
                EndDateTime = now.AddHours(2),
                Reason = "To be deleted"
            }
        );

        var response = await _client.DeleteAsync($"/api/SchedulingExceptions/station/{exceptionId}");
        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the deletion
        var getResponse = await _client.GetAsync($"/api/SchedulingExceptions/station/{exceptionId}/detail");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteStationException_ReturnsNotFound_WhenExceptionDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/SchedulingExceptions/station/99999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
