using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Customer.Components.ViewModelInterfaces;

namespace Cafeteria.Customer.Components.ViewModels;

public class FoodItemBuilderVM : IFoodItemBuilderVM
{
    public FoodItemBuilderVM()
    {
        SelectedFoodItem = DummyData.CreateTurkeysandwich();
        AvailableIngredients = DummyData.GetIngredientList;
        AvailableIngredientTypes = DummyData.GetIngredientTypeList;
        IngredientsByType = DummyData.GetIngredientsByType();
    }

    public FoodItemBuilderVM(FoodItemDto selectedItem)
    {
        SelectedFoodItem = selectedItem;
    }
    public FoodItemDto SelectedFoodItem { get; set; }
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

    public void GetDataFromRouteParameters(string uri)
    {
        string queryString = uri.Substring(uri.IndexOf('?') + 1);
        var queryParams = System.Web.HttpUtility.ParseQueryString(queryString);
        // have query params for ingredients be formatted like "&with-ingredient=1"
    }

    public string GetPartialQueryStringOfIngredients()
    {
        if (SelectedIngredients.Count == 0)
        {
            return string.Empty;
        }
        else
        {
            var queryParts = SelectedIngredients.Select(ing => $"with-ingredient={ing.Id}");
            return "&" + string.Join("&", queryParts);
        }
    }
}