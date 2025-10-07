using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IFoodItemBuilderVM
{
    FoodItemDto? SelectedFoodItem { get; set; }
    List<IngredientDto> SelectedIngredients { get; set; }
    Dictionary<IngredientTypeDto, List<IngredientDto>>? IngredientsByType { get; set; }
    Dictionary<string, string?> GetOrderAsJson();
    Task GetDataFromRouteParameters(string uri);
    Task InitializeWithFoodItem(FoodItemDto foodItem);
    void ToggleIngredientSelection(IngredientDto ingredient);
    bool IngredientIsSelected(IngredientDto ingredient);
    bool ErrorOccurredWhileParsingSelectedFoodItem();
}