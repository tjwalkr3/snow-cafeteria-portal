namespace Cafeteria.Customer.Components.ViewModels;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Shared.DTOs;

public class ItemSelectVM : IItemSelectVM
{
    public List<FoodItemDto> GetFoodItems()
    {
        return DummyData.GetFoodItemList;
    }
}