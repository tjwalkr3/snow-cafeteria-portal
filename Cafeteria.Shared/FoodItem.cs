namespace Cafeteria.Shared;

public class FoodItem
{
    #region Public Properties
    public string Name { get; init; }
    public string? Description { get; init; }
    public string ImageUrl { get; init; }
    public IReadOnlyDictionary<IngredientType, Ingredient>? Ingredients { get; }
    public decimal Price { get; init; }
    #endregion


    #region Constructor
    public FoodItem(string name, string imgUrl, decimal price, string? description = null, IReadOnlyDictionary<IngredientType, Ingredient>? ingredients = null)
    {
        Name = name;
        Description = description;
        ImageUrl = imgUrl;
        Ingredients = ingredients;
    }
    #endregion
}
