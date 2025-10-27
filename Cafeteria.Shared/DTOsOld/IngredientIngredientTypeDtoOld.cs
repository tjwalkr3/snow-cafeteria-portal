using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOsOld;

public class IngredientIngredientTypeDtoOld
{
    public int Id { get; set; }

    [Required]
    public int IngredientTypeId { get; set; }

    [Required]
    public int IngredientId { get; set; }
}