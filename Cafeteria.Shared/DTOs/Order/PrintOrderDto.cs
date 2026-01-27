namespace Cafeteria.Shared.DTOs.Order;

public class PrintOrderDto
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public List<FoodItemDto> FoodItems { get; set; } = new();
}
