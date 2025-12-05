using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;
using Dapper;

namespace Cafeteria.Api.Services;

public class LocationService : ILocationService
{
    private readonly IDbConnection _dbConnection;

    public LocationService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<LocationDto>> GetAllLocations()
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_name AS LocationName,
                location_description AS LocationDescription,
                image_url AS ImageUrl
            FROM cafeteria.cafeteria_location
            ORDER BY location_name;";

        var locations = await _dbConnection.QueryAsync<LocationDto>(sql);
        return locations.ToList();
    }

    public async Task<LocationDto?> GetLocationByID(int locationId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_name AS LocationName,
                location_description AS LocationDescription,
                image_url AS ImageUrl
            FROM cafeteria.cafeteria_location
            WHERE id = @id;";

        var location = await _dbConnection.QuerySingleOrDefaultAsync<LocationDto>(sql, new { id = locationId });
        return location;
    }

    public async Task CreateLocation(string name, string? description = null)
    {
        const string sql = @"
            INSERT INTO cafeteria.cafeteria_location (location_name, location_description)
            VALUES (@location_name, @location_description);";

        var parameters = new
        {
            location_name = name,
            location_description = description ?? string.Empty
        };

        await _dbConnection.ExecuteAsync(sql, parameters);
    }

    public async Task UpdateLocationByID(int locationId, string name, string? description)
    {
        const string sql = @"
            UPDATE cafeteria.cafeteria_location
            SET location_name = @location_name,
                location_description = @location_description
            WHERE id = @id;";

        var parameters = new
        {
            id = locationId,
            location_name = name,
            location_description = description ?? string.Empty
        };

        await _dbConnection.ExecuteAsync(sql, parameters);
    }

    public async Task DeleteLocationByID(int locationId)
    {
        const string sql = @"
            -- Delete meals that reference items in stations in this location
            DELETE FROM cafeteria.meal WHERE entree_id IN (SELECT id FROM cafeteria.entree WHERE station_id IN (SELECT id FROM cafeteria.station WHERE location_id = @id));
            DELETE FROM cafeteria.meal WHERE side_id IN (SELECT id FROM cafeteria.side WHERE station_id IN (SELECT id FROM cafeteria.station WHERE location_id = @id));
            DELETE FROM cafeteria.meal WHERE drink_id IN (SELECT id FROM cafeteria.drink WHERE station_id IN (SELECT id FROM cafeteria.station WHERE location_id = @id));

            -- Delete items in stations
            DELETE FROM cafeteria.entree WHERE station_id IN (SELECT id FROM cafeteria.station WHERE location_id = @id);
            DELETE FROM cafeteria.side WHERE station_id IN (SELECT id FROM cafeteria.station WHERE location_id = @id);
            DELETE FROM cafeteria.drink WHERE station_id IN (SELECT id FROM cafeteria.station WHERE location_id = @id);

            -- Delete station hours
            DELETE FROM cafeteria.station_business_hours WHERE station_id IN (SELECT id FROM cafeteria.station WHERE location_id = @id);

            -- Delete stations
            DELETE FROM cafeteria.station WHERE location_id = @id;

            -- Delete location hours
            DELETE FROM cafeteria.location_business_hours WHERE location_id = @id;

            -- Delete location
            DELETE FROM cafeteria.cafeteria_location
            WHERE id = @id;";

        await _dbConnection.ExecuteAsync(sql, new { id = locationId });
    }

    public async Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_id AS LocationId,
                weekday_id AS WeekdayId,
                open_time AS OpenTime,
                close_time AS CloseTime
            FROM cafeteria.location_business_hours
            WHERE location_id = @location_id
            ORDER BY weekday_id, open_time;";

        var hours = await _dbConnection.QueryAsync<LocationBusinessHoursDbModel>(sql, new { location_id = locationId });
        return hours.Select(h => new LocationBusinessHoursDto
        {
            Id = h.Id,
            LocationId = h.LocationId,
            WeekdayId = h.WeekdayId,
            OpenTime = TimeOnly.FromTimeSpan(h.OpenTime),
            CloseTime = TimeOnly.FromTimeSpan(h.CloseTime)
        }).ToList();
    }

    public async Task<LocationBusinessHoursDto?> GetLocationBusinessHoursById(int locationHrsId)
    {
        const string sql = @"
            SELECT
                id AS Id,
                location_id AS LocationId,
                weekday_id AS WeekdayId,
                open_time AS OpenTime,
                close_time AS CloseTime
            FROM cafeteria.location_business_hours
            WHERE id = @id;";

        var h = await _dbConnection.QuerySingleOrDefaultAsync<LocationBusinessHoursDbModel>(sql, new { id = locationHrsId });

        if (h is null)
        {
            return null;
        }

        return new LocationBusinessHoursDto
        {
            Id = h.Id,
            LocationId = h.LocationId,
            WeekdayId = h.WeekdayId,
            OpenTime = TimeOnly.FromTimeSpan(h.OpenTime),
            CloseTime = TimeOnly.FromTimeSpan(h.CloseTime)
        };
    }

    private class LocationBusinessHoursDbModel
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int WeekdayId { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
    }

    public async Task AddLocationHours(int locationId, DateTime startTime, DateTime endTime, WeekDay weekday)
    {
        const string sql = @"
            INSERT INTO cafeteria.location_business_hours (location_id, weekday_id, open_time, close_time)
            VALUES (@location_id, @weekday_id, @open_time, @close_time);";

        var parameters = new
        {
            location_id = locationId,
            weekday_id = (int)weekday,
            open_time = startTime.TimeOfDay,
            close_time = endTime.TimeOfDay
        };

        await _dbConnection.ExecuteAsync(sql, parameters);
    }

    public async Task UpdateLocationHoursById(int locationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday)
    {
        const string sql = @"
            UPDATE cafeteria.location_business_hours
            SET weekday_id = @weekday_id,
                open_time = @open_time,
                close_time = @close_time
            WHERE id = @id;";

        var parameters = new
        {
            id = locationHrsId,
            weekday_id = (int)weekday,
            open_time = startTime.TimeOfDay,
            close_time = endTime.TimeOfDay
        };

        await _dbConnection.ExecuteAsync(sql, parameters);
    }

    public async Task DeleteLocationHrsById(int locationHrsId)
    {
        const string sql = @"
            DELETE FROM cafeteria.location_business_hours
            WHERE id = @id;";

        await _dbConnection.ExecuteAsync(sql, new { id = locationHrsId });
    }
}
