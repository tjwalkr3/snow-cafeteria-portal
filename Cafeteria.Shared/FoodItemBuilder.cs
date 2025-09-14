namespace Cafeteria.Shared;

public class FoodItemBuilderService
{
    private string? _name;
    private string? _description;
    private string? _imageUrl;
    private decimal _price;
    private Dictionary<IngredientType, Ingredient>? _ingredients;

    public FoodItemBuilderService SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        
        _name = name;
        return this;
    }

    public FoodItemBuilderService SetDescription(string? description)
    {
        _description = description;
        return this;
    }

    public FoodItemBuilderService SetImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be null or empty", nameof(imageUrl));
        
        _imageUrl = imageUrl;
        return this;
    }

    public FoodItemBuilderService SetPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        
        _price = price;
        return this;
    }

    public FoodItemBuilderService AddIngredient(IngredientType type, Ingredient ingredient)
    {
        _ingredients ??= new Dictionary<IngredientType, Ingredient>();
        _ingredients[type] = ingredient;
        return this;
    }

    public FoodItem Build()
    {
        if (string.IsNullOrWhiteSpace(_name))
            throw new InvalidOperationException("Name is required to build a FoodItem");
        
        if (string.IsNullOrWhiteSpace(_imageUrl))
            throw new InvalidOperationException("Image URL is required to build a FoodItem");

        return new FoodItem(
            name: _name,
            imgUrl: _imageUrl,
            price: _price,
            description: _description,
            ingredients: _ingredients?.AsReadOnly()
        );
    }

    public FoodItemBuilderService Reset()
    {
        _name = null;
        _description = null;
        _imageUrl = null;
        _price = 0;
        _ingredients = null;
        return this;
    }
}
