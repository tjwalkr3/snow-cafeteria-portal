using Cafeteria.Api.Services;
using Cafeteria.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManagerController : ControllerBase
{
    private readonly IFoodOptionService _foodOptionService;
    private readonly IFoodTypeService _foodTypeService;
    private readonly IOptionOptionTypeService _optionOptionTypeService;

    public ManagerController(
        IFoodOptionService foodOptionService,
        IFoodTypeService foodTypeService,
        IOptionOptionTypeService optionOptionTypeService
    )
    {
        _foodOptionService = foodOptionService;
        _foodTypeService = foodTypeService;
        _optionOptionTypeService = optionOptionTypeService;
    }

    [HttpPost("food-options")]
    public async Task<FoodOptionDto> CreateFoodOption([FromBody] FoodOptionDto foodOptionDto)
    {
        return await _foodOptionService.CreateFoodOption(foodOptionDto);
    }

    [HttpGet("food-options/{id}")]
    public async Task<ActionResult<FoodOptionDto>> GetFoodOptionByID(int id)
    {
        var result = await _foodOptionService.GetFoodOptionByID(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("food-options")]
    public async Task<List<FoodOptionDto>> GetAllFoodOptions()
    {
        return await _foodOptionService.GetAllFoodOptions();
    }

    [HttpPut("food-options/{id}")]
    public async Task<ActionResult<FoodOptionDto>> UpdateFoodOption(
        int id,
        [FromBody] FoodOptionDto foodOptionDto
    )
    {
        var result = await _foodOptionService.UpdateFoodOption(id, foodOptionDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("food-options/{id}")]
    public async Task<IActionResult> DeleteFoodOption(int id)
    {
        var result = await _foodOptionService.DeleteFoodOption(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost("food-types")]
    public async Task<FoodOptionTypeDto> CreateFoodType([FromBody] FoodOptionTypeDto foodTypeDto)
    {
        return await _foodTypeService.CreateFoodType(foodTypeDto);
    }

    [HttpGet("food-types/{id}")]
    public async Task<ActionResult<FoodOptionTypeDto>> GetFoodTypeByID(int id)
    {
        var result = await _foodTypeService.GetFoodTypeByID(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("food-types")]
    public async Task<List<FoodOptionTypeDto>> GetAllFoodTypes()
    {
        return await _foodTypeService.GetAllFoodTypes();
    }

    [HttpPut("food-types/{id}")]
    public async Task<ActionResult<FoodOptionTypeDto>> UpdateFoodType(
        int id,
        [FromBody] FoodOptionTypeDto foodTypeDto
    )
    {
        var result = await _foodTypeService.UpdateFoodType(id, foodTypeDto);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpDelete("food-types/{id}")]
    public async Task<IActionResult> DeleteFoodType(int id)
    {
        var result = await _foodTypeService.DeleteFoodType(id);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost("option-option-types")]
    public async Task<OptionOptionTypeDto> CreateOptionOptionType(
        [FromBody] OptionOptionTypeDto optionOptionTypeDto
    )
    {
        return await _optionOptionTypeService.CreateOptionOptionType(optionOptionTypeDto);
    }

    [HttpGet("option-option-types/{id}")]
    public async Task<ActionResult<OptionOptionTypeDto>> GetOptionOptionTypeByID(int id)
    {
        var result = await _optionOptionTypeService.GetOptionOptionTypeByID(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("option-option-types")]
    public async Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes()
    {
        return await _optionOptionTypeService.GetAllOptionOptionTypes();
    }

    [HttpPut("option-option-types/{id}")]
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

    [HttpDelete("option-option-types/{id}")]
    public async Task<IActionResult> DeleteOptionOptionType(int id)
    {
        var result = await _optionOptionTypeService.DeleteOptionOptionTypeById(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
