namespace Cafeteria.Shared;

public class FoodItem
{
    #region Public Properties
    public string Name { get; init; }
    public string? Description { get; init; }
    public string ImageUrl { get; init; }
    public IReadOnlyDictionary<IngredientType, Ingredient>? Ingredients { get; }
    public decimal Price
    {
        get
        {
            if (extrasPrice is not null)
            {
                return basePrice + extrasPrice.Value;
            }
            else return basePrice;
        }
    }
    #endregion

    #region Private Properties
    private decimal basePrice { get; init; }
    private decimal? extrasPrice { get; init; }
    #endregion

    #region Constructor
    public FoodItem(string name, string imgUrl, decimal itemPrice, decimal extrasPrice, string? description = null, IReadOnlyDictionary<IngredientType, Ingredient>? ingredients = null)
    {
        Name = name;
        Description = description;
        ImageUrl = imgUrl;
        basePrice = itemPrice;
        this.extrasPrice = extrasPrice;
        Ingredients = ingredients;
    }
    #endregion
}
