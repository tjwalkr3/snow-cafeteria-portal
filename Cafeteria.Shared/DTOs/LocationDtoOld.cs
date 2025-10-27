using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class LocationDtoOld
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;
}