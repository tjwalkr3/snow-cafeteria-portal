using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.Entrees;

public interface IEntreeService
{
    Task<List<EntreeDto>> GetAllEntrees();
    Task<EntreeDto?> GetEntreeById(int id);
    Task<EntreeDto> CreateEntree(EntreeDto entreeDto);
    Task<EntreeDto?> UpdateEntreeById(int id, EntreeDto entreeDto);
    Task<bool> DeleteEntreeById(int id);
    Task<bool> SetStockStatusById(int id, bool inStock);
}
