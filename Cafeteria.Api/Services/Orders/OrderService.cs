using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Api.Services.Orders;

public class GetOrderService : IOrderService
{
    private readonly IDbConnection _dbConnection;

    public GetOrderService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
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
            SELECT id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId, location_id AS LocationId,
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
                SELECT id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId, location_id AS LocationId,
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

    public async Task<List<OrderWithCustomerDto>> GetAllOrdersWithCustomer()
    {
        const string sql = @"
            SELECT o.id AS Id, o.order_time AS OrderTime, o.total_price AS TotalPrice, o.tax AS Tax, o.total_swipe AS TotalSwipe,
                   o.customer_badger_id AS CustomerBadgerId, c.cust_name AS CustomerName, c.email AS CustomerEmail,
                   CASE WHEN sc.id IS NOT NULL THEN 'Card' ELSE 'Swipe' END AS PaymentType,
                   (SELECT COUNT(*) FROM cafeteria.food_item fi WHERE fi.order_id = o.id) AS ItemCount
            FROM cafeteria.order o
            LEFT JOIN cafeteria.customer c ON o.customer_badger_id = c.badger_id
            LEFT JOIN cafeteria.sale_card sc ON sc.order_id = o.id
            ORDER BY o.order_time DESC";

        var orders = (await _dbConnection.QueryAsync<OrderWithCustomerDto>(sql)).ToList();

        const string foodItemsSql = @"
            SELECT id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId,
                sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special
            FROM cafeteria.food_item
            WHERE order_id = @orderId;";

        foreach (var order in orders)
        {
            var foodItems = await _dbConnection.QueryAsync<FoodItemDto>(foodItemsSql, new { orderId = order.Id });
            order.FoodItems = foodItems.ToList();
        }

        return orders;
    }

    public async Task<List<OrderWithCustomerDto>> GetOrdersByCustomer(int badgerId)
    {
        const string sql = @"
            SELECT o.id AS Id, o.order_time AS OrderTime, o.total_price AS TotalPrice, o.tax AS Tax, o.total_swipe AS TotalSwipe,
                   o.customer_badger_id AS CustomerBadgerId, c.cust_name AS CustomerName, c.email AS CustomerEmail,
                   CASE WHEN sc.id IS NOT NULL THEN 'Card' ELSE 'Swipe' END AS PaymentType,
                   (SELECT COUNT(*) FROM cafeteria.food_item fi WHERE fi.order_id = o.id) AS ItemCount
            FROM cafeteria.order o
            LEFT JOIN cafeteria.customer c ON o.customer_badger_id = c.badger_id
            LEFT JOIN cafeteria.sale_card sc ON sc.order_id = o.id
            WHERE o.customer_badger_id = @BadgerId
            ORDER BY o.order_time DESC";

        var orders = await _dbConnection.QueryAsync<OrderWithCustomerDto>(sql, new { BadgerId = badgerId });
        return orders.ToList();
    }

    public async Task<List<OrderDto>> GetOrdersByCustomerEmail(string email)
    {
        const string sql = @"
            SELECT o.id AS Id, o.order_time AS OrderTime, o.total_price AS TotalPrice, o.tax AS Tax, o.total_swipe AS TotalSwipe
            FROM cafeteria.order o
            INNER JOIN cafeteria.customer c ON o.customer_badger_id = c.badger_id
            WHERE c.email = @Email
            ORDER BY o.order_time DESC";

        var orders = (await _dbConnection.QueryAsync<OrderDto>(sql, new { Email = email })).ToList();

        foreach (var order in orders)
        {
            const string foodItemsSql = @"
                SELECT id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId, location_id AS LocationId,
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

    public async Task<OrderWithCustomerDto?> GetOrderWithCustomerById(int id)
    {
        const string sql = @"
            SELECT o.id AS Id, o.order_time AS OrderTime, o.total_price AS TotalPrice, o.tax AS Tax, o.total_swipe AS TotalSwipe,
                   o.customer_badger_id AS CustomerBadgerId, c.cust_name AS CustomerName, c.email AS CustomerEmail,
                   CASE WHEN sc.id IS NOT NULL THEN 'Card' ELSE 'Swipe' END AS PaymentType,
                   (SELECT COUNT(*) FROM cafeteria.food_item fi WHERE fi.order_id = o.id) AS ItemCount
            FROM cafeteria.order o
            LEFT JOIN cafeteria.customer c ON o.customer_badger_id = c.badger_id
            LEFT JOIN cafeteria.sale_card sc ON sc.order_id = o.id
            WHERE o.id = @Id";

        var order = await _dbConnection.QuerySingleOrDefaultAsync<OrderWithCustomerDto>(sql, new { Id = id });
        if (order == null) return null;

        const string foodItemsSql = @"
            SELECT id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId, location_id AS LocationId,
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
}
