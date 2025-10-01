using Cafeteria.Shared.DTOs;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Customer.Components.ViewModelInterfaces;

namespace Cafeteria.Customer.Components.ViewModels;

public class PlaceOrderVM : IPlaceOrderVM
{
    public List<FoodItemDto> GetOrderItems()
    {
        return DummyData.GetFoodItemList;
    }

    public List<IngredientDto> GetOrderItemIngredients(int foodItemId)
    {
        // TODO: use dummy menu service to get ingredients for food item
        return new();
    }
}