using System.Collections.Immutable;

namespace Cafeteria.Shared;

public class FoodItemBuilder
{
    private string _name = string.Empty;
    private string? _description;
    private string _imageUrl = string.Empty;
    private decimal _price;
    private Dictionary<IngredientType, Ingredient>? _ingredients;

    public FoodItemBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    public FoodItemBuilder SetDescription(string? description)
    {
        _description = description;
        return this;
    }

    public FoodItemBuilder SetImageUrl(string imageUrl)
    {
        _imageUrl = imageUrl;
        return this;
    }

    public FoodItemBuilder SetPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public FoodItemBuilder AddIngredient(IngredientType ingredientType, Ingredient ingredient)
    {
        _ingredients ??= new Dictionary<IngredientType, Ingredient>();
        _ingredients[ingredientType] = ingredient;
        return this;
    }

    public FoodItemBuilder SetIngredients(Dictionary<IngredientType, Ingredient>? ingredients)
    {
        _ingredients = ingredients;
        return this;
    }

    public FoodItemBuilder Reset()
    {
        _name = string.Empty;
        _description = null;
        _imageUrl = string.Empty;
        _price = 0;
        _ingredients = null;
        return this;
    }

    public FoodItem Build()
    {
        if (string.IsNullOrEmpty(_name))
            throw new InvalidOperationException("Name is required to build a FoodItem");

        if (string.IsNullOrEmpty(_imageUrl))
            throw new InvalidOperationException("ImageUrl is required to build a FoodItem");

        if (_price < 0)
            throw new InvalidOperationException("Price cannot be negative");

        var readOnlyIngredients = _ingredients?.ToImmutableDictionary();

        return new FoodItem(_name, _imageUrl, _price, _description, readOnlyIngredients);
    }
}