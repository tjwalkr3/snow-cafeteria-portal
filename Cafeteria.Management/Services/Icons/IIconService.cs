using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Services.Icons;

public interface IIconService
{
    Task<List<IconDto>> GetAllIcons();
}
