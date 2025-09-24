namespace Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Shared.DTOs;

public interface IItemSelectViewModel
{
    List<FoodItemDto> GetFoodItems();
}