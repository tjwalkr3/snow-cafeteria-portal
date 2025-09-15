namespace Cafeteria.Customer.Components.Data;
using Cafeteria.Shared;

public static class DummyData {

    public static FoodItem GetDummySandwich()
    {
        IReadOnlyDictionary<IngredientType, Ingredient> NormalSandwichIngredients = new Dictionary<IngredientType, Ingredient>()
        {
            { new IngredientType("Bread"), new Ingredient("Wheat Bread", "https://picsum.photos/id/98/150/150", 0m) },
            { new IngredientType("Meat"), new Ingredient("Roast Beef", "https://picsum.photos/id/200/150/150", 0m) },
            { new IngredientType("Cheese"), new Ingredient("Gouda Cheese", "https://picsum.photos/id/835/150/150", 0m) },
            { new IngredientType("Other"), new Ingredient("Lettuce", "https://picsum.photos/id/189/150/150", 0m) },
            { new IngredientType("Other"), new Ingredient("Onion", "https://picsum.photos/id/292/150/150", 0m) }
        };

        return new FoodItem(
            name: "Test Sandwich",
            description: "A delicious test sandwich.",
            price: 5.99m,
            imgUrl: "https://picsum.photos/id/488/150/150", 
            ingredients: NormalSandwichIngredients
        );
    }

    public static FoodItem GetDummySandwichWithExtraCheese()
    {
        IReadOnlyDictionary<IngredientType, Ingredient> NormalSandwichIngredients = new Dictionary<IngredientType, Ingredient>()
        {
            { new IngredientType("Bread"), new Ingredient("Marble Rye Bread", "https://picsum.photos/id/98/150/150", 0m) },
            { new IngredientType("Meat"), new Ingredient("Grilled Chicken", "https://picsum.photos/id/200/150/150", 0m) },
            { new IngredientType("Cheese"), new Ingredient("Gouda Cheese", "https://picsum.photos/id/835/150/150", 0m) },
            { new IngredientType("Cheese"), new Ingredient("Mozzarella Cheese", "https://picsum.photos/id/835/150/150", 0m) },
            { new IngredientType("Other"), new Ingredient("Alfalfa Sprouts", "https://picsum.photos/id/400/150/150", 0m) },
        };
        
        return new FoodItem(
            name: "Test Sandwich With Extra Cheese",
            description: "A delicious test sandwich with mozzarella and gouda cheese!",
            price: 5.99m,
            imgUrl: "https://picsum.photos/id/488/150/150", 
            ingredients: NormalSandwichIngredients
        );
    }

    public static FoodItem GetDummyHamburger()
    { 
        return new FoodItem(
            name: "Test Hamburger",
            description: "Hamburgers have default ingredients and don't need to go through the food builder.",
            price: 5.99m,
            imgUrl: "https://picsum.photos/id/488/150/150" 
        );
    }
}