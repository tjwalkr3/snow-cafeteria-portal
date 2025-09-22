using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Api.DTOs;

public class IngredientTypeDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TypeName { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}