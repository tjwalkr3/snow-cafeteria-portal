using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IPlaceOrderVM
{
    FoodItemDto? SelectedFoodItem { get; }
    List<IngredientDto> SelectedIngredients { get; }
    List<FoodItemDto> GetOrderItems();
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingSelectedFoodItem();
}