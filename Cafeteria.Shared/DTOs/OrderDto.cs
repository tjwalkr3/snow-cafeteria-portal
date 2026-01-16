namespace Cafeteria.Shared.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public decimal TotalPrice { get; set; }
    public List<FoodItemOrderDto> FoodItems { get; set; } = new();
}
