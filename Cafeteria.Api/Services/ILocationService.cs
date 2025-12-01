using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Services;

public interface ILocationService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<LocationDto> GetLocationByID(int locationId);
    Task CreateLocation(string name, string? description = null);
    Task UpdateLocationByID(int locationId, string name, string? description);
    Task DeleteLocationByID(int locationId);
    Task<List<LocationBusinessHoursDto>> GetLocationBusinessHours(int locationId);
    Task<LocationBusinessHoursDto> GetLocationBusinessHoursById(int locationHrsId);
    Task AddLocationHours(int locationId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task UpdateLocationHoursById(int locationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task DeleteLocationHrsById(int locationHrsId);
}