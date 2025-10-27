using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class OptionOptionTypeDto
{
    public int Id { get; set; }

    [Required]
    public int FoodOptionId { get; set; }

    [Required]
    public int FoodOptionTypeId { get; set; }
}