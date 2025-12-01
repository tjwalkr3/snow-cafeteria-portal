using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Services;

public class StationService : IStationService
{
    private readonly IDbConnection _dbConnection;

    public StationService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public Task<List<StationDto>> GetStationsByLocation(int locationId)
    {
        throw new NotImplementedException();
    }

    public Task<StationDto> GetStationByID(int stationId)
    {
        throw new NotImplementedException();
    }

    public Task CreateStationForLocation(int locationId, string stationName, string? stationDescription = null)
    {
        throw new NotImplementedException();
    }

    public Task UpdateStationByID(int stationId, string name, string? description)
    {
        throw new NotImplementedException();
    }

    public Task DeleteStationByID(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId)
    {
        throw new NotImplementedException();
    }

    public Task<StationBusinessHoursDto> GetStationBusinessHoursById(int stationHrsId)
    {
        throw new NotImplementedException();
    }

    public Task AddStationHours(int stationId, DateTime startTime, DateTime endTime, WeekDay weekday)
    {
        throw new NotImplementedException();
    }

    public Task UpdateStationHoursById(int stationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday)
    {
        throw new NotImplementedException();
    }

    public Task DeleteStationHrsById(int stationHrsId)
    {
        throw new NotImplementedException();
    }
}
