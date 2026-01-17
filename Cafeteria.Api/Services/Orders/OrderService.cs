using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs;

namespace Cafeteria.Api.Services;

public class OrderService : IOrderService
{
    private readonly IDbConnection _dbConnection;

    public OrderService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<OrderDto> CreateOrder(CreateOrderDto createOrderDto)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        using var transaction = _dbConnection.BeginTransaction();

        try
        {
            const string insertOrderSql = @"
                INSERT INTO cafeteria.order (total_price)
                VALUES (@TotalPrice)
                RETURNING id AS Id, order_time AS OrderTime, total_price AS TotalPrice;";

            var order = await _dbConnection.QuerySingleAsync<OrderDto>(
                insertOrderSql,
                new { createOrderDto.TotalPrice },
                transaction);

            bool isCardOrder = createOrderDto.FoodItems.Any() && createOrderDto.FoodItems[0].CardCost.HasValue;
            int? saleCardId = null;
            int? saleSwipeId = null;

            if (isCardOrder)
            {
                const string insertSaleCardSql = @"
                    INSERT INTO cafeteria.sale_card (order_id)
                    VALUES (@OrderId)
                    RETURNING id;";

                saleCardId = await _dbConnection.QuerySingleAsync<int>(
                    insertSaleCardSql,
                    new { OrderId = order.Id },
                    transaction);
            }
            else
            {
                const string insertSaleSwipeSql = @"
                    INSERT INTO cafeteria.sale_swipe (order_id)
                    VALUES (@OrderId)
                    RETURNING id;";

                saleSwipeId = await _dbConnection.QuerySingleAsync<int>(
                    insertSaleSwipeSql,
                    new { OrderId = order.Id },
                    transaction);
            }

            foreach (var foodItem in createOrderDto.FoodItems)
            {
                const string insertFoodItemSql = @"
                    INSERT INTO cafeteria.food_item_order
                        (order_id, station_id, sale_card_id, sale_swipe_id, swipe_cost, card_cost, special)
                    VALUES
                        (@OrderId, @StationId, @SaleCardId, @SaleSwipeId, @SwipeCost, @CardCost, @Special)
                    RETURNING id AS Id, order_id AS OrderId, station_id AS StationId,
                        sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                        swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special;";

                var insertedFoodItem = await _dbConnection.QuerySingleAsync<FoodItemOrderDto>(
                    insertFoodItemSql,
                    new
                    {
                        OrderId = order.Id,
                        foodItem.StationId,
                        SaleCardId = saleCardId,
                        SaleSwipeId = saleSwipeId,
                        foodItem.SwipeCost,
                        foodItem.CardCost,
                        foodItem.Special
                    },
                    transaction);

                foreach (var option in foodItem.Options)
                {
                    const string insertOptionSql = @"
                        INSERT INTO cafeteria.food_item_option (food_item_order_id, food_option_name)
                        VALUES (@FoodItemOrderId, @FoodOptionName)
                        RETURNING id AS Id, food_item_order_id AS FoodItemOrderId, food_option_name AS FoodOptionName;";

                    var insertedOption = await _dbConnection.QuerySingleAsync<FoodItemOptionDto>(
                        insertOptionSql,
                        new
                        {
                            FoodItemOrderId = insertedFoodItem.Id,
                            option.FoodOptionName
                        },
                        transaction);

                    insertedFoodItem.Options.Add(insertedOption);
                }

                order.FoodItems.Add(insertedFoodItem);
            }

            transaction.Commit();
            return order;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<OrderDto?> GetOrderById(int id)
    {
        const string orderSql = @"
            SELECT id AS Id, order_time AS OrderTime, total_price AS TotalPrice
            FROM cafeteria.order
            WHERE id = @id;";

        var order = await _dbConnection.QuerySingleOrDefaultAsync<OrderDto>(orderSql, new { id });
        if (order == null) return null;

        const string foodItemsSql = @"
            SELECT id AS Id, order_id AS OrderId, station_id AS StationId,
                sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special
            FROM cafeteria.food_item_order
            WHERE order_id = @orderId;";

        var foodItems = await _dbConnection.QueryAsync<FoodItemOrderDto>(foodItemsSql, new { orderId = id });
        order.FoodItems = foodItems.ToList();

        foreach (var foodItem in order.FoodItems)
        {
            const string optionsSql = @"
                SELECT id AS Id, food_item_order_id AS FoodItemOrderId, food_option_name AS FoodOptionName
                FROM cafeteria.food_item_option
                WHERE food_item_order_id = @foodItemOrderId;";

            var options = await _dbConnection.QueryAsync<FoodItemOptionDto>(optionsSql, new { foodItemOrderId = foodItem.Id });
            foodItem.Options = options.ToList();
        }

        return order;
    }

    public async Task<List<OrderDto>> GetAllOrders()
    {
        const string ordersSql = @"
            SELECT id AS Id, order_time AS OrderTime, total_price AS TotalPrice
            FROM cafeteria.order
            ORDER BY order_time DESC;";

        var orders = (await _dbConnection.QueryAsync<OrderDto>(ordersSql)).ToList();

        foreach (var order in orders)
        {
            const string foodItemsSql = @"
                SELECT id AS Id, order_id AS OrderId, station_id AS StationId,
                    sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                    swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special
                FROM cafeteria.food_item_order
                WHERE order_id = @orderId;";

            var foodItems = await _dbConnection.QueryAsync<FoodItemOrderDto>(foodItemsSql, new { orderId = order.Id });
            order.FoodItems = foodItems.ToList();

            foreach (var foodItem in order.FoodItems)
            {
                const string optionsSql = @"
                    SELECT id AS Id, food_item_order_id AS FoodItemOrderId, food_option_name AS FoodOptionName
                    FROM cafeteria.food_item_option
                    WHERE food_item_order_id = @foodItemOrderId;";

                var options = await _dbConnection.QueryAsync<FoodItemOptionDto>(optionsSql, new { foodItemOrderId = foodItem.Id });
                foodItem.Options = options.ToList();
            }
        }

        return orders;
    }
}
