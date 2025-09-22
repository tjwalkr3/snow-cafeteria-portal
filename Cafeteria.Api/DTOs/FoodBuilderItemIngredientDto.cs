using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Api.DTOs;

public class FoodBuilderItemIngredientDto
{
    public int Id { get; set; }

    [Required]
    public int FoodItemId { get; set; }

    [Required]
    public int IngredientId { get; set; }
}