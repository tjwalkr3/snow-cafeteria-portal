using Cafeteria.Management.Services.Orders;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Management.Components.Pages.Analytics;

public class AnalyticsVM(IOrderService orderService) : IAnalyticsVM
{
    private List<OrderWithCustomerDto> _orders = [];

    public bool HasData => _orders.Any(o => o.FoodItems.Count > 0);

    public async Task LoadData()
    {
        _orders = await orderService.GetAllOrdersWithCustomer();
    }

    public List<TopFoodEntry> GetTopFoodForPeriod(AnalyticsPeriod period)
    {
        var now = DateTime.Now;

        Func<DateTime, bool> inRange;
        Func<DateTime, DateTime> truncate;
        Func<DateTime, string> format;

        switch (period)
        {
            case AnalyticsPeriod.Day:
                inRange   = t => t.Date == now.Date;
                truncate  = t => new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0);
                format    = t => t.ToString("h tt, MMM d yyyy");
                break;
            case AnalyticsPeriod.Week:
                inRange   = t => t.Date >= now.Date.AddDays(-6);
                truncate  = t => t.Date;
                format    = t => t.ToString("ddd MMM d, yyyy");
                break;
            case AnalyticsPeriod.Month:
                inRange   = t => t.Date >= now.Date.AddDays(-29);
                truncate  = t => t.Date;
                format    = t => t.ToString("MMM d, yyyy");
                break;
            case AnalyticsPeriod.Year:
                inRange   = t => t >= now.AddYears(-1);
                truncate  = t => new DateTime(t.Year, t.Month, 1);
                format    = t => t.ToString("MMM yyyy");
                break;
            case AnalyticsPeriod.FiveYears:
            default:
                inRange   = t => t >= now.AddYears(-5);
                truncate  = t => new DateTime(t.Year, 1, 1);
                format    = t => t.ToString("yyyy");
                break;
        }

        return _orders
            .SelectMany(o => o.FoodItems.Select(f => new { o.OrderTime, f.Name }))
            .Where(x => inRange(x.OrderTime))
            .GroupBy(x => truncate(x.OrderTime))
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var top = g.GroupBy(x => x.Name)
                           .OrderByDescending(ng => ng.Count())
                           .First();
                return new TopFoodEntry(format(g.Key), top.Key, top.Count());
            })
            .ToList();
    }
}
