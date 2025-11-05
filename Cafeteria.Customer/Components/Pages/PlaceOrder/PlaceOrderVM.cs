using Cafeteria.Shared.DTOsOld;
using Cafeteria.Customer.Services;
using System.Text.Json;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public class PlaceOrderVM : IPlaceOrderVM
{
    public decimal CalculateTotalPrice(BrowserOrder order)
    {
        if (order == null)
            return 0m;

        decimal total = 0m;

        foreach (var entreeItem in order.Entrees)
        {
            total += entreeItem.Entree.EntreePrice;
            total += entreeItem.SelectedOptions.Sum(opt => opt.OptionType.FoodOptionPrice);
        }

        foreach (var sideItem in order.Sides)
        {
            total += sideItem.Side.SidePrice;
            total += sideItem.SelectedOptions.Sum(opt => opt.OptionType.FoodOptionPrice);
        }

        total += order.Drinks.Sum(drink => drink.DrinkPrice);

        return total;
    }
}