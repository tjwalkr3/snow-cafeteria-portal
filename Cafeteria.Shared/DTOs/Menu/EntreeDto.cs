using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs.Menu;

public class EntreeDto
{
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }

    [Required]
    [StringLength(100)]
    public string EntreeName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? EntreeDescription { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal EntreePrice { get; set; }

    public bool InStock { get; set; } = true;
}
