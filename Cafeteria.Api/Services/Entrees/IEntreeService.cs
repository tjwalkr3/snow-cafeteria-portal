using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.Entrees;

public interface IEntreeService
{
    Task<EntreeDto> CreateEntree(EntreeDto entreeDto);
    Task<EntreeDto?> GetEntreeById(int id);
    Task<List<EntreeDto>> GetAllEntrees();
    Task<List<EntreeDto>> GetEntreesByStationId(int stationId);
    Task<EntreeDto?> UpdateEntreeById(int id, EntreeDto entreeDto);
    Task<bool> DeleteEntreeById(int id);
    Task<bool> SetStockStatusById(int id, bool inStock);
}
