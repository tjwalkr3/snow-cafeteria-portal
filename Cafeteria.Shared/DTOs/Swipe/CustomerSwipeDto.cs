namespace Cafeteria.Shared.DTOs.Swipe;

public class CustomerSwipeDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int BadgerId { get; set; }
    public int SwipeCount { get; set; }
}

