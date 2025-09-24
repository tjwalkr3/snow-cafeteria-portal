using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class FoodBuilderItemIngredientDto
{
    public int Id { get; set; }

    [Required]
    public int FoodItemId { get; set; }

    [Required]
    public int IngredientId { get; set; }
}