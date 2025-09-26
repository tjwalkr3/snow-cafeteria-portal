using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class CafeteriaLocationDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string LocationName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string LocationDescription { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string LocationAddress { get; set; } = string.Empty;
}