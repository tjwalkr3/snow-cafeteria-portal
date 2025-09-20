using Cafeteria.Shared;

namespace Cafeteria.Customer.Components.Pages
{
    public interface IItemSelectViewModel
    {
        List<FoodItem> MenuItems { get; }
        void InitializeMenuItems();
    }
}
