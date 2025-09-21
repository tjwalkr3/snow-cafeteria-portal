using Microsoft.AspNetCore.Mvc;
using Cafeteria.Api.DTOs;

namespace Cafeteria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<IngredientDto>> GetIngredients()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public ActionResult<IngredientDto> GetIngredient(int id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public ActionResult<IngredientDto> CreateIngredient(IngredientDto ingredientDto)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateIngredient(int id, IngredientDto ingredientDto)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteIngredient(int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("types")]
    public ActionResult<IEnumerable<IngredientTypeDto>> GetIngredientTypes()
    {
        throw new NotImplementedException();
    }

    [HttpGet("types/{id}")]
    public ActionResult<IngredientTypeDto> GetIngredientType(int id)
    {
        throw new NotImplementedException();
    }
}