using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Shared.Utilities;

public static class OrderCalculations
{
    public const decimal TaxRate = 0.0775m;

    public static decimal CalculateOptionsCost(List<SelectedFoodOption> selectedOptions)
    {
        if (selectedOptions == null || selectedOptions.Count == 0)
            return 0m;

        decimal cost = 0m;
        var optionsByType = selectedOptions.GroupBy(opt => opt.OptionType.Id);

        foreach (var group in optionsByType)
        {
            var optionType = group.First().OptionType;
            var selectedCount = group.Count();
            var chargeableCount = Math.Max(0, selectedCount - optionType.IncludedAmount);
            cost += chargeableCount * optionType.FoodOptionPrice;
        }

        return cost;
    }

    public static decimal CalculateTotalPrice(BrowserOrder order)
    {
        if (order == null)
            return 0m;

        decimal total = 0m;

        foreach (var entreeItem in order.Entrees)
        {
            total += entreeItem.Entree.EntreePrice;
            total += CalculateOptionsCost(entreeItem.SelectedOptions);
        }

        foreach (var sideItem in order.Sides)
        {
            total += sideItem.Side.SidePrice;
            total += CalculateOptionsCost(sideItem.SelectedOptions);
        }

        total += order.Drinks.Sum(drink => drink.DrinkPrice);
        return total;
    }

    public static decimal CalculateTax(BrowserOrder order)
    {
        if (order == null || !order.IsCardOrder)
            return 0m;

        return Math.Round(CalculateTotalPrice(order) * TaxRate, 2);
    }

    public static int CalculateTotalSwipe(BrowserOrder order)
    {
        if (order == null)
            return 0;

        return Math.Min(order.Entrees.Count,
                Math.Min(order.Sides.Count, order.Drinks.Count));
    }
}
