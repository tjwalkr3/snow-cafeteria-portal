using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.GrillSwipe;

public class GrillSwipeVM : IGrillSwipeVM
{
    public List<EntreeDto> Entrees { get; private set; } = new List<EntreeDto>
    {
        new EntreeDto { Id = 1, StationId = 1, EntreeName = "Hamburger", EntreePrice = 4.99m },
        new EntreeDto { Id = 2, StationId = 1, EntreeName = "Cheeseburger", EntreePrice = 4.99m },
        new EntreeDto { Id = 3, StationId = 1, EntreeName = "Badger Burger", EntreePrice = 5.49m },
        new EntreeDto { Id = 4, StationId = 1, EntreeName = "Grilled Cheese", EntreePrice = 4.49m },
        new EntreeDto { Id = 5, StationId = 1, EntreeName = "Grilled Ham & Cheese", EntreePrice = 4.79m },
        new EntreeDto { Id = 6, StationId = 1, EntreeName = "BLT Sandwich", EntreePrice = 4.89m },
        new EntreeDto { Id = 7, StationId = 1, EntreeName = "Chicken Nuggets", EntreePrice = 4.99m },
        new EntreeDto { Id = 8, StationId = 1, EntreeName = "Breaded Chicken Sandwich", EntreePrice = 5.19m },
        new EntreeDto { Id = 9, StationId = 1, EntreeName = "Philly Cheesesteak", EntreePrice = 5.99m },
        new EntreeDto { Id = 10, StationId = 1, EntreeName = "Corn Dog", EntreePrice = 3.99m },
        new EntreeDto { Id = 11, StationId = 1, EntreeName = "Cheese Quesadilla", EntreePrice = 4.79m },
        new EntreeDto { Id = 12, StationId = 1, EntreeName = "Chicken Quesadilla", EntreePrice = 5.29m },
        new EntreeDto { Id = 13, StationId = 1, EntreeName = "Steak Quesadilla", EntreePrice = 5.79m },
        new EntreeDto { Id = 14, StationId = 1, EntreeName = "Vegetarian Chicken Tenders", EntreePrice = 4.99m },
        new EntreeDto { Id = 15, StationId = 1, EntreeName = "Vegetarian Burger", EntreePrice = 5.29m }
    };

    public List<SideDto> Sides { get; private set; } = new List<SideDto>
    {
        new SideDto { Id = 1, StationId = 1, SideName = "Fries", SidePrice = 2.49m },
        new SideDto { Id = 2, StationId = 1, SideName = "Tator Tots", SidePrice = 2.79m },
        new SideDto { Id = 3, StationId = 1, SideName = "Waffle Fries", SidePrice = 2.99m },
        new SideDto { Id = 4, StationId = 1, SideName = "Rice", SidePrice = 1.99m },
        new SideDto { Id = 5, StationId = 1, SideName = "Side Salad", SidePrice = 2.79m },
        new SideDto { Id = 6, StationId = 1, SideName = "Fruit Cup", SidePrice = 2.29m },
        new SideDto { Id = 7, StationId = 1, SideName = "Vegetables", SidePrice = 2.49m }
    };

    public List<DrinkDto> Drinks { get; private set; } = new List<DrinkDto>
    {
        new DrinkDto { Id = 1, StationId = 1, DrinkName = "Fountain Drinks", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 2, StationId = 1, DrinkName = "Slushies", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 3, StationId = 1, DrinkName = "Tea/Coffee Machines", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 4, StationId = 1, DrinkName = "Tea/Coffee Fridge", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 5, StationId = 1, DrinkName = "Drink Fridge", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m }
    };

    public string ActiveTab { get; private set; } = "entrees";
    public EntreeDto? SelectedEntree { get; private set; }
    public SideDto? SelectedSide { get; private set; }
    public DrinkDto? SelectedDrink { get; private set; }
    public string? OrderConfirmation { get; private set; }

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    public void SelectEntree(EntreeDto entree)
    {
        SelectedEntree = entree;
    }

    public void SelectSide(SideDto side)
    {
        SelectedSide = side;
    }

    public void SelectDrink(DrinkDto drink)
    {
        SelectedDrink = drink;
    }

    public int GetSelectionCount()
    {
        int count = 0;
        if (SelectedEntree != null) count++;
        if (SelectedSide != null) count++;
        if (SelectedDrink != null) count++;
        return count;
    }

    public bool IsValidSelection()
    {
        return SelectedEntree != null && SelectedSide != null && SelectedDrink != null;
    }

    public void AddToOrder()
    {
        if (IsValidSelection() && SelectedEntree != null && SelectedSide != null && SelectedDrink != null)
        {
            var meal = new MealDto
            {
                EntreeId = SelectedEntree.Id,
                SideId = SelectedSide.Id,
                DrinkId = SelectedDrink.Id
            };

            OrderConfirmation = $"Meal Plan/Swipe: {SelectedEntree.EntreeName} with {SelectedSide.SideName}";

            Console.WriteLine($"Grill Order: EntreeId={meal.EntreeId}, SideId={meal.SideId}, DrinkId={meal.DrinkId}");

            ClearSelections();
        }
    }

    public void ClearOrderConfirmation()
    {
        OrderConfirmation = null;
    }

    private void ClearSelections()
    {
        SelectedEntree = null;
        SelectedSide = null;
        SelectedDrink = null;
        ActiveTab = "entrees";
    }
}
