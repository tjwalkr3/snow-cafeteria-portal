using Cafeteria.Shared.DTOs;

namespace Cafeteria.Management.Services;

public interface ISideService
{
    Task<List<SideDto>> GetAllSides();
    Task<bool> DeleteSide(int id);
}
