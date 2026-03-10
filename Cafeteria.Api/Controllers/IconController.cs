using Cafeteria.Api.Services.Icons;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/icon")]
public class IconController(IIconService iconService) : ControllerBase
{
    [HttpGet]
    public async Task<List<IconDto>> GetAllIcons()
    {
        return await iconService.GetAllIcons();
    }
}
