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

    public static readonly List<LocationDto> GetLocationList = new()
    {
        CreateBadgerDenLocation(),
        CreateBustersBistroLocation()
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
        CreateGreenSalad(),
        CreateHamSandwich(),
        CreateVeggieSandwich(),
        CreateCaesarSalad(),
        CreateCobbSalad(),
        CreateCheeseburger(),
        CreateGrilledChicken(),
        CreateFrenchFries()
    };


    #endregion ============================================================================

    #region ============================ Weekday Creation ==================================
    public static WeekDayDto CreateMonday() => new() { Id = 1, WeekdayName = "Monday" };
    public static WeekDayDto CreateFriday() => new() { Id = 5, WeekdayName = "Friday" };
    #endregion ============================================================================

    #region ========================= Location Creation ===================================
    public static LocationDto CreateBadgerDenLocation() => new()
    {
        Id = 1,
        Name = "Badger Den",
        Description = "Campus dining location in the GSC",
        Address = "GSC Cafeteria on the Ground Floor"
    };

    public static LocationDto CreateBustersBistroLocation() => new()
    {
        Id = 2,
        Name = "Buster's Bistro",
        Description = "Library dining location",
        Address = "Karen H. Huntsman Library Gallery"
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

    public static StationDto CreateGrillStation() => new()
    {
        Id = 3,
        LocationId = 2,
        StationName = "Grill Station",
        StationDescription = "Burgers, fries, and more"
    };
    #endregion ============================================================================

    #region ========================= Ingredient Type Creation ============================
    public static IngredientTypeDto CreateBreadType() => new() { Id = 1, TypeName = "Bread", Quantity = 1 };
    public static IngredientTypeDto CreateMeatType() => new() { Id = 2, TypeName = "Meat", Quantity = 1 };
    public static IngredientTypeDto CreateVegetableType() => new() { Id = 3, TypeName = "Vegetable", Quantity = 3 };
    private static IngredientTypeDto CreateDressingType() => new() { Id = 4, TypeName = "Dressing", Quantity = 1 };

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
        IngredientPrice = 0m
    };

    public static IngredientDto CreateRanchDressing() => new()
    {
        Id = 5,
        IngredientName = "Ranch Dressing",
        ImageUrl = "https://picsum.photos/id/292/150/150",
        IngredientPrice = 0m
    };

    public static IngredientDto CreateItalianDressing() => new()
    {
        Id = 5,
        IngredientName = "Italian Dressing",
        ImageUrl = "https://picsum.photos/id/292/150/150",
        IngredientPrice = 0m
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

    // Additional Sandwich Station Items
    public static FoodItemDto CreateHamSandwich() => new()
    {
        Id = 3,
        StationId = 1,
        ItemDescription = "Ham and cheese sandwich on sourdough bread",
        ImageUrl = "https://picsum.photos/id/490/150/150",
        ItemPrice = 6.25m
    };

    public static FoodItemDto CreateVeggieSandwich() => new()
    {
        Id = 4,
        StationId = 1,
        ItemDescription = "Vegetarian sandwich with avocado and sprouts",
        ImageUrl = "https://picsum.photos/id/491/150/150",
        ItemPrice = 5.75m
    };

    // Additional Salad Bar Items
    public static FoodItemDto CreateCaesarSalad() => new()
    {
        Id = 5,
        StationId = 2,
        ItemDescription = "Classic Caesar salad with croutons and parmesan",
        ImageUrl = "https://picsum.photos/id/551/150/150",
        ItemPrice = 5.50m
    };

    public static FoodItemDto CreateCobbSalad() => new()
    {
        Id = 6,
        StationId = 2,
        ItemDescription = "Cobb salad with bacon, eggs, and blue cheese",
        ImageUrl = "https://picsum.photos/id/552/150/150",
        ItemPrice = 6.75m
    };

    // Grill Station Items
    public static FoodItemDto CreateCheeseburger() => new()
    {
        Id = 7,
        StationId = 3,
        ItemDescription = "Classic cheeseburger with lettuce and tomato",
        ImageUrl = "https://picsum.photos/id/600/150/150",
        ItemPrice = 8.50m
    };

    public static FoodItemDto CreateGrilledChicken() => new()
    {
        Id = 8,
        StationId = 3,
        ItemDescription = "Grilled chicken breast with seasoned vegetables",
        ImageUrl = "https://picsum.photos/id/101/150/150",
        ItemPrice = 9.25m
    };

    public static FoodItemDto CreateFrenchFries() => new()
    {
        Id = 9,
        StationId = 3,
        ItemDescription = "Crispy golden french fries",
        ImageUrl = "https://picsum.photos/id/602/150/150",
        ItemPrice = 3.50m
    };
    #endregion ============================================================================

    #region ========================== API-like Methods ===================================
    public static Dictionary<IngredientTypeDto, List<IngredientDto>> GetIngredientsOrganizedByType()
    {
        Dictionary<IngredientTypeDto, List<IngredientDto>> ingredientsByType = new();
        foreach (IngredientTypeDto type in types)
        {
            if (type.Quantity >= 0)
            {
                ingredientsByType.Add(type, GetIngredientsForType(type.Id));
            }
            else if (type.Quantity < 0)
            {
                throw new ArgumentException("An ingredient type was found with a negative minimum quantity.");
            }
        }

        return ingredientsByType;
    }

    public static List<IngredientTypeDto> GetIngredientTypesByFoodItem(int foodItemId)
    {
        return foodItemId switch
        {
            1 => new List<IngredientTypeDto> { CreateBreadType(), CreateMeatType(), CreateVegetableType() },
            2 => new List<IngredientTypeDto> { CreateVegetableType(), CreateDressingType() },
            _ => new List<IngredientTypeDto>()
        };
    }

    public static List<IngredientDto> GetIngredientsForType(int ingredientTypeId)
    {
        return ingredientTypeId switch
        {
            1 => new List<IngredientDto> { CreateWheatBread() },
            2 => new List<IngredientDto> { CreateTurkey() },
            3 => new List<IngredientDto> { CreateLettuce(), CreateTomato() },
            _ => new List<IngredientDto>()
        };
    }

    public static List<IngredientTypeDto> GetIngredientTypesForFoodItem(int foodItemId)
    {
        return foodItemId switch
        {
            1 => new List<IngredientTypeDto> { CreateBreadType(), CreateMeatType(), CreateVegetableType() },
            2 => new List<IngredientTypeDto> { CreateVegetableType() },
            _ => new List<IngredientTypeDto>()
        };
    }

    public static List<IngredientDto> GetDefaultIngredientsForFoodItem(int foodItemId)
    {
        return foodItemId switch
        {
            1 => new List<IngredientDto> { CreateWheatBread(), CreateTurkey(), CreateLettuce() },
            2 => new List<IngredientDto> { CreateLettuce(), CreateTomato() },
            _ => new List<IngredientDto>()
        };
    }

    public static List<StationDto> GetStationsByLocation(int id)
    {
        return id switch
        {
            1 => new List<StationDto> { CreateSandwichStation(), CreateSaladStation(), CreateGrillStation() },
            2 => new List<StationDto> { CreateSandwichStation() },
            _ => new List<StationDto>()
        };
    }

    public static List<FoodItemDto> GetFoodItemsByStation(int stationId)
    {
        return stationId switch
        {
            1 => new List<FoodItemDto> { CreateTurkeysandwich(), CreateHamSandwich(), CreateVeggieSandwich() },
            2 => new List<FoodItemDto> { CreateGreenSalad(), CreateCaesarSalad(), CreateCobbSalad() },
            3 => new List<FoodItemDto> { CreateCheeseburger(), CreateGrilledChicken(), CreateFrenchFries() },
            _ => new List<FoodItemDto>()
        };
    }

    public static IngredientDto? GetIngredientById(int ingredientId)
    {
        return ingredientId switch
        {
            1 => CreateWheatBread(),
            2 => CreateTurkey(),
            3 => CreateLettuce(),
            4 => CreateTomato(),
            5 => CreateRanchDressing(),
            6 => CreateItalianDressing(),
            _ => null
        };
    }
    #endregion ============================================================================
}