namespace Cafeteria.Shared;

public class Ingredient
{
    #region Properties
    public string Name { get; private set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; private set; }
    public IngredientType? Type { get; set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates an Ingredient object with a null IngredientType.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="imgUrl"></param>
    /// <param name="price"></param>
    public Ingredient(string name, string imgUrl, decimal price)
    {
        Name = name;
        Price = price;
        ImageUrl = imgUrl;
    }

    /// <summary>
    /// Creates an ingredient with an IngredientType by accepting parameters for type name and quantity of IngredientType.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="imgUrl"></param>
    /// <param name="ingredientType"></param>
    /// <param name="quantityOfType"></param>
    /// <param name="price"></param>
    public Ingredient(string name, string imgUrl, string ingredientType, int quantityOfType, decimal price)
    {
        Name = name;
        Price = price;
        Type = new IngredientType(ingredientType, quantityOfType);
        ImageUrl = imgUrl;
    }
    #endregion
}
