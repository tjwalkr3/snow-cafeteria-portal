namespace Cafeteria.Shared.DTOs;

public class PrintOrderDto
{
    public int OrderId { get; set; }
    public DateTime OrderTime { get; set; }
    public decimal TotalPrice { get; set; }
    public List<FoodItemOrderDto> FoodItems { get; set; } = new();
}
