using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs.Order;

public class CreateOrderDto
{
    [Range(0, double.MaxValue)]
    public decimal? TotalPrice { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Tax { get; set; }

    [Range(0, int.MaxValue)]
    public int? TotalSwipe { get; set; }

    [Required]
    public List<CreateFoodItemDto> FoodItems { get; set; } = new();
}
