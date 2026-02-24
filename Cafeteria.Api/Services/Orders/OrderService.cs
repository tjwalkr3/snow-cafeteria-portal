using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Api.Services.Orders;

public class OrderService : IOrderService
{
    private readonly IDbConnection _dbConnection;

    public OrderService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<OrderDto> CreateOrder(BrowserOrder browserOrder, string customerEmail)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        using var transaction = _dbConnection.BeginTransaction();

        try
        {
            // Get customer badger_id from email
            const string getCustomerSql = @"
                SELECT badger_id
                FROM cafeteria.customer
                WHERE email = @Email";

            var customerBadgerId = await _dbConnection.QuerySingleOrDefaultAsync<int?>(
                getCustomerSql,
                new { Email = customerEmail },
                transaction);

            if (customerBadgerId == null)
            {
                throw new InvalidOperationException($"Customer with email {customerEmail} not found");
            }

            decimal? totalPrice = browserOrder.IsCardOrder ? CalculateTotalPrice(browserOrder) : null;
            decimal tax = CalculateTax(browserOrder);
            int? totalSwipe = browserOrder.IsCardOrder ? null : CalculateTotalSwipe(browserOrder);

            const string insertOrderSql = @"
                INSERT INTO cafeteria.order (customer_badger_id, total_price, tax, total_swipe)
                VALUES (@CustomerBadgerId, @TotalPrice, @Tax, @TotalSwipe)
                RETURNING id AS Id, order_time AS OrderTime, total_price AS TotalPrice, tax AS Tax, total_swipe AS TotalSwipe;";

            var order = await _dbConnection.QuerySingleAsync<OrderDto>(
                insertOrderSql,
                new { CustomerBadgerId = customerBadgerId.Value, TotalPrice = totalPrice, Tax = tax, TotalSwipe = totalSwipe },
                transaction);

            int? saleCardId = null;
            int? saleSwipeId = null;

            if (browserOrder.IsCardOrder)
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

            foreach (var foodItem in ConvertOrderToFoodItems(browserOrder))
            {
                const string insertFoodItemSql = @"
                    INSERT INTO cafeteria.food_item
                        (name, order_id, station_id, location_id, sale_card_id, sale_swipe_id, swipe_cost, card_cost, special)
                    VALUES
                        (@Name, @OrderId, @StationId, @LocationId, @SaleCardId, @SaleSwipeId, @SwipeCost, @CardCost, @Special)
                    RETURNING id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId, location_id AS LocationId,
                        sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                        swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special;";

                var insertedFoodItem = await _dbConnection.QuerySingleAsync<FoodItemDto>(
                    insertFoodItemSql,
                    new
                    {
                        foodItem.Name,
                        OrderId = order.Id,
                        foodItem.StationId,
                        foodItem.LocationId,
                        SaleCardId = saleCardId,
                        SaleSwipeId = saleSwipeId,
                        foodItem.SwipeCost,
                        foodItem.CardCost,
                        foodItem.Special
                    },
                    transaction);

                foreach (var optionName in foodItem.OptionNames)
                {
                    const string insertOptionSql = @"
                        INSERT INTO cafeteria.food_item_option (food_item_order_id, food_option_name)
                        VALUES (@FoodItemId, @FoodOptionName)
                        RETURNING id AS Id, food_item_order_id AS FoodItemId, food_option_name AS FoodOptionName;";

                    var insertedOption = await _dbConnection.QuerySingleAsync<FoodItemOptionDto>(
                        insertOptionSql,
                        new { FoodItemId = insertedFoodItem.Id, FoodOptionName = optionName },
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

    private const decimal TaxRate = 0.0775m;

    private decimal CalculateOptionsCost(List<SelectedFoodOption> selectedOptions)
    {
        if (selectedOptions == null || selectedOptions.Count == 0)
            return 0m;

        decimal cost = 0m;
        var optionsByType = selectedOptions.GroupBy(opt => opt.OptionType.Id);

        foreach (var group in optionsByType)
        {
            var optionType = group.First().OptionType;
            var selectedCount = group.Count();
            var chargeableCount = Math.Max(0, selectedCount - optionType.NumIncluded);
            cost += chargeableCount * optionType.FoodOptionPrice;
        }

        return cost;
    }

    private decimal CalculateTotalPrice(BrowserOrder order)
    {
        decimal total = 0m;

        foreach (var entreeItem in order.Entrees)
        {
            total += entreeItem.Entree.EntreePrice;
            total += CalculateOptionsCost(entreeItem.SelectedOptions);
        }

        foreach (var sideItem in order.Sides)
        {
            total += sideItem.Side.SidePrice;
            total += CalculateOptionsCost(sideItem.SelectedOptions);
        }

        total += order.Drinks.Sum(drink => drink.DrinkPrice);
        return total;
    }

    private decimal CalculateTax(BrowserOrder order)
    {
        if (!order.IsCardOrder)
            return 0m;

        return Math.Round(CalculateTotalPrice(order) * TaxRate, 2);
    }

    private int CalculateTotalSwipe(BrowserOrder order)
    {
        if (order.IsCardOrder)
            return 0;

        return Math.Min(order.Entrees.Count,
               Math.Min(order.Sides.Count, order.Drinks.Count));
    }

    private List<FoodItemData> ConvertOrderToFoodItems(BrowserOrder order)
    {
        var items = new List<FoodItemData>();

        if (order.IsCardOrder)
        {
            foreach (var entreeItem in order.Entrees)
            {
                items.Add(new FoodItemData(
                    Name: entreeItem.Entree.EntreeName,
                    StationId: entreeItem.Entree.StationId,
                    LocationId: null,
                    SwipeCost: null,
                    CardCost: entreeItem.Entree.EntreePrice + CalculateOptionsCost(entreeItem.SelectedOptions),
                    Special: false,
                    OptionNames: entreeItem.SelectedOptions.Select(o => o.Option.FoodOptionName).ToList()
                ));
            }

            foreach (var sideItem in order.Sides)
            {
                items.Add(new FoodItemData(
                    Name: sideItem.Side.SideName,
                    StationId: sideItem.Side.StationId,
                    LocationId: null,
                    SwipeCost: null,
                    CardCost: sideItem.Side.SidePrice + CalculateOptionsCost(sideItem.SelectedOptions),
                    Special: false,
                    OptionNames: sideItem.SelectedOptions.Select(o => o.Option.FoodOptionName).ToList()
                ));
            }

            foreach (var drink in order.Drinks)
            {
                items.Add(new FoodItemData(
                    Name: drink.DrinkName,
                    StationId: null,
                    LocationId: drink.LocationId,
                    SwipeCost: null,
                    CardCost: drink.DrinkPrice,
                    Special: false,
                    OptionNames: new List<string>()
                ));
            }
        }
        else
        {
            int swipeCount = Math.Min(order.Entrees.Count,
                             Math.Min(order.Sides.Count, order.Drinks.Count));

            for (int i = 0; i < swipeCount; i++)
            {
                var entreeItem = order.Entrees[i];
                var sideItem = order.Sides[i];
                var drink = order.Drinks[i];

                items.Add(new FoodItemData(
                    Name: entreeItem.Entree.EntreeName,
                    StationId: entreeItem.Entree.StationId,
                    LocationId: null,
                    SwipeCost: 1,
                    CardCost: null,
                    Special: false,
                    OptionNames: entreeItem.SelectedOptions.Select(o => o.Option.FoodOptionName).ToList()
                ));

                items.Add(new FoodItemData(
                    Name: sideItem.Side.SideName,
                    StationId: sideItem.Side.StationId,
                    LocationId: null,
                    SwipeCost: 0,
                    CardCost: null,
                    Special: false,
                    OptionNames: sideItem.SelectedOptions.Select(o => o.Option.FoodOptionName).ToList()
                ));

                items.Add(new FoodItemData(
                    Name: drink.DrinkName,
                    StationId: null,
                    LocationId: drink.LocationId,
                    SwipeCost: 0,
                    CardCost: null,
                    Special: false,
                    OptionNames: new List<string>()
                ));
            }
        }

        return items;
    }

    private record FoodItemData(
        string Name,
        int? StationId,
        int? LocationId,
        int? SwipeCost,
        decimal? CardCost,
        bool Special,
        List<string> OptionNames);

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

        var orders = await _dbConnection.QueryAsync<OrderWithCustomerDto>(sql);
        return orders.ToList();
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
