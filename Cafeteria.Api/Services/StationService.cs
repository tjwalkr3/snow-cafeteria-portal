using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;
using Dapper;

namespace Cafeteria.Api.Services;

public class StationService : IStationService
{
    private readonly IDbConnection _dbConnection;

    public StationService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<StationDto>> GetAllStations()
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_id AS LocationId,
                station_name AS StationName,
                station_description AS StationDescription
            FROM cafeteria.station
            ORDER BY station_name;";

        var stations = await _dbConnection.QueryAsync<StationDto>(sql);
        return stations.ToList();
    }

    public async Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_id AS LocationId,
                station_name AS StationName,
                station_description AS StationDescription
            FROM cafeteria.station
            WHERE location_id = @location_id
            ORDER BY station_name;";

        var stations = await _dbConnection.QueryAsync<StationDto>(sql, new { location_id = locationId });
        return stations.ToList();
    }

    public async Task<StationDto?> GetStationByID(int stationId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_id AS LocationId,
                station_name AS StationName,
                station_description AS StationDescription
            FROM cafeteria.station
            WHERE id = @id;";

        var station = await _dbConnection.QuerySingleOrDefaultAsync<StationDto>(sql, new { id = stationId });
        return station;
    }

    public async Task CreateStationForLocation(int locationId, StationDto station)
    {
        const string sql = @"
            INSERT INTO cafeteria.station (location_id, station_name, station_description)
            VALUES (@location_id, @station_name, @station_description);";

        var parameters = new
        {
            location_id = locationId,
            station_name = station.StationName,
            station_description = station.StationDescription ?? string.Empty
        };

        await _dbConnection.ExecuteAsync(sql, parameters);
    }

    public async Task UpdateStationByID(int stationId, StationDto station)
    {
        const string sql = @"
            UPDATE cafeteria.station
            SET station_name = @station_name,
                station_description = @station_description
            WHERE id = @id;";

        var parameters = new
        {
            id = stationId,
            station_name = station.StationName,
            station_description = station.StationDescription ?? string.Empty
        };

        await _dbConnection.ExecuteAsync(sql, parameters);
    }

    public async Task<bool> DeleteStationByID(int id)
    {
        const string sql = @"
            DELETE FROM cafeteria.station
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id });
        return rowsAffected > 0;
    }

    public async Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                station_id AS StationId,
                weekday_id AS WeekdayId,
                open_time AS OpenTime,
                close_time AS CloseTime
            FROM cafeteria.station_business_hours
            WHERE station_id = @station_id
            ORDER BY weekday_id, open_time;";

        var hours = await _dbConnection.QueryAsync<StationBusinessHoursDbModel>(sql, new { station_id = stationId });
        return hours.Select(h => new StationBusinessHoursDto
        {
            Id = h.Id,
            StationId = h.StationId,
            WeekdayId = h.WeekdayId,
            OpenTime = TimeOnly.FromTimeSpan(h.OpenTime),
            CloseTime = TimeOnly.FromTimeSpan(h.CloseTime)
        }).ToList();
    }

    public async Task<StationBusinessHoursDto?> GetStationBusinessHoursById(int stationHrsId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                station_id AS StationId,
                weekday_id AS WeekdayId,
                open_time AS OpenTime,
                close_time AS CloseTime
            FROM cafeteria.station_business_hours
            WHERE id = @id;";

        var h = await _dbConnection.QuerySingleOrDefaultAsync<StationBusinessHoursDbModel>(sql, new { id = stationHrsId });

        if (h is null)
        {
            return null;
        }

        return new StationBusinessHoursDto
        {
            Id = h.Id,
            StationId = h.StationId,
            WeekdayId = h.WeekdayId,
            OpenTime = TimeOnly.FromTimeSpan(h.OpenTime),
            CloseTime = TimeOnly.FromTimeSpan(h.CloseTime)
        };
    }

    private class StationBusinessHoursDbModel
    {
        public int Id { get; set; }
        public int StationId { get; set; }
        public int WeekdayId { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
    }

    public async Task AddStationHours(int stationId, DateTime startTime, DateTime endTime, WeekDay weekday)
    {
        const string sql = @"
            INSERT INTO cafeteria.station_business_hours (station_id, weekday_id, open_time, close_time)
            VALUES (@station_id, @weekday_id, @open_time, @close_time);";

        var parameters = new
        {
            station_id = stationId,
            weekday_id = (int)weekday,
            open_time = startTime.TimeOfDay,
            close_time = endTime.TimeOfDay
        };

        await _dbConnection.ExecuteAsync(sql, parameters);
    }

    public async Task UpdateStationHoursById(int stationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday)
    {
        const string sql = @"
            UPDATE cafeteria.station_business_hours
            SET weekday_id = @weekday_id,
                open_time = @open_time,
                close_time = @close_time
            WHERE id = @id;";

        var parameters = new
        {
            id = stationHrsId,
            weekday_id = (int)weekday,
            open_time = startTime.TimeOfDay,
            close_time = endTime.TimeOfDay
        };

        await _dbConnection.ExecuteAsync(sql, parameters);
    }

    public async Task<bool> DeleteStationHrsById(int stationHrsId)
    {
        const string sql = @"
            DELETE FROM cafeteria.station_business_hours
            WHERE id = @id;";

        var rowsAffected = await _dbConnection.ExecuteAsync(sql, new { id = stationHrsId });
        return rowsAffected > 0;
    }
}
