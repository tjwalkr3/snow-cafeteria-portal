namespace Cafeteria.Management.Components.Pages.Analytics;

public record TopFoodEntry(string Label, string FoodName, int Count);

public enum AnalyticsPeriod { Day, Week, Month, Year, FiveYears }

public interface IAnalyticsVM
{
    List<TopFoodEntry> GetTopFoodForPeriod(AnalyticsPeriod period);
    bool HasData { get; }
    Task LoadData();
}
