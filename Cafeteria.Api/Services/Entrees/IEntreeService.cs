using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public interface IEntreeService
{
    Task<EntreeDto> CreateEntree(EntreeDto entreeDto);
    Task<EntreeDto?> GetEntreeByID(int id);
    Task<List<EntreeDto>> GetAllEntrees();
    Task<List<EntreeDto>> GetEntreesByStationID(int stationId);
    Task<EntreeDto?> UpdateEntreeByID(int id, EntreeDto entreeDto);
    Task<bool> DeleteEntreeByID(int id);
    Task<bool> SetInStockById(int id, bool inStock);
}
