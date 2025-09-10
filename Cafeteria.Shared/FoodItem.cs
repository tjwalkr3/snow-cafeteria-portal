namespace Cafeteria.Shared;

public class FoodItem
{
    #region Properties
    public IReadOnlyDictionary<IngredientType, Ingredient>? Ingredients { get; }
    #endregion

    #region Constructor
    public FoodItem(IReadOnlyDictionary<IngredientType, Ingredient>? ingredients = null)
    {
        Ingredients = ingredients;
    }
    #endregion
}
