using Cafeteria.Management.Services.Orders;

namespace Cafeteria.Management.Components.Pages.Analytics;

public class AnalyticsVM(IOrderService orderService) : IAnalyticsVM
{
    public int TotalOrdersToday { get; private set; }
    public decimal RevenueToday { get; private set; }
    public int SwipesToday { get; private set; }

    public async Task LoadData()
    {
        var orders = await orderService.GetAllOrdersWithCustomer();
        var today = orders.Where(o => o.OrderTime.Date == DateTime.Today).ToList();

        TotalOrdersToday = today.Count;
        RevenueToday = today
            .Where(o => o.TotalPrice.HasValue)
            .Sum(o => o.TotalPrice!.Value + o.Tax);
        SwipesToday = today
            .Where(o => o.TotalSwipe.HasValue)
            .Sum(o => o.TotalSwipe!.Value);
    }
}
