using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs;

public class CreateOrderDto
{
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalPrice { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Tax { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int TotalSwipe { get; set; }

    [Required]
    public List<CreateFoodItemOrderDto> FoodItems { get; set; } = new();
}

public class CreateFoodItemOrderDto
{
    [Required]
    public int StationId { get; set; }

    public int? SaleCardId { get; set; }
    public int? SaleSwipeId { get; set; }
    public int? SwipeCost { get; set; }
    public decimal? CardCost { get; set; }
    public bool Special { get; set; }

    public List<CreateFoodItemOptionDto> Options { get; set; } = new();
}

public class CreateFoodItemOptionDto
{
    public string? FoodOptionName { get; set; }
}
