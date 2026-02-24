namespace Cafeteria.Management.Components.Pages.Analytics;

public interface IAnalyticsVM
{
    int TotalOrdersToday { get; }
    decimal RevenueToday { get; }
    int SwipesToday { get; }

    Task LoadData();
}
