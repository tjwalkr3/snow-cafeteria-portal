namespace Cafeteria.Shared.Data;

using Cafeteria.Shared.DTOs;

public static class DummyData
{
    #region ============================ Static List Creation =================================
    public static readonly List<WeekDayDtoOld> GetWeekDayList = new()
    {
        CreateMonday(),
        CreateFriday()
    };

    public static readonly List<LocationDtoOld> GetLocationList = new()
    {
        CreateBadgerDenLocation(),
        CreateBustersBistroLocation()
    };

    public static readonly List<LocationBusinessHoursDtoOld> GetBusinessHoursList = new()
    {
        CreateBusinessHours(1, 1, new TimeOnly(8, 0), new TimeOnly(18, 0)),   // Monday
        CreateBusinessHours(2, 1, new TimeOnly(8, 0), new TimeOnly(16, 0))    // Friday
    };

    public static readonly List<StationDtoOld> GetStationList = new()
    {
        CreateSandwichStation(),
        CreateSaladStation()
    };

    public static readonly List<IngredientTypeDtoOld> GetIngredientTypeList = new()
    {
        CreateBreadType(),
        CreateMeatType(),
        CreateVegetableType()
    };

    public static readonly List<IngredientDtoOld> GetIngredientList = new()
    {
        CreateWheatBread(),
        CreateTurkey(),
        CreateLettuce(),
        CreateTomato()
    };

    public static readonly List<FoodItemDtoOld> GetFoodItemList = new()
    {
        CreateTurkeysandwich(),
        CreateGreenSalad()
    };


    #endregion ============================================================================

    #region ============================ Weekday Creation ==================================
    public static WeekDayDtoOld CreateMonday() => new() { Id = 1, WeekdayName = "Monday" };
    public static WeekDayDtoOld CreateFriday() => new() { Id = 5, WeekdayName = "Friday" };
    #endregion ============================================================================

    #region ========================= Location Creation ===================================
    public static LocationDtoOld CreateBadgerDenLocation() => new()
    {
        Id = 1,
        Name = "Badger Den",
        Description = "Campus dining location in the GSC",
        Address = "GSC Cafeteria on the Ground Floor"
    };

    public static LocationDtoOld CreateBustersBistroLocation() => new()
    {
        Id = 2,
        Name = "Buster's Bistro",
        Description = "Library dining location",
        Address = "Karen H. Huntsman Library Gallery"
    };
    #endregion ============================================================================

    #region ======================= Business Hours Creation =============================
    public static LocationBusinessHoursDtoOld CreateBusinessHours(int id, int locationId, TimeOnly openTime, TimeOnly closeTime) => new()
    {
        Id = id,
        LocationId = locationId,
        WeekdayId = ((id - 1) % 7) + 1, // Cycles through weekdays 1-7
        OpenTime = openTime,
        CloseTime = closeTime
    };
    #endregion ============================================================================

    #region =========================== Station Creation =================================
    public static StationDtoOld CreateSandwichStation() => new()
    {
        Id = 1,
        LocationId = 1,
        StationName = "Sandwich Station",
        StationDescription = "Fresh made-to-order sandwiches"
    };

    public static StationDtoOld CreateSaladStation() => new()
    {
        Id = 2,
        LocationId = 1,
        StationName = "Salad Bar",
        StationDescription = "Fresh salads and healthy options"
    };
    #endregion ============================================================================

    #region ========================= Ingredient Type Creation ============================
    public static IngredientTypeDtoOld CreateBreadType() => new() { Id = 1, TypeName = "Bread", Quantity = 1 };
    public static IngredientTypeDtoOld CreateMeatType() => new() { Id = 2, TypeName = "Meat", Quantity = 1 };
    public static IngredientTypeDtoOld CreateVegetableType() => new() { Id = 3, TypeName = "Vegetable", Quantity = 2 };
    #endregion ============================================================================

    #region =========================== Ingredients Creation ==============================
    public static IngredientDtoOld CreateWheatBread() => new()
    {
        Id = 1,
        IngredientName = "Wheat Bread",
        ImageUrl = "https://picsum.photos/id/98/150/150",
        IngredientPrice = 0m
    };

    public static IngredientDtoOld CreateTurkey() => new()
    {
        Id = 2,
        IngredientName = "Sliced Turkey",
        ImageUrl = "https://picsum.photos/id/429/150/150",
        IngredientPrice = 2.50m
    };

    public static IngredientDtoOld CreateLettuce() => new()
    {
        Id = 3,
        IngredientName = "Fresh Lettuce",
        ImageUrl = "https://picsum.photos/id/550/150/150",
        IngredientPrice = 0m
    };

    public static IngredientDtoOld CreateTomato() => new()
    {
        Id = 4,
        IngredientName = "Sliced Tomato",
        ImageUrl = "https://picsum.photos/id/189/150/150",
        IngredientPrice = 0.25m
    };
    #endregion ============================================================================

    #region =========================== Food Item Creation ================================
    public static FoodItemDtoOld CreateTurkeysandwich() => new()
    {
        Id = 1,
        StationId = 1,
        ItemDescription = "Turkey sandwich with lettuce on wheat bread",
        ImageUrl = "https://picsum.photos/id/488/150/150",
        ItemPrice = 6.50m
    };

    public static FoodItemDtoOld CreateGreenSalad() => new()
    {
        Id = 2,
        StationId = 2,
        ItemDescription = "Fresh green salad with lettuce and tomato",
        ImageUrl = "https://picsum.photos/id/550/150/150",
        ItemPrice = 4.95m
    };
    #endregion ============================================================================
}