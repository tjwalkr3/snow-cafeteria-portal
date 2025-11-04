namespace Cafeteria.Customer;

using Cafeteria.Shared.DTOs;

public class BrowserOrder
{
    public bool IsCardOrder { get; set; } = false;
    public LocationDto? Location { get; set; }
    public List<OrderEntreeItem> Entrees { get; set; } = [];
    public List<OrderSideItem> Sides { get; set; } = [];
    public List<DrinkDto> Drinks { get; set; } = [];
}

public class OrderEntreeItem
{
    public EntreeDto Entree { get; set; } = new();
    public List<SelectedFoodOption> SelectedOptions { get; set; } = [];
}

public class OrderSideItem
{
    public SideDto Side { get; set; } = new();
    public List<SelectedFoodOption> SelectedOptions { get; set; } = [];
}

public class SelectedFoodOption
{
    public FoodOptionDto Option { get; set; } = new();
    public FoodOptionTypeDto OptionType { get; set; } = new();
}
