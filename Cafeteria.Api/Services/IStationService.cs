using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cafeteria.Shared.DTOs;
using Cafeteria.Shared.Enums;

namespace Cafeteria.Api.Services;

public interface IStationService
{
    Task<List<StationDto>> GetAllStations();
    Task<List<StationDto>> GetStationsByLocation(int locationId);
    Task<StationDto?> GetStationByID(int stationId);
    Task CreateStationForLocation(int locationId, StationDto station);
    Task UpdateStationByID(int stationId, StationDto station);
    Task<bool> DeleteStationByID(int id);
    Task<List<StationBusinessHoursDto>> GetStationBusinessHours(int stationId);
    Task<StationBusinessHoursDto?> GetStationBusinessHoursById(int stationHrsId);
    Task AddStationHours(int stationId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task UpdateStationHoursById(int stationHrsId, DateTime startTime, DateTime endTime, WeekDay weekday);
    Task<bool> DeleteStationHrsById(int stationHrsId);
}
