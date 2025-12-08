using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface ISideService
{
    Task<List<SideDto>> GetAllSides();
    Task<SideDto?> CreateSide(SideDto side);
    Task<SideDto?> UpdateSide(SideDto side);
    Task<bool> DeleteSide(int id);
    Task<bool> SetInStockById(int id, bool inStock);
}
