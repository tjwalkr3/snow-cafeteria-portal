using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs2;

public class LocationDto
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string LocationName { get; set; } = string.Empty;
    [Required]
    [StringLength(200)]
    public string LocationDescription { get; set; } = string.Empty;
    [StringLength(500)]
    public string? ImageUrl { get; set; }
}