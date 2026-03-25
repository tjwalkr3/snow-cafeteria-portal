using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Services.Locations;

public interface ILocationService
{
    Task<List<LocationDto>> GetAllLocations();
    Task<LocationDto?> GetLocationByID(int locationId);
    Task CreateLocation(string name, string? description = null, int? iconId = null, string? printerUrl = null);
    Task UpdateLocationById(int locationId, string name, string? description, int? iconId = null, string? printerUrl = null);
    Task DeleteLocationById(int locationId);
    Task<List<LocationBusinessHoursDto>> GetLocationBusinessHoursByLocationId(int locationId);
    Task<LocationBusinessHoursDto?> GetLocationBusinessHoursById(int locationHrsId);
    Task AddLocationHoursByLocationId(int locationId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task UpdateLocationHoursById(int locationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task DeleteLocationHoursById(int locationHrsId);
}
