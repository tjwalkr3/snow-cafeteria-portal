namespace Cafeteria.Shared.DTOs;

public class FoodItemOptionDto
{
    public int Id { get; set; }
    public int FoodItemOrderId { get; set; }
    public string? FoodOptionName { get; set; }
}
