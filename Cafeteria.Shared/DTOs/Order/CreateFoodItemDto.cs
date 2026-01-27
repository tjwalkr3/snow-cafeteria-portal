using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Shared.DTOs.Order;

public class CreateFoodItemDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int StationId { get; set; }
    public int? SaleCardId { get; set; }
    public int? SaleSwipeId { get; set; }
    public int? SwipeCost { get; set; }
    public decimal? CardCost { get; set; }
    public bool Special { get; set; }
    public List<CreateFoodItemOptionDto> Options { get; set; } = new();
}
