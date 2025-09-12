namespace Cafeteria.Customer.Components.Data;

using Cafeteria.Shared;

public static class DummyData {

    public static IngredientType Bread = new IngredientType("Bread");
    public static Ingredient TestBread = new Ingredient("Wheat Bread", "https://picsum.photos/id/98/150/150", 0m);

    public static IngredientType Meat = new IngredientType("Meat");
    public static Ingredient TestMeat1 = new Ingredient("Roast Beef", "https://picsum.photos/id/200/150/150", 0m);
    public static Ingredient TestMeat2 = new Ingredient("Unseasoned Roast Beef", "https://picsum.photos/id/200/150/150", 0m);


    public static IngredientType Cheese = new IngredientType("Cheese");
    public static Ingredient TestCheese1 = new Ingredient("Mozzarella Cheese", "https://picsum.photos/id/835/150/150", 0m);
    public static Ingredient TestCheese2 = new Ingredient("Gouda Cheese", "https://picsum.photos/id/835/150/150", 0m);

    public static IngredientType Other = new IngredientType("Other");
    public static Ingredient TestLettuce = new Ingredient("Lettuce", "https://picsum.photos/id/189/150/150", 0m);
    public static Ingredient TestOnion = new Ingredient("Onion", "https://picsum.photos/id/292/150/150", 0m);

    public static IReadOnlyDictionary<IngredientType, Ingredient> NormalSandwichIngredients = new Dictionary<IngredientType, Ingredient>()
    {
        { Bread, TestBread },
        { Meat, TestMeat1 },
        { Cheese, TestCheese1 },
        { Other, TestLettuce },
        { Other, TestOnion }
    };

    public static FoodItem TestSandwich = new FoodItem(
        name: "Test Sandwich",
        description: "A delicious test sandwich.",
        price: 5.99m,
        imgUrl: "https://picsum.photos/id/488/150/150" // this salad image was the closest I could find to a sandwich on picsum
    );

}