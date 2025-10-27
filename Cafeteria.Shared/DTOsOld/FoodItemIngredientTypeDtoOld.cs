using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOsOld;

public class FoodItemIngredientTypeDtoOld
{
    public int Id { get; set; }

    [Required]
    public int FoodItemId { get; set; }

    [Required]
    public int IngredientTypeId { get; set; }
}