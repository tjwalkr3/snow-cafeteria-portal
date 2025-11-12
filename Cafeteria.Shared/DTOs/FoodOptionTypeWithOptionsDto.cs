namespace Cafeteria.Shared.DTOs;

public class FoodOptionTypeWithOptionsDto
{
    public FoodOptionTypeDto OptionType { get; set; } = new();
    public List<FoodOptionDto> Options { get; set; } = new();
}
