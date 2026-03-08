using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs.Menu;

public class LocationExceptionHoursDto
{
    public int Id { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public DateTime StartExceptionDateTime { get; set; }

    [Required]
    public DateTime EndExceptionDateTime { get; set; }
}
