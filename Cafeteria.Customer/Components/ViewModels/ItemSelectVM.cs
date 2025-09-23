namespace Cafeteria.Customer.Components.ViewModels;
using Cafeteria.Customer.Components.ViewModelInterfaces;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Shared;

public class ItemSelectVM : IItemSelectViewModel
{
    public List<FoodItem> GetFoodItems()
    {
        return DummyData.GetFoodItemList;
    }
}