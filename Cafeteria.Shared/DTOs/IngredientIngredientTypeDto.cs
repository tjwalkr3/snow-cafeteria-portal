using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class IngredientIngredientTypeDto
{
    public int Id { get; set; }

    [Required]
    public int IngredientTypeId { get; set; }

    [Required]
    public int IngredientId { get; set; }
}