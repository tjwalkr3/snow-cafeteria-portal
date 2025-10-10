using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Services;
using System.Text.Json;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public class PlaceOrderVM : IPlaceOrderVM
{
    private readonly IApiMenuService _menuService;
    private bool urlParsingFailed = false;
    public FoodItemDto? SelectedFoodItem { get; private set; }
    public List<IngredientDto> SelectedIngredients { get; private set; } = new();

    public PlaceOrderVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public List<FoodItemDto> GetOrderItems()
    {
        if (SelectedFoodItem != null && !ErrorOccurredWhileParsingSelectedFoodItem())
        {
            return new List<FoodItemDto> { SelectedFoodItem };
        }
        return new List<FoodItemDto>(); // Return empty list when no food item selected
    }

    public async Task GetDataFromRouteParameters(string uri)
    {
        await Task.Delay(0); // Simulate async work

        if (!uri.Contains('?'))
        {
            // No query parameters, set error state
            urlParsingFailed = true;
            return;
        }

        string queryString = uri.Substring(uri.IndexOf('?') + 1);
        var queryParams = System.Web.HttpUtility.ParseQueryString(queryString);

        try
        {
            // Parse food item (required)
            FoodItemDto foodItem = JsonSerializer.Deserialize<FoodItemDto>(queryParams.Get("food-item") ?? string.Empty) ?? throw new ArgumentException("Failed to deserialize food item from query parameter.");
            SelectedFoodItem = foodItem;

            // Parse ingredients (optional)
            SelectedIngredients.Clear();
            foreach (string? key in queryParams.AllKeys.Where(k => !string.IsNullOrEmpty(k) && k != "food-item"))
            {
                try
                {
                    IngredientDto ingredient = JsonSerializer.Deserialize<IngredientDto>(queryParams.Get(key) ?? string.Empty) ?? throw new ArgumentException($"Failed to deserialize ingredient from query parameter: {key}");
                    SelectedIngredients.Add(ingredient);
                }
                catch
                {
                    // Ignore individual ingredient parsing failures since ingredients are optional
                    continue;
                }
            }
        }
        catch
        {
            urlParsingFailed = true;
            SelectedIngredients.Clear();
        }
    }

    public bool ErrorOccurredWhileParsingSelectedFoodItem()
    {
        return urlParsingFailed;
    }
}