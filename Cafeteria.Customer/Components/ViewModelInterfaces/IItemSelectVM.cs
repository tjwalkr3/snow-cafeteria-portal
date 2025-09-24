using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IItemSelectVM
{
    List<FoodItemDto> GetFoodItems();
}