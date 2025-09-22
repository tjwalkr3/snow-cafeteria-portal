namespace Cafeteria.Customer.DTOs;

public class FoodItemDto
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
}