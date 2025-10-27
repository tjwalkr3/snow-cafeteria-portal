using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Services;
using System.Text.Json;

namespace Cafeteria.Customer.Components.Pages.FoodItemBuilderModal;

public class FoodItemBuilderVM : IFoodItemBuilderVM
{
    private readonly IApiMenuService _menuService;
    string errorString = "Error";
    public FoodItemDtoOld? SelectedFoodItem { get; set; }
    public List<IngredientDtoOld> SelectedIngredients { get; set; } = new List<IngredientDtoOld>();
    public Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>? IngredientsByType { get; set; } = new Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>();

    public FoodItemBuilderVM(IApiMenuService menuService)
    {
        _menuService = menuService;
    }

    public void ToggleIngredientSelection(IngredientDtoOld ingredient)
    {
        if (IngredientIsSelected(ingredient))
        {
            UnselectIngredient(ingredient);
        }
        else SelectIngredient(ingredient);
    }

    public bool IngredientIsSelected(IngredientDtoOld ingredient)
    {
        return SelectedIngredients.Contains(ingredient);
    }

    public void SelectIngredient(IngredientDtoOld ingredient)
    {
        if (!SelectedIngredients.Contains(ingredient))
        {
            SelectedIngredients.Add(ingredient);
        }
    }

    public void UnselectIngredient(IngredientDtoOld ingredient)
    {
        if (SelectedIngredients.Contains(ingredient))
        {
            SelectedIngredients.Remove(ingredient);
        }
    }

    public async Task GetDataFromRouteParameters(string uri)
    {
        await Task.Delay(0); // Simulate async work

        string queryString = uri.Substring(uri.IndexOf('?') + 1);

        var queryParams = System.Web.HttpUtility.ParseQueryString(queryString);

        try
        {
            FoodItemDtoOld foodItem = JsonSerializer.Deserialize<FoodItemDtoOld>(queryParams.Get("food-item") ?? string.Empty) ?? throw new ArgumentException("Failed to deserialize food item from query parameter.");
            SelectedFoodItem = foodItem;
            List<IngredientTypeDtoOld> ingredientTypes = await _menuService.GetIngredientTypesByFoodItem(SelectedFoodItem.Id);
            IngredientsByType = await _menuService.GetIngredientsOrganizedByType(ingredientTypes);
        }
        catch
        {
            SelectedFoodItem = new();
            SelectedFoodItem.ItemDescription = errorString;
        }
    }

    public async Task InitializeWithFoodItem(FoodItemDtoOld foodItem)
    {
        try
        {
            SelectedFoodItem = foodItem;
            SelectedIngredients.Clear();
            List<IngredientTypeDtoOld> ingredientTypes = await _menuService.GetIngredientTypesByFoodItem(SelectedFoodItem.Id);
            IngredientsByType = await _menuService.GetIngredientsOrganizedByType(ingredientTypes);
        }
        catch
        {
            SelectedFoodItem = new();
            SelectedFoodItem.ItemDescription = errorString;
        }
    }

    public Dictionary<string, string?> GetOrderAsJson()
    {
        Dictionary<string, string?> orderAsJson = new()
        {
            { "food-item", JsonSerializer.Serialize(SelectedFoodItem) }
        };

        foreach (var ingredient in SelectedIngredients)
        {
            string json = JsonSerializer.Serialize(ingredient);
            orderAsJson.Add(ingredient.IngredientName, json);
        }
        return orderAsJson;
    }

    public bool ErrorOccurredWhileParsingSelectedFoodItem()
    {
        return SelectedFoodItem != null && SelectedFoodItem.ItemDescription == errorString;
    }
}