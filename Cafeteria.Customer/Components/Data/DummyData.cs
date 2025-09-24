namespace Cafeteria.Customer.Components.Data;

using Cafeteria.Shared.DTOs;

public static class DummyData
{
    #region ============================ Static List Creation =================================
    public static readonly List<WeekDayDto> GetWeekDayList = new()
    {
        CreateMonday(),
        CreateFriday()
    };

    public static readonly List<CafeteriaLocationDto> GetLocationList = new()
    {
        CreateMainCampusLocation()
    };

    public static readonly List<LocationBusinessHoursDto> GetBusinessHoursList = new()
    {
        CreateBusinessHours(1, 1, new TimeOnly(8, 0), new TimeOnly(18, 0)),   // Monday
        CreateBusinessHours(2, 1, new TimeOnly(8, 0), new TimeOnly(16, 0))    // Friday
    };

    public static readonly List<StationDto> GetStationList = new()
    {
        CreateSandwichStation(),
        CreateSaladStation()
    };

    public static readonly List<IngredientTypeDto> GetIngredientTypeList = new()
    {
        CreateBreadType(),
        CreateMeatType(),
        CreateVegetableType()
    };

    public static readonly List<IngredientDto> GetIngredientList = new()
    {
        CreateWheatBread(),
        CreateTurkey(),
        CreateLettuce(),
        CreateTomato()
    };

    public static readonly List<FoodItemDto> GetFoodItemList = new()
    {
        CreateTurkeysandwich(),
        CreateGreenSalad()
    };

    public static readonly List<IngredientIngredientTypeDto> GetIngredientIngredientTypeList = new()
    {
        CreateIngredientIngredientType(1, 1, 1),   // Wheat Bread -> Bread Type
        CreateIngredientIngredientType(2, 2, 2),   // Turkey -> Meat Type
        CreateIngredientIngredientType(3, 3, 3),   // Lettuce -> Vegetable Type
        CreateIngredientIngredientType(4, 4, 3)    // Tomato -> Vegetable Type
    };

    public static readonly List<FoodItemIngredientTypeDto> GetFoodItemIngredientTypeList = new()
    {
        // Turkey Sandwich - requires bread, meat, vegetable
        CreateFoodItemIngredientType(1, 1, 1), // Turkey Sandwich -> Bread Type
        CreateFoodItemIngredientType(2, 1, 2), // Turkey Sandwich -> Meat Type
        CreateFoodItemIngredientType(3, 1, 3), // Turkey Sandwich -> Vegetable Type
        
        // Green Salad - requires vegetables only
        CreateFoodItemIngredientType(4, 2, 3)  // Green Salad -> Vegetable Type
    };

    public static readonly List<FoodBuilderItemIngredientDto> GetFoodBuilderItemIngredientList = new()
    {
        // Turkey Sandwich default ingredients
        CreateFoodBuilderItemIngredient(1, 1, 1),  // Turkey Sandwich -> Wheat Bread
        CreateFoodBuilderItemIngredient(2, 1, 2),  // Turkey Sandwich -> Turkey
        CreateFoodBuilderItemIngredient(3, 1, 3),  // Turkey Sandwich -> Lettuce
        
        // Green Salad default ingredients
        CreateFoodBuilderItemIngredient(4, 2, 3),  // Green Salad -> Lettuce
        CreateFoodBuilderItemIngredient(5, 2, 4)   // Green Salad -> Tomato
    };
    #endregion ============================================================================

    #region ============================ Weekday Creation ==================================
    public static WeekDayDto CreateMonday() => new() { Id = 1, WeekdayName = "Monday" };
    public static WeekDayDto CreateFriday() => new() { Id = 5, WeekdayName = "Friday" };
    #endregion ============================================================================

    #region ========================= Location Creation ===================================
    public static CafeteriaLocationDto CreateMainCampusLocation() => new()
    {
        Id = 1,
        LocationName = "Main Campus Cafeteria",
        LocationDescription = "The primary dining facility serving fresh meals throughout the day",
        Address = "100 University Way, Campus Center Building"
    };
    #endregion ============================================================================

    #region ======================= Business Hours Creation =============================
    public static LocationBusinessHoursDto CreateBusinessHours(int id, int locationId, TimeOnly openTime, TimeOnly closeTime) => new()
    {
        Id = id,
        LocationId = locationId,
        WeekdayId = ((id - 1) % 7) + 1, // Cycles through weekdays 1-7
        OpenTime = openTime,
        CloseTime = closeTime
    };
    #endregion ============================================================================

    #region =========================== Station Creation =================================
    public static StationDto CreateSandwichStation() => new()
    {
        Id = 1,
        LocationId = 1,
        StationName = "Sandwich Station",
        StationDescription = "Fresh made-to-order sandwiches"
    };

    public static StationDto CreateSaladStation() => new()
    {
        Id = 2,
        LocationId = 1,
        StationName = "Salad Bar",
        StationDescription = "Fresh salads and healthy options"
    };
    #endregion ============================================================================

    #region ========================= Ingredient Type Creation ============================
    public static IngredientTypeDto CreateBreadType() => new() { Id = 1, TypeName = "Bread", Quantity = 1 };
    public static IngredientTypeDto CreateMeatType() => new() { Id = 2, TypeName = "Meat", Quantity = 1 };
    public static IngredientTypeDto CreateVegetableType() => new() { Id = 3, TypeName = "Vegetable", Quantity = 2 };
    #endregion ============================================================================

    #region =========================== Ingredients Creation ==============================
    public static IngredientDto CreateWheatBread() => new()
    {
        Id = 1,
        IngredientName = "Wheat Bread",
        ImageUrl = "https://picsum.photos/id/98/150/150",
        IngredientPrice = 0m
    };

    public static IngredientDto CreateTurkey() => new()
    {
        Id = 2,
        IngredientName = "Sliced Turkey",
        ImageUrl = "https://picsum.photos/id/429/150/150",
        IngredientPrice = 2.50m
    };

    public static IngredientDto CreateLettuce() => new()
    {
        Id = 3,
        IngredientName = "Fresh Lettuce",
        ImageUrl = "https://picsum.photos/id/550/150/150",
        IngredientPrice = 0m
    };

    public static IngredientDto CreateTomato() => new()
    {
        Id = 4,
        IngredientName = "Sliced Tomato",
        ImageUrl = "https://picsum.photos/id/189/150/150",
        IngredientPrice = 0.25m
    };
    #endregion ============================================================================

    #region =========================== Food Item Creation ================================
    public static FoodItemDto CreateTurkeysandwich() => new()
    {
        Id = 1,
        StationId = 1,
        ItemDescription = "Turkey sandwich with lettuce on wheat bread",
        ImageUrl = "https://picsum.photos/id/488/150/150",
        ItemPrice = 6.50m
    };

    public static FoodItemDto CreateGreenSalad() => new()
    {
        Id = 2,
        StationId = 2,
        ItemDescription = "Fresh green salad with lettuce and tomato",
        ImageUrl = "https://picsum.photos/id/550/150/150",
        ItemPrice = 4.95m
    };
    #endregion ============================================================================

    #region ====================== Relational Table Creation ============================
    public static IngredientIngredientTypeDto CreateIngredientIngredientType(int id, int ingredientId, int ingredientTypeId) => new()
    {
        Id = id,
        IngredientId = ingredientId,
        IngredientTypeId = ingredientTypeId
    };

    public static FoodItemIngredientTypeDto CreateFoodItemIngredientType(int id, int foodItemId, int ingredientTypeId) => new()
    {
        Id = id,
        FoodItemId = foodItemId,
        IngredientTypeId = ingredientTypeId
    };

    public static FoodBuilderItemIngredientDto CreateFoodBuilderItemIngredient(int id, int foodItemId, int ingredientId) => new()
    {
        Id = id,
        FoodItemId = foodItemId,
        IngredientId = ingredientId
    };
    #endregion ============================================================================
}