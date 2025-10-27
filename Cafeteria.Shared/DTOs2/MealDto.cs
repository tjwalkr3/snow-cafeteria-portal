using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs2;

public class MealDto
{
    public int Id { get; set; }

    [Required]
    public int EntreeId { get; set; }

    public int? SideId { get; set; }

    [Required]
    public int DrinkId { get; set; }
}
