namespace Cafeteria.Shared.DTOs.Order;

public class OrderWithCustomerDto
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal Tax { get; set; }
    public int? TotalSwipe { get; set; }
    public int? CustomerBadgerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}
