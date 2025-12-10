using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class DrinkDto
{
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }

    [Required]
    [StringLength(100)]
    public string DrinkName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? DrinkDescription { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal DrinkPrice { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool InStock { get; set; } = true;
}
