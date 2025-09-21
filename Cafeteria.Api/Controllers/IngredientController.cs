using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredients()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IngredientDto>> GetIngredient(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<ActionResult<IngredientDto>> CreateIngredient(IngredientDto ingredientDto)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIngredient(int id, IngredientDto ingredientDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<IngredientTypeDto>>> GetIngredientTypes()
    {
        throw new NotImplementedException();
    }

    [HttpGet("types/{id}")]
    public async Task<ActionResult<IngredientTypeDto>> GetIngredientType(int id)
    {
        throw new NotImplementedException();
    }
}