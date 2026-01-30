using Cafeteria.Api.Services.OptionOptionTypes;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptionOptionTypeController(IOptionOptionTypeService optionOptionTypeService) : ControllerBase
{
    private readonly IOptionOptionTypeService _optionOptionTypeService = optionOptionTypeService;

    [HttpGet("{id}")]
    public async Task<ActionResult<OptionOptionTypeDto>> GetOptionOptionTypeById(int id)
    {
        var result = await _optionOptionTypeService.GetOptionOptionTypeById(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes()
    {
        return await _optionOptionTypeService.GetAllOptionOptionTypes();
    }

    [Authorize]
    [HttpPost]
    public async Task<OptionOptionTypeDto> CreateOptionOptionType(
        [FromBody] OptionOptionTypeDto optionOptionTypeDto
    )
    {
        return await _optionOptionTypeService.CreateOptionOptionType(optionOptionTypeDto);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<OptionOptionTypeDto>> UpdateOptionOptionTypeById(
        int id,
        [FromBody] OptionOptionTypeDto optionOptionTypeDto
    )
    {
        var result = await _optionOptionTypeService.UpdateOptionOptionTypeById(
            id,
            optionOptionTypeDto
        );
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOptionOptionTypeById(int id)
    {
        var result = await _optionOptionTypeService.DeleteOptionOptionTypeById(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
