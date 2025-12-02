using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface ISideService
{
    Task<SideDto> CreateSide(SideDto sideDto);
    Task<SideDto?> GetSideByID(int id);
    Task<List<SideDto>> GetAllSides();
    Task<List<SideDto>> GetSidesByStationID(int stationId);
    Task<SideDto?> UpdateSideByID(int id, SideDto sideDto);
    Task<bool> DeleteSideByID(int id);
}
