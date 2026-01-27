using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs.Menu;

public class OptionOptionTypeDto
{
    public int Id { get; set; }

    [Required]
    public int FoodOptionId { get; set; }

    [Required]
    public int FoodOptionTypeId { get; set; }
}