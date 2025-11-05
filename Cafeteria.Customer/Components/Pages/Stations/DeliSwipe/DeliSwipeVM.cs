using Cafeteria.Shared.DTOs;

namespace Cafeteria.Customer.Components.Pages.Stations.DeliSwipe;

public class DeliSwipeVM : IDeliSwipeVM
{
    public List<SideDto> Sides { get; private set; } = new List<SideDto>
    {
        new SideDto { Id = 1, StationId = 3, SideName = "Bacon", SidePrice = 2.49m },
        new SideDto { Id = 2, StationId = 3, SideName = "Cheese", SidePrice = 1.99m },
        new SideDto { Id = 3, StationId = 3, SideName = "Additional Meats", SidePrice = 2.99m },
        new SideDto { Id = 4, StationId = 3, SideName = "Rice", SidePrice = 1.99m },
        new SideDto { Id = 5, StationId = 3, SideName = "Vegetables", SidePrice = 2.49m },
        new SideDto { Id = 6, StationId = 3, SideName = "Fruit Cup", SidePrice = 2.29m },
        new SideDto { Id = 7, StationId = 3, SideName = "Chips", SidePrice = 1.79m },
        new SideDto { Id = 8, StationId = 3, SideName = "Side Salad", SidePrice = 2.79m }
    };

    public List<DrinkDto> Drinks { get; private set; } = new List<DrinkDto>
    {
        new DrinkDto { Id = 1, StationId = 3, DrinkName = "Fountain Drinks", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 2, StationId = 3, DrinkName = "Slushies", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 3, StationId = 3, DrinkName = "Tea/Coffee Machines", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 4, StationId = 3, DrinkName = "Tea/Coffee Fridge", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m },
        new DrinkDto { Id = 5, StationId = 3, DrinkName = "Drink Fridge", DrinkDescription = "Included with your meal", DrinkPrice = 0.00m }
    };

    public string ActiveTab { get; private set; } = "sandwich";
    public SideDto? SelectedSide { get; private set; }
    public DrinkDto? SelectedDrink { get; private set; }
    public string? OrderConfirmation { get; private set; }

    public string? SelectedBread { get; private set; }
    public string? SelectedMeat { get; private set; }
    public string? SelectedCheese { get; private set; }
    public List<string> SelectedToppings { get; private set; } = new();
    public string? SelectedDressing { get; private set; }

    public List<string> BreadOptions { get; private set; } = new()
    {
        "Pretzel Bun", "Marble Rye", "Sourdough", "Pita Bread", "Wheat", "White Bread"
    };

    public List<string> MeatOptions { get; private set; } = new()
    {
        "Ham", "Turkey", "Bacon", "Grilled Chicken", "Tuna Salad"
    };

    public List<string> CheeseOptions { get; private set; } = new()
    {
        "American", "Provolone", "Cheddar", "Pepper Jack", "Swiss", "Mozzarella"
    };

    public List<string> ToppingOptions { get; private set; } = new()
    {
        "Spinach", "Romaine", "Green Leaf Lettuce", "Olives", "Cucumber",
        "Tomato", "Sprouts", "Bell Pepper", "Onions", "Pickles", "Banana Peppers"
    };

    public List<string> DressingOptions { get; private set; } = new()
    {
        "Oil", "Vinegar", "Ranch", "1000 Island", "Italian", "Caesar",
        "Raspberry Vinaigrette", "Honey Mustard", "Mayonnaise", "Mustard"
    };

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    public void SelectSide(SideDto side)
    {
        SelectedSide = side;
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

    public void SetBread(string bread)
    {
        SelectedBread = bread;
    }

    public void SetMeat(string meat)
    {
        SelectedMeat = meat;
    }

    public void SetCheese(string cheese)
    {
        SelectedCheese = cheese;
    }

    public void SetDressing(string dressing)
    {
        SelectedDressing = dressing;
    }

    public int GetSelectionCount()
    {
        int count = 0;
        if (SelectedBread != null) count++;
        if (SelectedMeat != null) count++;
        if (SelectedCheese != null) count++;
        if (SelectedToppings.Any()) count++;
        if (SelectedDressing != null) count++;
        if (SelectedSide != null) count++;
        if (SelectedDrink != null) count++;
        return count;
    }

    public string GetSelectionSummary()
    {
        if (!IsValidSelection())
        {
            return "Complete all required fields";
        }
        return $"{SelectedBread}, {SelectedMeat}, {SelectedCheese}, {SelectedToppings.Count} topping(s), {SelectedDressing}";
    }

    public bool IsValidSelection()
    {
        return SelectedBread != null
            && SelectedMeat != null
            && SelectedCheese != null
            && SelectedToppings.Any()
            && SelectedDressing != null
            && SelectedSide != null
            && SelectedDrink != null;
    }

    public void AddToOrder()
    {
        if (IsValidSelection() && SelectedSide != null && SelectedDrink != null)
        {
            var meal = new MealDto
            {
                EntreeId = 0,
                SideId = SelectedSide.Id,
                DrinkId = SelectedDrink.Id
            };

            var toppingsText = string.Join(", ", SelectedToppings);
            OrderConfirmation = $"Custom Deli Sandwich: {SelectedBread} with {SelectedMeat}, {SelectedCheese}, {toppingsText}, and {SelectedDressing}. Side: {SelectedSide.SideName}";

            Console.WriteLine($"Deli Order: Bread={SelectedBread}, Meat={SelectedMeat}, Cheese={SelectedCheese}, Toppings={toppingsText}, Dressing={SelectedDressing}, SideId={meal.SideId}, DrinkId={meal.DrinkId}");

            ClearSelections();
        }
    }

    public void ClearOrderConfirmation()
    {
        OrderConfirmation = null;
    }

    private void ClearSelections()
    {
        SelectedBread = null;
        SelectedMeat = null;
        SelectedCheese = null;
        SelectedToppings.Clear();
        SelectedDressing = null;
        SelectedSide = null;
        SelectedDrink = null;
        ActiveTab = "sandwich";
    }
}
