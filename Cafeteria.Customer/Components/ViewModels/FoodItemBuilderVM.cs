using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using System.Text.Json;

namespace Cafeteria.Customer.Components.ViewModels;

public class FoodItemBuilderVM : IFoodItemBuilderVM
{
    string errorString = "Error";
    public FoodItemDto? SelectedFoodItem { get; set; }
    public List<IngredientDto> SelectedIngredients { get; set; } = new List<IngredientDto>();
    public Dictionary<IngredientTypeDto, List<IngredientDto>>? IngredientsByType { get; set; } = new Dictionary<IngredientTypeDto, List<IngredientDto>>();

    public void ToggleIngredientSelection(IngredientDto ingredient)
    {
        if (IngredientIsSelected(ingredient))
        {
            UnselectIngredient(ingredient);
        }
        else SelectIngredient(ingredient);
    }

    public bool IngredientIsSelected(IngredientDto ingredient)
    {
        return SelectedIngredients.Contains(ingredient);
    }

    public void SelectIngredient(IngredientDto ingredient)
    {
        if (!SelectedIngredients.Contains(ingredient))
        {
            SelectedIngredients.Add(ingredient);
        }
    }

    public void UnselectIngredient(IngredientDto ingredient)
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
            FoodItemDto foodItem = JsonSerializer.Deserialize<FoodItemDto>(queryParams.Get("food-item") ?? string.Empty) ?? throw new ArgumentException("Failed to deserialize food item from query parameter.");
            SelectedFoodItem = foodItem;
            List<IngredientTypeDto> ingredientTypes = DummyData.GetIngredientTypesByFoodItem(SelectedFoodItem.Id);
            IngredientsByType = DummyData.GetIngredientsByType(ingredientTypes);
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