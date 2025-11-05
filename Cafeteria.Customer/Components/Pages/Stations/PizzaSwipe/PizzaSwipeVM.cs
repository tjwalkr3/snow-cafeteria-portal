using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.PizzaSwipe;

public class PizzaSwipeVM : IPizzaSwipeVM
{
    public List<EntreeDto> Entrees { get; private set; } = new List<EntreeDto>
    {
        new EntreeDto { Id = 1, StationId = 2, EntreeName = "Personal Pizza", EntreeDescription = "Choose 2 toppings included", EntreePrice = 5.69m }
    };

    public List<DrinkDto> Drinks { get; private set; } = new List<DrinkDto>
    {
        new DrinkDto { Id = 1, StationId = 2, DrinkName = "Fountain Drinks", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 2, StationId = 2, DrinkName = "Slushies", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 3, StationId = 2, DrinkName = "Tea/Coffee Machines", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 4, StationId = 2, DrinkName = "Tea/Coffee Fridge", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 5, StationId = 2, DrinkName = "Drink Fridge", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m }
    };

    public string ActiveTab { get; private set; } = "toppings";
    public EntreeDto? SelectedEntree { get; private set; }
    public DrinkDto? SelectedDrink { get; private set; }
    public List<string> SelectedToppings { get; private set; } = new();
    public string? OrderConfirmation { get; private set; }

    public List<string> AvailableToppings { get; private set; } = new List<string>
    {
        "Extra Cheese",
        "Pepperoni",
        "Sausage",
        "Bacon",
        "Chicken",
        "Ham",
        "Olives",
        "Mushrooms",
        "Onions",
        "Pineapple",
        "Bell Peppers",
        "Banana Peppers"
    };

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    public void SelectEntree(EntreeDto entree)
    {
        SelectedEntree = entree;
    }

    public void SelectDrink(DrinkDto drink)
    {
        SelectedDrink = drink;
    }

    public void ToggleTopping(string topping)
    {
        if (SelectedToppings.Contains(topping))
        {
            SelectedToppings.Remove(topping);
        }
        else
        {
            SelectedToppings.Add(topping);
        }
    }

    public int GetSelectionCount()
    {
        int count = 0;
        if (SelectedToppings.Count >= 2) count++;
        if (SelectedDrink != null) count++;
        return count;
    }

    public bool IsValidSelection()
    {
        return SelectedToppings.Count >= 2 && SelectedDrink != null;
    }

    public void AddToOrder()
    {
        if (IsValidSelection() && SelectedDrink != null)
        {
            // Auto-select the personal pizza entree if not already selected
            if (SelectedEntree == null && Entrees.Any())
            {
                SelectedEntree = Entrees.First();
            }

            var meal = new MealDto
            {
                EntreeId = SelectedEntree?.Id ?? 1,
                SideId = 0,  // No side for pizza
                DrinkId = SelectedDrink.Id
            };

            var toppingsText = string.Join(", ", SelectedToppings);
            OrderConfirmation = $"Personal Pizza with {toppingsText} and {SelectedDrink.DrinkName}";

            Console.WriteLine($"Pizza Order: EntreeId={meal.EntreeId}, DrinkId={meal.DrinkId}, Toppings={toppingsText}");

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
        SelectedDrink = null;
        SelectedToppings.Clear();
        ActiveTab = "toppings";
    }
}
