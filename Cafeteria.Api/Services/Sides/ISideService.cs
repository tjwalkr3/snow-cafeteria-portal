using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.Sides;

public interface ISideService
{
    Task<SideDto> CreateSide(SideDto sideDto);
    Task<SideDto?> GetSideById(int id);
    Task<List<SideDto>> GetAllSides();
    Task<List<SideDto>> GetSidesByStationId(int stationId);
    Task<SideDto?> UpdateSideById(int id, SideDto sideDto);
    Task<bool> DeleteSideById(int id);
    Task<bool> SetStockStatusById(int id, bool inStock);
}
