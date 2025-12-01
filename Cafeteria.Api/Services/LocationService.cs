using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Services;

public class LocationService : ILocationService
{
    private readonly IDbConnection _dbConnection;

    public LocationService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public Task<List<LocationDto>> GetAllLocations()
    {
        throw new NotImplementedException();
    }

    public Task<LocationDto> GetLocationByID(int locationId)
    {
        throw new NotImplementedException();
    }

    public Task CreateLocation(string name, string? description = null)
    {
        throw new NotImplementedException();
    }

    public Task UpdateLocationByID(int locationId, string name, string? description)
    {
        throw new NotImplementedException();
    }

    public Task DeleteLocationByID(int locationId)
    {
        throw new NotImplementedException();
    }

    public Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId)
    {
        throw new NotImplementedException();
    }

    public Task<LocationBusinessHoursDto> GetLocationBusinessHoursById(int locationHrsId)
    {
        throw new NotImplementedException();
    }

    public Task AddLocationHours(int locationId, DateTime startTime, DateTime endTime, WeekDay weekday)
    {
        throw new NotImplementedException();
    }

    public Task UpdateLocationHoursById(int locationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday)
    {
        throw new NotImplementedException();
    }

    public Task DeleteLocationHrsById(int locationHrsId)
    {
        throw new NotImplementedException();
    }
}
