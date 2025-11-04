using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class FoodOptionDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FoodOptionName { get; set; } = string.Empty;

    [Required]
    public bool InStock { get; set; } = true;

    [StringLength(500)]
    public string? ImageUrl { get; set; }
}