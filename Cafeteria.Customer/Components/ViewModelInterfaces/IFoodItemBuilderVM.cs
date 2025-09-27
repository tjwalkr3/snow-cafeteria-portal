using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IFoodItemBuilderVM
{
    FoodItemDto? SelectedFoodItem { get; set; }
    List<IngredientTypeDto> AvailableIngredientTypes { get; set; }
    List<IngredientDto> AvailableIngredients { get; set; }
    List<IngredientDto> SelectedIngredients { get; set; }
    Dictionary<IngredientTypeDto, List<IngredientDto>> IngredientsByType { get; set; }
    string GetPartialQueryStringOfIngredients();
    Task GetDataFromRouteParameters(string uri);
    void ToggleIngredientSelection(IngredientDto ingredient);
    bool IngredientIsSelected(IngredientDto ingredient);
}