using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Services;

public interface IStationService
{
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<StationDto> GetStationByID(int stationId);
    Task CreateStationForLocation(int locationId, string stationName, string? stationDescription = null);
    Task UpdateStationByID(int stationId, string name, string? description);
    Task DeleteStationByID(int id);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
    Task<StationBusinessHoursDto> GetStationBusinessHoursById(int stationHrsId);
    Task AddStationHours(int stationId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task UpdateStationHoursById(int stationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task DeleteStationHrsById(int stationHrsId);
}
