using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOsOld;

public class StationDtoOld
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