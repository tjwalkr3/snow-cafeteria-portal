using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class StationDto
{
    public int Id { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    [StringLength(100)]
    public string StationName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string StationDescription { get; set; } = string.Empty;
}