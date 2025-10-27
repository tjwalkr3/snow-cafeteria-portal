using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs2;

public class LocationBusinessHoursDto
{
    public int Id { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public int WeekdayId { get; set; }

    [Required]
    public TimeOnly OpenTime { get; set; }

    [Required]
    public TimeOnly CloseTime { get; set; }
}