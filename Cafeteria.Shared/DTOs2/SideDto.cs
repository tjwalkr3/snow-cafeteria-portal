using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs2;

public class SideDto
{
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }

    [Required]
    [StringLength(100)]
    public string SideName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? SideDescription { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal SidePrice { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }
}
