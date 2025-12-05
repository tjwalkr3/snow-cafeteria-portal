using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface IEntreeService
{
    Task<List<EntreeDto>> GetAllEntrees();
    Task<EntreeDto?> GetEntreeById(int id);
    Task<EntreeDto> CreateEntree(EntreeDto entreeDto);
    Task<EntreeDto?> UpdateEntreeById(int id, EntreeDto entreeDto);
    Task<bool> DeleteEntreeById(int id);
}
