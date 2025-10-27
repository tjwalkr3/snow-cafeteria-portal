using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOsOld;

public class LocationBusinessHoursDtoOld
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