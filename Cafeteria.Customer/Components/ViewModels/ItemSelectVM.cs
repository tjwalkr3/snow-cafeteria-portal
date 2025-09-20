using Cafeteria.Shared;

namespace Cafeteria.Customer.Components.Pages
{
    public class ItemSelectVM : IItemSelectViewModel
    {
        public List<FoodItem> MenuItems { get; private set; } = new();

        public ItemSelectVM()
        {
            InitializeMenuItems();
        }

        public void InitializeMenuItems()
        {
            var builder = new FoodItemBuilderService();

            MenuItems = new List<FoodItem>
            {
                builder.Reset()
                    .SetName("Burger")
                    .SetDescription("Classic beef burger with lettuce, tomato, and onion")
                    .SetImageUrl("https://via.placeholder.com/300x200?text=Burger")
                    .SetPrice(8.99m)
                    .Build(),
                
                builder.Reset()
                    .SetName("Cheeseburger")
                    .SetDescription("Juicy beef patty topped with melted cheese")
                    .SetImageUrl("https://via.placeholder.com/300x200?text=Cheeseburger")
                    .SetPrice(9.99m)
                    .Build(),
                
                builder.Reset()
                    .SetName("French Fries")
                    .SetDescription("Crispy golden french fries with sea salt")
                    .SetImageUrl("https://via.placeholder.com/300x200?text=French+Fries")
                    .SetPrice(4.49m)
                    .Build(),
                
                builder.Reset()
                    .SetName("Waffle Fries")
                    .SetDescription("Crispy waffle-cut fries with perfect seasoning")
                    .SetImageUrl("https://via.placeholder.com/300x200?text=Waffle+Fries")
                    .SetPrice(4.99m)
                    .Build(),
                
                builder.Reset()
                    .SetName("Grilled Cheese")
                    .SetDescription("Golden grilled cheese sandwich with melted cheddar")
                    .SetImageUrl("https://via.placeholder.com/300x200?text=Grilled+Cheese")
                    .SetPrice(6.49m)
                    .Build()
            };
        }
    }
}
