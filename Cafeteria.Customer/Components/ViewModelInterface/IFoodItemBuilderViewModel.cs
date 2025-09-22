using Cafeteria.Shared;

namespace Cafeteria.Customer.Components.ViewModelInterfaces;

public interface IFoodItemBuilderViewModel
{
    FoodItem SelectedFoodItem { get; set; }
    List<IngredientType> AvailableIngredientTypes { get; set; }
    List<Ingredient> AvailableIngredients { get; set; }
    List<Ingredient> SelectedIngredients { get; set; }
    Dictionary<IngredientType, List<Ingredient>> IngredientsByType { get; set; }
    void SelectIngredient(Ingredient ingredient);
    void UnselectIngredient(Ingredient ingredient);
}