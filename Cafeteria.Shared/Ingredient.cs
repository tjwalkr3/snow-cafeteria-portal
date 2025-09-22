namespace Cafeteria.Shared;

public class Ingredient
{
    #region Properties
    public string Name { get; private set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; private set; }
    public IngredientType Type { get; set; }
    #endregion

    #region Constructor
    public Ingredient(string name, string imgUrl, string ingredientType, int quantityOfType, decimal price)
    {
        Name = name;
        Price = price;
        Type = new IngredientType(ingredientType, quantityOfType);
        ImageUrl = imgUrl;
    }
    #endregion
}
