namespace Cafeteria.Customer.Components.Pages.ThankYouPage;

public class ThankYouPageVM
{
    public string OrderId { get; set; } = string.Empty;
    public string EstimatedTime { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;

    public void Initialize(string? orderId = null, string? estimatedTime = null, string? locationName = null)
    {
        OrderId = orderId ?? "N/A";
        EstimatedTime = estimatedTime ?? "15-20 minutes";
        LocationName = locationName ?? "Cafeteria";
    }
}