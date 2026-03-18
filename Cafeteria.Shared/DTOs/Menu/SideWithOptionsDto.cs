namespace Cafeteria.Shared.DTOs.Menu;

public class SideWithOptionsDto
{
    public SideDto Side { get; set; } = new();
    public List<FoodOptionTypeWithOptionsDto> OptionTypes { get; set; } = new();
}
