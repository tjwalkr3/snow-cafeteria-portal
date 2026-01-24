namespace Cafeteria.Shared.DTOs.Order;

public class FoodItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public int StationId { get; set; }
    public int? SaleCardId { get; set; }
    public int? SaleSwipeId { get; set; }
    public int? SwipeCost { get; set; }
    public decimal? CardCost { get; set; }
    public bool Special { get; set; }
    public List<FoodItemOptionDto> Options { get; set; } = new();
}
