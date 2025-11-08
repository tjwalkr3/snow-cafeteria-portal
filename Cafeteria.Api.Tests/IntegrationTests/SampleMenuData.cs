using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Tests.IntegrationTests;

public static class SampleMenuData
{
    // SQL Insert Queries
    public const string InsertLocationSql = @"
        INSERT INTO cafeteria.cafeteria_location (location_name, location_description, image_url)
        VALUES (@LocationName, @LocationDescription, @ImageUrl)";

    public const string InsertStationSql = @"
        INSERT INTO cafeteria.station (location_id, station_name, station_description)
        VALUES (@LocationId, @StationName, @StationDescription)";

    public const string InsertEntreeSql = @"
        INSERT INTO cafeteria.entree (station_id, entree_name, entree_description, entree_price, image_url)
        VALUES (@StationId, @EntreeName, @EntreeDescription, @EntreePrice, @ImageUrl)";

    public const string InsertSideSql = @"
        INSERT INTO cafeteria.side (station_id, side_name, side_description, side_price, image_url)
        VALUES (@StationId, @SideName, @SideDescription, @SidePrice, @ImageUrl)";

    public const string InsertDrinkSql = @"
        INSERT INTO cafeteria.drink (station_id, drink_name, drink_description, drink_price, image_url)
        VALUES (@StationId, @DrinkName, @DrinkDescription, @DrinkPrice, @ImageUrl)";

    public const string InsertFoodOptionSql = @"
        INSERT INTO cafeteria.food_option (food_option_name, in_stock, image_url)
        VALUES (@FoodOptionName, @InStock, @ImageUrl)";

    public const string InsertFoodOptionTypeSql = @"
        INSERT INTO cafeteria.food_option_type (food_option_type_name, num_included, max_amount, food_option_price, entree_id, side_id)
        VALUES (@FoodOptionTypeName, @NumIncluded, @MaxAmount, @FoodOptionPrice, @EntreeId, @SideId)";

    public const string InsertOptionOptionTypeSql = @"
        INSERT INTO cafeteria.option_option_type (food_option_id, food_option_type_id)
        VALUES (@FoodOptionId, @FoodOptionTypeId)";

    // Sample Data Lists
    public static List<LocationDto> Locations => [
        new() {
            Id = 1,
            LocationName = "Badger Den",
            LocationDescription = "Located on the main floor of the Greenwood Student Center",
            ImageUrl = "https://picsum.photos/id/292/300/200"
        },
        new() {
            Id = 2,
            LocationName = "Busters Bistro",
            LocationDescription = "Located on the main floor of the Karen H Huntsman Library",
            ImageUrl = "https://picsum.photos/id/326/300/200"
        }
    ];

    public static List<StationDto> Stations => [
        new() {
            Id = 1,
            LocationId = 1,
            StationName = "Sandwich Station",
            StationDescription = "Fresh made-to-order sandwiches"
        },
        new() {
            Id = 2,
            LocationId = 1,
            StationName = "Grill Station",
            StationDescription = "Hot grilled items"
        }
    ];

    public static List<EntreeDto> Entrees => [
        new() {
            Id = 1,
            StationId = 1,
            EntreeName = "Grilled Chicken",
            EntreeDescription = "Juicy grilled chicken breast",
            EntreePrice = 8.99m,
            ImageUrl = "https://picsum.photos/id/1/300/200"
        },
        new() {
            Id = 2,
            StationId = 1,
            EntreeName = "Burger",
            EntreeDescription = "Classic beef burger",
            EntreePrice = 7.99m,
            ImageUrl = "https://picsum.photos/id/2/300/200"
        }
    ];

    public static List<SideDto> Sides => [
        new() {
            Id = 1,
            StationId = 1,
            SideName = "French Fries",
            SideDescription = "Crispy golden fries",
            SidePrice = 2.99m,
            ImageUrl = "https://picsum.photos/id/3/300/200"
        },
        new() {
            Id = 2,
            StationId = 1,
            SideName = "Coleslaw",
            SideDescription = "Fresh cabbage slaw",
            SidePrice = 1.99m,
            ImageUrl = "https://picsum.photos/id/4/300/200"
        }
    ];

    public static List<DrinkDto> Drinks => [
        new() {
            Id = 1,
            StationId = 1,
            DrinkName = "Coca-Cola",
            DrinkDescription = "Classic Coke",
            DrinkPrice = 1.99m,
            ImageUrl = "https://picsum.photos/id/5/300/200"
        },
        new() {
            Id = 2,
            StationId = 1,
            DrinkName = "Lemonade",
            DrinkDescription = "Fresh squeezed lemonade",
            DrinkPrice = 2.49m,
            ImageUrl = "https://picsum.photos/id/6/300/200"
        }
    ];

    public static List<FoodOptionDto> FoodOptions => [
        new() {
            Id = 1,
            FoodOptionName = "Lettuce",
            InStock = true,
            ImageUrl = "https://picsum.photos/id/7/300/200"
        },
        new() {
            Id = 2,
            FoodOptionName = "Tomato",
            InStock = true,
            ImageUrl = "https://picsum.photos/id/8/300/200"
        }
    ];
}

