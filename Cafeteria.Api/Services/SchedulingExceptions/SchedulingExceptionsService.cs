using System.Data;
using Cafeteria.Shared.DTOs.Menu;
using Dapper;

namespace Cafeteria.Api.Services.SchedulingExceptions;

public class SchedulingExceptionsService : ISchedulingExceptionsService
{
    private readonly IDbConnection _dbConnection;

    public SchedulingExceptionsService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    // Location exception methods
    public async Task<List<LocationExceptionHoursDto>> GetLocationExceptionsByLocationId(int locationId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_id AS LocationId,
                start_exception_datetime AS StartExceptionDateTime,
                end_exception_datetime AS EndExceptionDateTime
            FROM cafeteria.location_exception_hours
            WHERE location_id = @location_id
            ORDER BY start_exception_datetime DESC;";

        var exceptions = await _dbConnection.QueryAsync<LocationExceptionHoursDto>(sql, new { location_id = locationId });
        return exceptions.ToList();
    }

    public async Task<LocationExceptionHoursDto?> GetLocationExceptionById(int exceptionId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_id AS LocationId,
                start_exception_datetime AS StartExceptionDateTime,
                end_exception_datetime AS EndExceptionDateTime
            FROM cafeteria.location_exception_hours
            WHERE id = @id;";

        var exception = await _dbConnection.QuerySingleOrDefaultAsync<LocationExceptionHoursDto>(sql, new { id = exceptionId });
        return exception;
    }

    public async Task AddLocationException(int locationId, DateTime startDateTime, DateTime endDateTime)
    {
        const string sql = @"
            INSERT INTO cafeteria.location_exception_hours (location_id, start_exception_datetime, end_exception_datetime)
            VALUES (@location_id, @start_exception_datetime, @end_exception_datetime);";

        await _dbConnection.ExecuteAsync(sql, new
        {
            location_id = locationId,
            start_exception_datetime = startDateTime,
            end_exception_datetime = endDateTime
        });
    }

    public async Task UpdateLocationException(int exceptionId, DateTime startDateTime, DateTime endDateTime)
    {
        const string sql = @"
            UPDATE cafeteria.location_exception_hours
            SET start_exception_datetime = @start_exception_datetime,
                end_exception_datetime = @end_exception_datetime
            WHERE id = @id;";

        await _dbConnection.ExecuteAsync(sql, new
        {
            id = exceptionId,
            start_exception_datetime = startDateTime,
            end_exception_datetime = endDateTime
        });
    }

    public async Task DeleteLocationException(int exceptionId)
    {
        const string sql = @"
            DELETE FROM cafeteria.location_exception_hours
            WHERE id = @id;";

        await _dbConnection.ExecuteAsync(sql, new { id = exceptionId });
    }

    // Station exception methods
    public async Task<List<StationExceptionHoursDto>> GetStationExceptionsByStationId(int stationId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                station_id AS StationId,
                start_exception_datetime AS StartExceptionDateTime,
                end_exception_datetime AS EndExceptionDateTime
            FROM cafeteria.station_exception_hours
            WHERE station_id = @station_id
            ORDER BY start_exception_datetime DESC;";

        var exceptions = await _dbConnection.QueryAsync<StationExceptionHoursDto>(sql, new { station_id = stationId });
        return exceptions.ToList();
    }

    public async Task<StationExceptionHoursDto?> GetStationExceptionById(int exceptionId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                station_id AS StationId,
                start_exception_datetime AS StartExceptionDateTime,
                end_exception_datetime AS EndExceptionDateTime
            FROM cafeteria.station_exception_hours
            WHERE id = @id;";

        var exception = await _dbConnection.QuerySingleOrDefaultAsync<StationExceptionHoursDto>(sql, new { id = exceptionId });
        return exception;
    }

    public async Task AddStationException(int stationId, DateTime startDateTime, DateTime endDateTime)
    {
        const string sql = @"
            INSERT INTO cafeteria.station_exception_hours (station_id, start_exception_datetime, end_exception_datetime)
            VALUES (@station_id, @start_exception_datetime, @end_exception_datetime);";

        await _dbConnection.ExecuteAsync(sql, new
        {
            station_id = stationId,
            start_exception_datetime = startDateTime,
            end_exception_datetime = endDateTime
        });
    }

    public async Task UpdateStationException(int exceptionId, DateTime startDateTime, DateTime endDateTime)
    {
        const string sql = @"
            UPDATE cafeteria.station_exception_hours
            SET start_exception_datetime = @start_exception_datetime,
                end_exception_datetime = @end_exception_datetime
            WHERE id = @id;";

        await _dbConnection.ExecuteAsync(sql, new
        {
            id = exceptionId,
            start_exception_datetime = startDateTime,
            end_exception_datetime = endDateTime
        });
    }

    public async Task DeleteStationException(int exceptionId)
    {
        const string sql = @"
            DELETE FROM cafeteria.station_exception_hours
            WHERE id = @id;";

        await _dbConnection.ExecuteAsync(sql, new { id = exceptionId });
    }
}
