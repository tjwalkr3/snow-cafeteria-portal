using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class FoodBuilderItemIngredientDtoOld
{
    public int Id { get; set; }

    [Required]
    public int FoodItemId { get; set; }

    [Required]
    public int IngredientId { get; set; }
}