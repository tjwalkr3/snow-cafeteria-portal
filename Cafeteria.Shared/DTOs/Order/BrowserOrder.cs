namespace Cafeteria.Shared.DTOs.Order;

using Cafeteria.Shared.DTOs.Menu;

public class BrowserOrder
{
    public bool IsCardOrder { get; set; } = false;
    public LocationDto? Location { get; set; }
    public int StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
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
