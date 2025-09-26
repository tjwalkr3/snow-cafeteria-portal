using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Cafeteria.Api.Services;

namespace Cafeteria.Api.Tests;

public class MenuServiceTests
{
    private readonly string _connectionString;

    public MenuServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<MenuServiceTests>()
            .AddEnvironmentVariables()
            .Build();

        _connectionString = configuration["DB_CONN"] ??
            configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("No database connection string found in configuration or user secrets");
    }

    [Fact]
    public async Task GetIngredientsForType_ShouldReturnIngredients_WhenTypeExists()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var menuService = new MenuService(connection);

        var ingredients = await menuService.GetIngredientsForType(1);

        Assert.NotNull(ingredients);
        var ingredientList = ingredients.ToList();
        foreach (var ingredient in ingredientList)
        {
            Assert.True(ingredient.Id > 0);
            Assert.False(string.IsNullOrEmpty(ingredient.IngredientName));
        }
    }

    [Fact]
    public async Task GetIngredientTypesForFoodItem_ShouldReturnIngredientTypes_WhenFoodItemExists()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var menuService = new MenuService(connection);

        var ingredientTypes = await menuService.GetIngredientTypesForFoodItem(1);

        Assert.NotNull(ingredientTypes);
        var typesList = ingredientTypes.ToList();
        foreach (var type in typesList)
        {
            Assert.True(type.Id > 0);
            Assert.False(string.IsNullOrEmpty(type.TypeName));
            Assert.True(type.Quantity > 0);
        }
    }

    [Fact]
    public async Task GetDefaultIngredientsForFoodItem_ShouldReturnIngredients_WhenFoodItemExists()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        var menuService = new MenuService(connection);

        var defaultIngredients = await menuService.GetDefaultIngredientsForFoodItem(1);

        Assert.NotNull(defaultIngredients);
        var ingredientList = defaultIngredients.ToList();
        foreach (var ingredient in ingredientList)
        {
            Assert.True(ingredient.Id > 0);
            Assert.False(string.IsNullOrEmpty(ingredient.IngredientName));
        }
    }
}