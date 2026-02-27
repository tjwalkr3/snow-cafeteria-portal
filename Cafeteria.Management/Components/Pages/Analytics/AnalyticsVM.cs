using Cafeteria.Management.Services.Locations;
using Cafeteria.Management.Services.Orders;
using Cafeteria.Management.Services.Stations;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Management.Components.Pages.Analytics;

public class AnalyticsVM(IOrderService orderService, IStationService stationService, ILocationService locationService) : IAnalyticsVM
{
    private List<OrderWithCustomerDto> _orders = [];

    public bool HasData => _orders.Any(o => o.FoodItems.Count > 0);
    public List<StationDto> Stations { get; private set; } = [];
    public List<LocationDto> Locations { get; private set; } = [];

    public async Task LoadData()
    {
        var ordersTask = orderService.GetAllOrdersWithCustomer();
        var stationsTask = stationService.GetAllStations();
        var locationsTask = locationService.GetAllLocations();
        await Task.WhenAll(ordersTask, stationsTask, locationsTask);
        _orders = ordersTask.Result;
        Stations = stationsTask.Result;
        Locations = locationsTask.Result;
    }

    public List<TopFoodEntry> GetTopFoodForPeriod(AnalyticsPeriod period, int? stationId = null, int? locationId = null)
    {
        var now = DateTime.Now;

        Func<DateTime, bool> inRange;
        Func<DateTime, DateTime> truncate;
        Func<DateTime, string> format;

        switch (period)
        {
            case AnalyticsPeriod.Day:
                inRange = t => t.Date == now.Date;
                truncate = t => new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0);
                format = t => t.ToString("h tt, MMM d yyyy");
                break;
            case AnalyticsPeriod.Week:
                inRange = t => t.Date >= now.Date.AddDays(-6);
                truncate = t => t.Date;
                format = t => t.ToString("ddd MMM d, yyyy");
                break;
            case AnalyticsPeriod.Month:
                inRange = t => t.Date >= now.Date.AddDays(-29);
                truncate = t => t.Date;
                format = t => t.ToString("MMM d, yyyy");
                break;
            case AnalyticsPeriod.Year:
                inRange = t => t >= now.AddYears(-1);
                truncate = t => new DateTime(t.Year, t.Month, 1);
                format = t => t.ToString("MMM yyyy");
                break;
            case AnalyticsPeriod.FiveYears:
            default:
                inRange = t => t >= now.AddYears(-5);
                truncate = t => new DateTime(t.Year, 1, 1);
                format = t => t.ToString("yyyy");
                break;
        }

        var stationIdsForLocation = locationId.HasValue
            ? Stations.Where(s => s.LocationId == locationId).Select(s => (int?)s.Id).ToHashSet()
            : null;

        return _orders
            .SelectMany(o => o.FoodItems.Select(f => new { o.OrderTime, f.Name, f.StationId, f.LocationId }))
            .Where(x => inRange(x.OrderTime))
            .Where(x =>
            {
                if (stationId.HasValue)
                    return x.StationId == stationId;
                if (locationId.HasValue)
                    return x.LocationId == locationId || (x.StationId.HasValue && stationIdsForLocation!.Contains(x.StationId));
                return true;
            })
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
