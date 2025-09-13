using Cafeteria.Shared;

namespace Cafeteria.Api.Tests;

public class FoodItemBuilderTests
{
    [Fact]
    public void Build_WithValidData_CreatesFoodItem()
    {
        var builder = new FoodItemBuilder();
        var foodItem = builder
            .SetName("Test Item")
            .SetImageUrl("test.jpg")
            .SetPrice(9.99m)
            .Build();

        Assert.Equal("Test Item", foodItem.Name);
        Assert.Equal("test.jpg", foodItem.ImageUrl);
        Assert.Equal(9.99m, foodItem.Price);
        Assert.Null(foodItem.Description);
        Assert.Null(foodItem.Ingredients);
    }

    [Fact]
    public void Build_WithoutName_ThrowsException()
    {
        var builder = new FoodItemBuilder();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.SetImageUrl("test.jpg").SetPrice(5.00m).Build());

        Assert.Equal("Name is required to build a FoodItem", exception.Message);
    }

    [Fact]
    public void Build_WithoutImageUrl_ThrowsException()
    {
        var builder = new FoodItemBuilder();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.SetName("Test").SetPrice(5.00m).Build());

        Assert.Equal("ImageUrl is required to build a FoodItem", exception.Message);
    }

    [Fact]
    public void Build_WithNegativePrice_ThrowsException()
    {
        var builder = new FoodItemBuilder();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            builder.SetName("Test").SetImageUrl("test.jpg").SetPrice(-1.00m).Build());

        Assert.Equal("Price cannot be negative", exception.Message);
    }

    [Fact]
    public void Reset_ClearsAllFields()
    {
        var builder = new FoodItemBuilder();
        builder.SetName("Test").SetImageUrl("test.jpg").SetPrice(5.00m).Reset();

        var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Equal("Name is required to build a FoodItem", exception.Message);
    }
}