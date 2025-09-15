using Cafeteria.Shared;

namespace Cafeteria.Api.Tests;

public class FoodItemBuilderServiceTests
{
    [Fact]
    public void Build_WithValidData_CreatesFoodItem()
    {
        var builder = new FoodItemBuilderService();

        var foodItem = builder
            .SetName("Pizza")
            .SetImageUrl("pizza.jpg")
            .SetPrice(12.99m)
            .Build();

        Assert.Equal("Pizza", foodItem.Name);
        Assert.Equal("pizza.jpg", foodItem.ImageUrl);
        Assert.Equal(12.99m, foodItem.Price);
    }

    [Fact]
    public void SetName_WithEmptyString_ThrowsArgumentException()
    {
        var builder = new FoodItemBuilderService();

        var exception = Assert.Throws<ArgumentException>(() => builder.SetName(""));

        Assert.Equal("Name cannot be null or empty (Parameter 'name')", exception.Message);
    }

    [Fact]
    public void SetPrice_WithNegativeValue_ThrowsArgumentException()
    {
        var builder = new FoodItemBuilderService();

        var exception = Assert.Throws<ArgumentException>(() => builder.SetPrice(-5.00m));

        Assert.Equal("Price cannot be negative (Parameter 'price')", exception.Message);
    }

    [Fact]
    public void Reset_ClearsAllFields_RequiresBuildValidation()
    {
        var builder = new FoodItemBuilderService();

        builder.SetName("Test").SetImageUrl("test.jpg").Reset();

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Equal("Name is required to build a FoodItem", exception.Message);
    }

    [Fact]
    public void AddIngredient_AddsToCollection()
    {
        var builder = new FoodItemBuilderService();
        var ingredientType = new IngredientType("Cheese");
        var ingredient = new Ingredient("Mozzarella", "cheese.jpg", 2.50m);

        var foodItem = builder
            .SetName("Pizza")
            .SetImageUrl("pizza.jpg")
            .SetPrice(10.00m)
            .AddIngredient(ingredientType, ingredient)
            .Build();

        Assert.NotNull(foodItem.Ingredients);
        Assert.Single(foodItem.Ingredients);
    }
}