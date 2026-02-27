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
        var (inRange, truncate, format) = GetPeriodFunctions(period);

        return GetFilteredItems(inRange, stationId, locationId)
            .Select(f => new { f.OrderTime, f.Name })
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

    public int GetSwipesForPeriod(AnalyticsPeriod period, int? stationId = null, int? locationId = null)
    {
        var (inRange, _, _) = GetPeriodFunctions(period);
        return GetFilteredItems(inRange, stationId, locationId)
            .Sum(f => f.SwipeCost ?? 0);
    }

    public decimal GetCardRevenueForPeriod(AnalyticsPeriod period, int? stationId = null, int? locationId = null)
    {
        var (inRange, _, _) = GetPeriodFunctions(period);
        return GetFilteredItems(inRange, stationId, locationId)
            .Sum(f => f.CardCost ?? 0);
    }

    private IEnumerable<(DateTime OrderTime, string Name, int? StationId, int? LocationId, int? SwipeCost, decimal? CardCost)>
        GetFilteredItems(Func<DateTime, bool> inRange, int? stationId, int? locationId)
    {
        var stationIdsForLocation = locationId.HasValue
            ? Stations.Where(s => s.LocationId == locationId).Select(s => (int?)s.Id).ToHashSet()
            : null;

        return _orders
            .SelectMany(o => o.FoodItems.Select(f =>
                (o.OrderTime, f.Name, f.StationId, f.LocationId, f.SwipeCost, f.CardCost)))
            .Where(x => inRange(x.OrderTime))
            .Where(x =>
            {
                if (stationId.HasValue)
                    return x.StationId == stationId;
                if (locationId.HasValue)
                    return x.LocationId == locationId || (x.StationId.HasValue && stationIdsForLocation!.Contains(x.StationId));
                return true;
            });
    }

    private static (Func<DateTime, bool> inRange, Func<DateTime, DateTime> truncate, Func<DateTime, string> format)
        GetPeriodFunctions(AnalyticsPeriod period)
    {
        var now = DateTime.Now;
        return period switch
        {
            AnalyticsPeriod.Day => (
                t => t.Date == now.Date,
                t => new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0),
                t => t.ToString("h tt, MMM d yyyy")),
            AnalyticsPeriod.Week => (
                t => t.Date >= now.Date.AddDays(-6),
                t => t.Date,
                t => t.ToString("ddd MMM d, yyyy")),
            AnalyticsPeriod.Month => (
                t => t.Date >= now.Date.AddDays(-29),
                t => t.Date,
                t => t.ToString("MMM d, yyyy")),
            AnalyticsPeriod.Year => (
                t => t >= now.AddYears(-1),
                t => new DateTime(t.Year, t.Month, 1),
                t => t.ToString("MMM yyyy")),
            _ => (
                t => t >= now.AddYears(-5),
                t => new DateTime(t.Year, 1, 1),
                t => t.ToString("yyyy")),
        };
    }
}
