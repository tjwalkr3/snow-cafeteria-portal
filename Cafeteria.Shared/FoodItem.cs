namespace Cafeteria.Shared;

public class FoodItem
{
    #region Properties
    public IReadOnlyDictionary<IngredientType, Ingredient>? Ingredients { get; }
    // TODO: add price calculator and properties for different pieces of price
    //       also add properties for name, description, image url, etc.
    #endregion

    #region Constructor
    public FoodItem(IReadOnlyDictionary<IngredientType, Ingredient>? ingredients = null)
    {
        Ingredients = ingredients;
    }
    #endregion
}
