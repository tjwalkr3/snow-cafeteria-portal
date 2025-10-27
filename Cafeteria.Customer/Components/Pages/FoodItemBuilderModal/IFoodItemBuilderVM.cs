using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.FoodItemBuilderModal;

public interface IFoodItemBuilderVM
{
    FoodItemDtoOld? SelectedFoodItem { get; set; }
    List<IngredientDtoOld> SelectedIngredients { get; set; }
    Dictionary<IngredientTypeDtoOld, List<IngredientDtoOld>>? IngredientsByType { get; set; }
    Dictionary<string, string?> GetOrderAsJson();
    Task GetDataFromRouteParameters(string uri);
    Task InitializeWithFoodItem(FoodItemDtoOld foodItem);
    void ToggleIngredientSelection(IngredientDtoOld ingredient);
    bool IngredientIsSelected(IngredientDtoOld ingredient);
    bool ErrorOccurredWhileParsingSelectedFoodItem();
}