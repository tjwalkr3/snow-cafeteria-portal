using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IItemSelectViewModel
{
    List<FoodItemDto> GetFoodItems();
}