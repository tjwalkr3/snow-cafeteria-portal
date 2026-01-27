using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs.Menu;

public class StationBusinessHoursDto
{
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }

    [Required]
    public int WeekdayId { get; set; }

    [Required]
    public TimeOnly OpenTime { get; set; }

    [Required]
    public TimeOnly CloseTime { get; set; }
}
