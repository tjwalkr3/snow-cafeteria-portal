using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using System.Text.Json;

namespace Cafeteria.Customer.Components.ViewModels;

public class FoodItemBuilderVM : IFoodItemBuilderVM
{
    public FoodItemBuilderVM()
    {
        // SelectedFoodItem = DummyData.CreateTurkeysandwich(); // uncomment this line to use for testing
        // SelectedFoodItem gets initialized in the GetDataFromRouteParameters method, which is called in the Blazor page's OnInitialized method
        AvailableIngredients = DummyData.GetIngredientList;
        AvailableIngredientTypes = DummyData.GetIngredientTypeList;
        IngredientsByType = DummyData.GetIngredientsByType();
    }

    public FoodItemBuilderVM(FoodItemDto selectedItem)
    {
        SelectedFoodItem = selectedItem;
    }
    public FoodItemDto? SelectedFoodItem { get; set; }
    public List<IngredientTypeDto> AvailableIngredientTypes { get; set; } = new List<IngredientTypeDto>();
    public List<IngredientDto> AvailableIngredients { get; set; } = new List<IngredientDto>();
    public List<IngredientDto> SelectedIngredients { get; set; } = new List<IngredientDto>();
    public Dictionary<IngredientTypeDto, List<IngredientDto>> IngredientsByType { get; set; } = new Dictionary<IngredientTypeDto, List<IngredientDto>>();

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
        }
        catch
        {
            SelectedFoodItem = new();
            SelectedFoodItem.ItemDescription = IFoodItemBuilderVM.ItemDescriptionWhenError; // TODO: this feels like a jenky way of handling this, so probably consider refactoring
        }
    }

    public string GetPartialQueryStringOfIngredients()
    {
        if (SelectedIngredients.Count == 0)
        {
            return string.Empty;
        }
        else
        {
            var queryParts = AvailableIngredients.Select(ingredient => JsonSerializer.Serialize(ingredient));
            // var queryParts = SelectedIngredients.Select(ingredient => JsonSerializer.Serialize(ingredient));
            return "&" + string.Join("&", queryParts);
        }
    }
}