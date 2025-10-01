using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IPlaceOrderVM
{
    List<FoodItemDto> GetOrderItems();
    List<IngredientDto> GetOrderItemIngredients(int foodItemId);
}