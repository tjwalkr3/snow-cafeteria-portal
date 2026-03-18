using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.Services.Auth;

namespace Cafeteria.Management.Services.Icons;

public class IconService(IHttpClientAuth client) : IIconService
{
    public async Task<List<IconDto>> GetAllIcons()
    {
        return await client.GetAsync<List<IconDto>>("icon") ?? [];
    }
}
