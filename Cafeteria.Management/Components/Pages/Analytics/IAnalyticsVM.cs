using Cafeteria.Shared.DTOs.Menu;

namespace Cafeteria.Management.Components.Pages.Analytics;

public record TopFoodEntry(string Label, string FoodName, int Count);

public enum AnalyticsPeriod { Day, Week, Month, Year, FiveYears }

public interface IAnalyticsVM
{
    List<TopFoodEntry> GetTopFoodForPeriod(AnalyticsPeriod period, int? stationId = null, int? locationId = null);
    int GetSwipesForPeriod(AnalyticsPeriod period, int? stationId = null, int? locationId = null);
    decimal GetCardRevenueForPeriod(AnalyticsPeriod period, int? stationId = null, int? locationId = null);
    bool HasData { get; }
    Task LoadData();
    List<StationDto> Stations { get; }
    List<LocationDto> Locations { get; }
}
