using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Api.DTOs;

public class FoodItemDto
{
    public int Id { get; set; }

    [Required]
    public int StationId { get; set; }

    [StringLength(200)]
    public string? ItemDescription { get; set; }

    [StringLength(300)]
    public string? ImageUrl { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal ItemPrice { get; set; }
}