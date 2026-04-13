using Microsoft.AspNetCore.Components;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Management.Services.Orders;

namespace Cafeteria.Management.Components.Pages.Order;

public partial class OrderDetail : ComponentBase
{
    [Parameter]
    public int OrderId { get; set; }

    [Inject]
    public IOrderService OrderService { get; set; } = default!;

    private OrderWithCustomerDto? Order { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Order = await OrderService.GetOrderWithCustomerById(OrderId);
    }

    public sealed class GroupedCardItem
    {
        public FoodItemDto Item { get; init; } = new();
        public int Count { get; init; }
    }

    public sealed class GroupedSwipeItem
    {
        public FoodItemDto Entree { get; init; } = new();
        public FoodItemDto Side { get; init; } = new();
        public FoodItemDto Drink { get; init; } = new();
        public int Count { get; init; }
    }

    public static List<GroupedCardItem> GetGroupedCardItems(List<FoodItemDto> foodItems)
    {
        return foodItems
            .GroupBy(item => GetFoodItemGroupKey(item))
            .Select(group => new GroupedCardItem { Item = group.First(), Count = group.Count() })
            .ToList();
    }

    public static List<GroupedSwipeItem> GetGroupedSwipeItems(List<FoodItemDto> foodItems)
    {
        var swipeCombos = new List<(FoodItemDto Entree, FoodItemDto Side, FoodItemDto Drink)>();

        for (int i = 0; i + 2 < foodItems.Count; i += 3)
        {
            swipeCombos.Add((foodItems[i], foodItems[i + 1], foodItems[i + 2]));
        }

        return swipeCombos
            .GroupBy(combo => new
            {
                EntreeKey = GetFoodItemGroupKey(combo.Entree),
                SideKey = GetFoodItemGroupKey(combo.Side),
                DrinkKey = GetFoodItemGroupKey(combo.Drink)
            })
            .Select(group => new GroupedSwipeItem
            {
                Entree = group.First().Entree,
                Side = group.First().Side,
                Drink = group.First().Drink,
                Count = group.Count()
            })
            .ToList();
    }

    private static string GetFoodItemGroupKey(FoodItemDto item)
    {
        var optionsKey = string.Join("|", item.Options.Select(o => o.FoodOptionName ?? string.Empty).OrderBy(o => o));
        return string.Join("::",
            item.Name,
            item.StationId?.ToString() ?? string.Empty,
            item.LocationId?.ToString() ?? string.Empty,
            item.CardCost?.ToString("F2") ?? string.Empty,
            item.SwipeCost?.ToString() ?? string.Empty,
            optionsKey);
    }
}
