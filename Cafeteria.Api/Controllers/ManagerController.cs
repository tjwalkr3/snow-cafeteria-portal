using Microsoft.AspNetCore.Mvc;
using Cafeteria.Shared.DTOs;
using Cafeteria.Api.Services;

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
        IOptionOptionTypeService optionOptionTypeService)
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
    public async Task<FoodOptionDto?> GetFoodOptionByID(int id)
    {
        return await _foodOptionService.GetFoodOptionByID(id);
    }

    [HttpGet("food-options")]
    public async Task<List<FoodOptionDto>> GetAllFoodOptions()
    {
        return await _foodOptionService.GetAllFoodOptions();
    }

    [HttpPut("food-options/{id}")]
    public async Task<FoodOptionDto?> UpdateFoodOption(int id, [FromBody] FoodOptionDto foodOptionDto)
    {
        return await _foodOptionService.UpdateFoodOption(id, foodOptionDto);
    }

    [HttpDelete("food-options/{id}")]
    public async Task<bool> DeleteFoodOption(int id)
    {
        return await _foodOptionService.DeleteFoodOption(id);
    }

    [HttpPost("food-types")]
    public async Task<FoodOptionTypeDto> CreateFoodType([FromBody] FoodOptionTypeDto foodTypeDto)
    {
        return await _foodTypeService.CreateFoodType(foodTypeDto);
    }

    [HttpGet("food-types/{id}")]
    public async Task<FoodOptionTypeDto?> GetFoodTypeByID(int id)
    {
        return await _foodTypeService.GetFoodTypeByID(id);
    }

    [HttpGet("food-types")]
    public async Task<List<FoodOptionTypeDto>> GetAllFoodTypes()
    {
        return await _foodTypeService.GetAllFoodTypes();
    }

    [HttpPut("food-types/{id}")]
    public async Task<FoodOptionTypeDto?> UpdateFoodType(int id, [FromBody] FoodOptionTypeDto foodTypeDto)
    {
        return await _foodTypeService.UpdateFoodType(id, foodTypeDto);
    }

    [HttpDelete("food-types/{id}")]
    public async Task<bool> DeleteFoodType(int id)
    {
        return await _foodTypeService.DeleteFoodType(id);
    }

    [HttpPost("option-option-types")]
    public async Task<OptionOptionTypeDto> CreateOptionOptionType([FromBody] OptionOptionTypeDto optionOptionTypeDto)
    {
        return await _optionOptionTypeService.CreateOptionOptionType(optionOptionTypeDto);
    }

    [HttpGet("option-option-types/{id}")]
    public async Task<OptionOptionTypeDto?> GetOptionOptionTypeByID(int id)
    {
        return await _optionOptionTypeService.GetOptionOptionTypeByID(id);
    }

    [HttpGet("option-option-types")]
    public async Task<List<OptionOptionTypeDto>> GetAllOptionOptionTypes()
    {
        return await _optionOptionTypeService.GetAllOptionOptionTypes();
    }

    [HttpPut("option-option-types/{id}")]
    public async Task<OptionOptionTypeDto?> UpdateOptionOptionType(int id, [FromBody] OptionOptionTypeDto optionOptionTypeDto)
    {
        return await _optionOptionTypeService.UpdateOptionOptionTypeById(id, optionOptionTypeDto);
    }

    [HttpDelete("option-option-types/{id}")]
    public async Task<bool> DeleteOptionOptionType(int id)
    {
        return await _optionOptionTypeService.DeleteOptionOptionTypeById(id);
    }
}
