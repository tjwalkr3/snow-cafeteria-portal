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
                INSERT INTO cafeteria.order (total_price, tax, total_swipe)
                VALUES (@TotalPrice, @Tax, @TotalSwipe)
                RETURNING id AS Id, order_time AS OrderTime, total_price AS TotalPrice, tax AS Tax, total_swipe AS TotalSwipe;";

            var order = await _dbConnection.QuerySingleAsync<OrderDto>(
                insertOrderSql,
                new { createOrderDto.TotalPrice, createOrderDto.Tax, createOrderDto.TotalSwipe },
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
                    INSERT INTO cafeteria.food_item
                        (name, order_id, station_id, sale_card_id, sale_swipe_id, swipe_cost, card_cost, special)
                    VALUES
                        (@Name, @OrderId, @StationId, @SaleCardId, @SaleSwipeId, @SwipeCost, @CardCost, @Special)
                    RETURNING id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId,
                        sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                        swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special;";

                var insertedFoodItem = await _dbConnection.QuerySingleAsync<FoodItemDto>(
                    insertFoodItemSql,
                    new
                    {
                        foodItem.Name,
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
                        VALUES (@FoodItemId, @FoodOptionName)
                        RETURNING id AS Id, food_item_order_id AS FoodItemId, food_option_name AS FoodOptionName;";

                    var insertedOption = await _dbConnection.QuerySingleAsync<FoodItemOptionDto>(
                        insertOptionSql,
                        new
                        {
                            FoodItemId = insertedFoodItem.Id,
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
            SELECT id AS Id, order_time AS OrderTime, total_price AS TotalPrice, tax AS Tax, total_swipe AS TotalSwipe
            FROM cafeteria.order
            WHERE id = @id;";

        var order = await _dbConnection.QuerySingleOrDefaultAsync<OrderDto>(orderSql, new { id });
        if (order == null) return null;

        const string foodItemsSql = @"
            SELECT id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId,
                sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special
            FROM cafeteria.food_item
            WHERE order_id = @orderId;";

        var foodItems = await _dbConnection.QueryAsync<FoodItemDto>(foodItemsSql, new { orderId = id });
        order.FoodItems = foodItems.ToList();

        foreach (var foodItem in order.FoodItems)
        {
            const string optionsSql = @"
                SELECT id AS Id, food_item_order_id AS FoodItemId, food_option_name AS FoodOptionName
                FROM cafeteria.food_item_option
                WHERE food_item_order_id = @foodItemId;";

            var options = await _dbConnection.QueryAsync<FoodItemOptionDto>(optionsSql, new { foodItemId = foodItem.Id });
            foodItem.Options = options.ToList();
        }

        return order;
    }

    public async Task<List<OrderDto>> GetAllOrders()
    {
        const string ordersSql = @"
            SELECT id AS Id, order_time AS OrderTime, total_price AS TotalPrice, tax AS Tax, total_swipe AS TotalSwipe
            FROM cafeteria.order
            ORDER BY order_time DESC;";

        var orders = (await _dbConnection.QueryAsync<OrderDto>(ordersSql)).ToList();

        foreach (var order in orders)
        {
            const string foodItemsSql = @"
                SELECT id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId,
                    sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                    swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special
                FROM cafeteria.food_item
                WHERE order_id = @orderId;";

            var foodItems = await _dbConnection.QueryAsync<FoodItemDto>(foodItemsSql, new { orderId = order.Id });
            order.FoodItems = foodItems.ToList();

            foreach (var foodItem in order.FoodItems)
            {
                const string optionsSql = @"
                    SELECT id AS Id, food_item_order_id AS FoodItemId, food_option_name AS FoodOptionName
                    FROM cafeteria.food_item_option
                    WHERE food_item_order_id = @foodItemId;";

                var options = await _dbConnection.QueryAsync<FoodItemOptionDto>(optionsSql, new { foodItemId = foodItem.Id });
                foodItem.Options = options.ToList();
            }
        }

        return orders;
    }
}
