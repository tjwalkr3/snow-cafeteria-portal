using Cafeteria.Api.Services.OptionOptionTypes;
using Cafeteria.Shared.DTOs.Menu;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptionOptionTypeController : ControllerBase
{
    private readonly IOptionOptionTypeService _optionOptionTypeService;

    public OptionOptionTypeController(IOptionOptionTypeService optionOptionTypeService)
    {
        _optionOptionTypeService = optionOptionTypeService;
    }

    [HttpPost]
    public async Task<OptionOptionTypeDto> CreateOptionOptionType(
        [FromBody] OptionOptionTypeDto optionOptionTypeDto
    )
    {
        return await _optionOptionTypeService.CreateOptionOptionType(optionOptionTypeDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OptionOptionTypeDto>> GetOptionOptionTypeByID(int id)
    {
        var result = await _optionOptionTypeService.GetOptionOptionTypeByID(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes()
    {
        return await _optionOptionTypeService.GetAllOptionOptionTypes();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OptionOptionTypeDto>> UpdateOptionOptionType(
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOptionOptionType(int id)
    {
        var result = await _optionOptionTypeService.DeleteOptionOptionTypeById(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
