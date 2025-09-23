using Cafeteria.Shared;
using Cafeteria.Customer.Components.Data;
using Cafeteria.Customer.Components.ViewModelInterfaces;

namespace Cafeteria.Customer.Components.ViewModels;
public class FoodItemBuilderVM : IFoodItemBuilderViewModel
{
    /// <summary>
    /// This constructor should be used only with dummy data/for testing
    /// </summary>
    public FoodItemBuilderVM()
    {
        SelectedFoodItem = DummyData.GetDummySandwich();
        AvailableIngredients = DummyData.GetIngredientList;
        IngredientsByType = GetIngredientsByType();

    }

    public FoodItemBuilderVM(FoodItem selectedItem)
    {
        SelectedFoodItem = selectedItem;
    }
    public FoodItem SelectedFoodItem { get; set; }
    public List<IngredientType> AvailableIngredientTypes { get; set; } = new List<IngredientType>();
    public List<Ingredient> AvailableIngredients { get; set; } = new List<Ingredient>();
    public List<Ingredient> SelectedIngredients { get; set; } = new List<Ingredient>();
    public Dictionary<IngredientType, List<Ingredient>> IngredientsByType { get; set; } = new Dictionary<IngredientType, List<Ingredient>>();

    public void SelectIngredient(Ingredient ingredient)
    {
        if (!SelectedIngredients.Contains(ingredient))
        {
            SelectedIngredients.Add(ingredient);
        }
    }

    public void UnselectIngredient(Ingredient ingredient)
    {
        if (SelectedIngredients.Contains(ingredient))
        {
            SelectedIngredients.Remove(ingredient);
        }
    }

    private Dictionary<IngredientType, List<Ingredient>> GetIngredientsByType()
    {
        Dictionary<IngredientType, List<Ingredient>> ingredients = new();

        // Get all distinct ingredient types from available ingredients
        var distinctTypes = AvailableIngredients
            .Where(i => i.Type != null)
            .Select(i => i.Type!)
            .GroupBy(t => t.Name)
            .Select(g => g.First()) // Take the first instance of each type name
            .ToList();

        AvailableIngredientTypes = distinctTypes;

        foreach (var type in AvailableIngredientTypes)
        {
            ingredients[type] = AvailableIngredients
                .Where(i => i.Type != null && i.Type.Name == type.Name)
                .ToList();
        }

        return ingredients;
    }
}