using Cafeteria.Shared.DTOs;

namespace Cafeteria.IntegrationTests.Api;

public static class SampleMenuData
{
    // SQL Insert Queries
    public const string InsertLocationSql =
        @"
        INSERT INTO cafeteria.cafeteria_location (location_name, location_description, image_url)
        VALUES (@LocationName, @LocationDescription, @ImageUrl)";

    public const string InsertStationSql =
        @"
        INSERT INTO cafeteria.station (location_id, station_name, station_description)
        VALUES (@LocationId, @StationName, @StationDescription)";

    public const string InsertEntreeSql =
        @"
        INSERT INTO cafeteria.entree (station_id, entree_name, entree_description, entree_price, image_url)
        VALUES (@StationId, @EntreeName, @EntreeDescription, @EntreePrice, @ImageUrl)";

    public const string InsertSideSql =
        @"
        INSERT INTO cafeteria.side (station_id, side_name, side_description, side_price, image_url)
        VALUES (@StationId, @SideName, @SideDescription, @SidePrice, @ImageUrl)";

    public const string InsertDrinkSql =
        @"
        INSERT INTO cafeteria.drink (location_id, drink_name, drink_description, drink_price, image_url)
        VALUES (@LocationId, @DrinkName, @DrinkDescription, @DrinkPrice, @ImageUrl)";

    public const string InsertFoodOptionSql =
        @"
        INSERT INTO cafeteria.food_option (food_option_name, in_stock, image_url)
        VALUES (@FoodOptionName, @InStock, @ImageUrl)";

    public const string InsertFoodOptionTypeSql =
        @"
        INSERT INTO cafeteria.food_option_type (food_option_type_name, num_included, max_amount, food_option_price, entree_id, side_id)
        VALUES (@FoodOptionTypeName, @NumIncluded, @MaxAmount, @FoodOptionPrice, @EntreeId, @SideId)";

    public const string InsertOptionOptionTypeSql =
        @"
        INSERT INTO cafeteria.option_option_type (food_option_id, food_option_type_id)
        VALUES (@FoodOptionId, @FoodOptionTypeId)";

    public const string InsertOrderSql =
        @"
        INSERT INTO cafeteria.order (total_price, total_swipe)
        VALUES (@TotalPrice, @TotalSwipe)";

    public const string InsertFoodItemOrderSql =
        @"
        INSERT INTO cafeteria.food_item_order (order_id, station_id, sale_card_id, sale_swipe_id, swipe_cost, card_cost, special)
        VALUES (@OrderId, @StationId, @SaleCardId, @SaleSwipeId, @SwipeCost, @CardCost, @Special)";

    public const string InsertFoodItemOptionSql =
        @"
        INSERT INTO cafeteria.food_item_option (food_item_order_id, food_option_name)
        VALUES (@FoodItemOrderId, @FoodOptionName)";
}
