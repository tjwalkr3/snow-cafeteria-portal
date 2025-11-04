using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.BreakfastSwipe;

public class BreakfastSwipeVM : IBreakfastSwipeVM
{
    public List<EntreeDto> Entrees { get; private set; } = new List<EntreeDto>
    {
        new EntreeDto { Id = 1, StationId = 1, EntreeName = "Breakfast Sandwich", EntreeDescription = "Bread: English Muffin or Biscuit. ONE CHOICE of Meat: Sausage, Bacon, or Ham", EntreePrice = 4.99m },
        new EntreeDto { Id = 2, StationId = 1, EntreeName = "Breakfast Burrito", EntreeDescription = "Eggs, potatoes, cheese and choice of ONE MEAT: Bacon, Ham, or Sausage", EntreePrice = 5.29m },
        new EntreeDto { Id = 3, StationId = 1, EntreeName = "1 Pancake and Potatoes", EntreePrice = 4.49m },
        new EntreeDto { Id = 4, StationId = 1, EntreeName = "1 Pancake and Choice of ONE Meat", EntreeDescription = "Sausage, Bacon, or Ham", EntreePrice = 4.99m },
        new EntreeDto { Id = 5, StationId = 1, EntreeName = "French Toast and Potatoes", EntreePrice = 4.79m },
        new EntreeDto { Id = 6, StationId = 1, EntreeName = "1 Biscuit and Gravy with Potatoes", EntreePrice = 4.49m },
        new EntreeDto { Id = 7, StationId = 1, EntreeName = "Breakfast Plate", EntreeDescription = "Scrambled eggs, potatoes, one slice of toast and ONE choice of meat: Sausage, Bacon, or Ham", EntreePrice = 5.49m }
    };

    public List<SideDto> Sides { get; private set; } = new List<SideDto>
    {
        new SideDto { Id = 1, StationId = 1, SideName = "Toast", SidePrice = 1.49m },
        new SideDto { Id = 2, StationId = 1, SideName = "Bacon", SidePrice = 2.29m },
        new SideDto { Id = 3, StationId = 1, SideName = "Sausage Link", SidePrice = 2.29m },
        new SideDto { Id = 4, StationId = 1, SideName = "Tater Tots", SidePrice = 2.79m },
        new SideDto { Id = 5, StationId = 1, SideName = "Potatoes", SidePrice = 1.99m }
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

    public string? SelectedMeatOption { get; private set; }
    public string? SelectedBreadOption { get; private set; }

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    public void SelectEntree(EntreeDto entree)
    {
        SelectedEntree = entree;

        // Reset customization options when switching entrees
        SelectedMeatOption = null;
        SelectedBreadOption = null;

        // Set default selections for items that require them
        if (RequiresBreadSelection(entree.Id))
        {
            SelectedBreadOption = "English Muffin";
        }

        if (RequiresMeatSelection(entree.Id))
        {
            SelectedMeatOption = "Sausage";
        }
    }

    public void SelectSide(SideDto side)
    {
        SelectedSide = side;
    }

    public void SelectDrink(DrinkDto drink)
    {
        SelectedDrink = drink;
    }

    public void SetMeatOption(string meat)
    {
        SelectedMeatOption = meat;
    }

    public void SetBreadOption(string bread)
    {
        SelectedBreadOption = bread;
    }

    public bool RequiresMeatSelection(int entreeId)
    {
        return entreeId == 1 || entreeId == 2 || entreeId == 4 || entreeId == 7;
    }

    public bool RequiresBreadSelection(int entreeId)
    {
        return entreeId == 1;
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
        if (SelectedEntree == null || SelectedSide == null || SelectedDrink == null)
            return false;

        // Validate customization options
        if (RequiresMeatSelection(SelectedEntree.Id) && string.IsNullOrEmpty(SelectedMeatOption))
            return false;

        if (RequiresBreadSelection(SelectedEntree.Id) && string.IsNullOrEmpty(SelectedBreadOption))
            return false;

        return true;
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

            var customizations = new List<string>();
            if (!string.IsNullOrEmpty(SelectedBreadOption))
                customizations.Add(SelectedBreadOption);
            if (!string.IsNullOrEmpty(SelectedMeatOption))
                customizations.Add(SelectedMeatOption);

            var customizationText = customizations.Any() ? $" ({string.Join(", ", customizations)})" : "";
            OrderConfirmation = $"Meal Plan/Swipe: {SelectedEntree.EntreeName}{customizationText} with {SelectedSide.SideName}";

            Console.WriteLine($"Breakfast Order: EntreeId={meal.EntreeId}, SideId={meal.SideId}, DrinkId={meal.DrinkId}, Meat={SelectedMeatOption}, Bread={SelectedBreadOption}");

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
        SelectedMeatOption = null;
        SelectedBreadOption = null;
        ActiveTab = "entrees";
    }
}
