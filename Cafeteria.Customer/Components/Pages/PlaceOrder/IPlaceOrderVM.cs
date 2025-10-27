using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.PlaceOrder;

public interface IPlaceOrderVM
{
    FoodItemDtoOld? SelectedFoodItem { get; }
    List<IngredientDtoOld> SelectedIngredients { get; }
    List<FoodItemDtoOld> GetOrderItems();
    Task GetDataFromRouteParameters(string uri);
    bool ErrorOccurredWhileParsingSelectedFoodItem();
}