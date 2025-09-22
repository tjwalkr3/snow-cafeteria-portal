namespace Cafeteria.Customer.Components.Data;

using System;
using Cafeteria.Shared;

public static class DummyData
{
    #region ============================ Static List Creation =================================
    public static readonly List<IngredientType> GetIngredientTypeList = new()
    {
        CreateBreadType(),
        CreateMeatType(),
        CreateCheeseType(),
        CreateOtherType()
    };

    public static readonly List<Ingredient> GetIngredientList = new()
    {
        CreateWheatBread(),
        CreateOnion(),
        CreateLettuce(),
        CreateGoudaCheese(),
        CreateRoastBeef(),
        CreateMarbleRyeBread(),
        CreateGrilledChicken(),
        CreateMozzarellaCheeseExtra(),
        CreateAlfalfaSprouts()
    };

    public static readonly List<FoodItem> GetFoodItemList = new()
    {
        GetDummySandwich(),
        GetDummySandwichWithExtraCheese(),
        GetDummyHamburger()
    };
    #endregion ============================================================================




    #region =========================== Ingredients Creation ==============================
    public static Ingredient CreateWheatBread()
    {
        var ingredient = new Ingredient("Wheat Bread", "https://picsum.photos/id/98/150/150", 0m);
        ingredient.Type = CreateBreadType();
        return ingredient;
    }

    public static Ingredient CreateOnion()
    {
        var ingredient = new Ingredient("Onion", "https://picsum.photos/id/292/150/150", 0m);
        ingredient.Type = CreateOtherType();
        return ingredient;
    }

    public static Ingredient CreateLettuce()
    {
        var ingredient = new Ingredient("Lettuce", "https://picsum.photos/id/189/150/150", 0m);
        ingredient.Type = CreateOtherType();
        return ingredient;
    }

    public static Ingredient CreateGoudaCheese()
    {
        var ingredient = new Ingredient("Gouda Cheese", "https://picsum.photos/id/835/150/150", 0m);
        ingredient.Type = CreateCheeseType();
        return ingredient;
    }

    public static Ingredient CreateRoastBeef()
    {
        var ingredient = new Ingredient("Roast Beef", "https://picsum.photos/id/200/150/150", 0m);
        ingredient.Type = CreateMeatType();
        return ingredient;
    }

    public static Ingredient CreateMarbleRyeBread()
    {
        var ingredient = new Ingredient("Marble Rye Bread", "https://picsum.photos/id/98/150/150", 0m);
        ingredient.Type = CreateBreadType();
        return ingredient;
    }

    public static Ingredient CreateGrilledChicken()
    {
        var ingredient = new Ingredient("Grilled Chicken", "https://picsum.photos/id/200/150/150", 0m);
        ingredient.Type = CreateMeatType();
        return ingredient;
    }

    public static Ingredient CreateMozzarellaCheeseExtra()
    {
        var ingredient = new Ingredient("Mozzarella Cheese", "https://picsum.photos/id/835/150/150", 0.79m);
        ingredient.Type = CreateCheeseType();
        return ingredient;
    }

    public static Ingredient CreateAlfalfaSprouts()
    {
        var ingredient = new Ingredient("Alfalfa Sprouts", "https://picsum.photos/id/400/150/150", 0m);
        ingredient.Type = CreateOtherType();
        return ingredient;
    }
    #endregion ==========================================================================



    #region ========================= Ingredient Type Creation ============================
    public static IngredientType CreateBreadType()
    {
        return new IngredientType("Bread", 1);
    }

    public static IngredientType CreateMeatType()
    {
        return new IngredientType("Meat", 1);
    }

    public static IngredientType CreateCheeseType()
    {
        return new IngredientType("Cheese", 1);
    }

    public static IngredientType CreateOtherType()
    {
        return new IngredientType("Other", 3);
    }
    #endregion ============================================================================



    #region =========================== Food Item Creation ================================
    public static FoodItem GetDummySandwich()
    {
        var breadType = CreateBreadType();
        var meatType = CreateMeatType();
        var cheeseType = CreateCheeseType();
        var otherType1 = CreateOtherType();
        var otherType2 = CreateOtherType();

        IReadOnlyDictionary<IngredientType, Ingredient> normalSandwichIngredients = new Dictionary<IngredientType, Ingredient>()
        {
            { breadType, CreateWheatBread() },
            { meatType, CreateRoastBeef() },
            { cheeseType, CreateGoudaCheese() },
            { otherType1, CreateLettuce() },
            { otherType2, CreateOnion() }
        };

        return new FoodItem(
            name: "Test Sandwich",
            description: "A delicious test sandwich.",
            price: 5.99m,
            imgUrl: "https://picsum.photos/id/488/150/150",
            ingredients: normalSandwichIngredients
        );
    }

    public static FoodItem GetDummySandwichWithExtraCheese()
    {
        var breadType = CreateBreadType();
        var meatType = CreateMeatType();
        var cheeseType1 = CreateCheeseType();
        var cheeseType2 = CreateCheeseType();
        var otherType = CreateOtherType();

        IReadOnlyDictionary<IngredientType, Ingredient> sandwichIngredientsWithExtraCheese = new Dictionary<IngredientType, Ingredient>()
        {
            { breadType, CreateMarbleRyeBread() },
            { meatType, CreateGrilledChicken() },
            { cheeseType1, CreateGoudaCheese() },
            { cheeseType2, CreateMozzarellaCheeseExtra() }, // second piece of cheese adds $0.79
            { otherType, CreateAlfalfaSprouts() },
        };

        return new FoodItem(
            name: "Test Sandwich With Extra Cheese",
            description: "A delicious test sandwich with mozzarella and gouda cheese!",
            price: 5.99m,
            imgUrl: "https://picsum.photos/id/488/150/150",
            ingredients: sandwichIngredientsWithExtraCheese
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
    #endregion ============================================================================

}