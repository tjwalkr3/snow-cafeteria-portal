using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class IngredientDtoOld
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string IngredientName { get; set; } = string.Empty;

    [StringLength(300)]
    public string? ImageUrl { get; set; }

    [Range(0.01, 999.99)]
    public decimal? IngredientPrice { get; set; }
}