using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Api.Services.Icons;

public interface IIconService
{
    Task<List<IconDto>> GetAllIcons();
}
