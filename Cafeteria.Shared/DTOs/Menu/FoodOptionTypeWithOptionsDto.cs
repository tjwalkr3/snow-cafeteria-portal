namespace Cafeteria.Shared.DTOs.Menu;

public class FoodOptionTypeWithOptionsDto
{
    public FoodOptionTypeDto OptionType { get; set; } = new();
    public List<FoodOptionDto> Options { get; set; } = new();
}
