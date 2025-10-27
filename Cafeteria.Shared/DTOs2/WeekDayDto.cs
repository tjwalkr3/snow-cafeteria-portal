using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs2;

public class WeekDayDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string WeekdayName { get; set; } = string.Empty;
}