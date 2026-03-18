using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs.Menu;

public class StationExceptionHoursDto
{
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }

    [Required]
    public DateTime StartExceptionDateTime { get; set; }

    [Required]
    public DateTime EndExceptionDateTime { get; set; }

    public string? Reason { get; set; }
}
