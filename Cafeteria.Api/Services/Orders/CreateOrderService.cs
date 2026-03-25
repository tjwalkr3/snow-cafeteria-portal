using System.Data;
using Cafeteria.Api.Services.Print;
using Dapper;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Shared.Utilities;

namespace Cafeteria.Api.Services.Orders;

public class CreateOrderService(IDbConnection dbConnection, IPrintService printService) : ICreateOrderService
{
    private readonly IDbConnection _dbConnection = dbConnection;
    private readonly IPrintService _printService = printService;

    public async Task<OrderDto> CreateOrder(BrowserOrder browserOrder, string customerEmail)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        using var transaction = _dbConnection.BeginTransaction();

        try
        {
            var customerBadgerId = await GetCustomerBadgerIdAsync(customerEmail, transaction);

            var convertedOrder = ConvertToOrderDto(browserOrder);
            var order = await InsertOrderAsync(customerBadgerId, convertedOrder.TotalPrice, convertedOrder.Tax, convertedOrder.TotalSwipe, transaction);

            if (browserOrder.IsCardOrder)
            {
                var saleCardId = await InsertSaleCardAsync(order.Id, transaction);

                foreach (var entreeItem in browserOrder.Entrees)
                    order.FoodItems.Add(await InsertEntreeAsync(entreeItem, order.Id, saleCardId, null, transaction));

                foreach (var sideItem in browserOrder.Sides)
                    order.FoodItems.Add(await InsertSideAsync(sideItem, order.Id, saleCardId, null, transaction));

                foreach (var drink in browserOrder.Drinks)
                    order.FoodItems.Add(await InsertDrinkAsync(drink, order.Id, saleCardId, null, transaction));
            }
            else
            {
                var saleSwipeId = await InsertSaleSwipeAsync(order.Id, transaction);
                int swipeCount = OrderCalculations.CalculateTotalSwipe(browserOrder);

                await DeductSwipeBalanceAsync(customerBadgerId, swipeCount, transaction);

                for (int i = 0; i < swipeCount; i++)
                {
                    order.FoodItems.Add(await InsertEntreeAsync(browserOrder.Entrees[i], order.Id, null, saleSwipeId, transaction, swipeCost: 1));
                    order.FoodItems.Add(await InsertSideAsync(browserOrder.Sides[i], order.Id, null, saleSwipeId, transaction, swipeCost: 0));
                    order.FoodItems.Add(await InsertDrinkAsync(browserOrder.Drinks[i], order.Id, null, saleSwipeId, transaction, swipeCost: 0));
                }
            }

            transaction.Commit();
            var orderId = order.Id;
            await _printService.PrintOrder(browserOrder, orderId);
            return order;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private async Task<FoodItemDto> InsertEntreeAsync(
            OrderEntreeItem item, int orderId, int? saleCardId, int? saleSwipeId, IDbTransaction transaction, int? swipeCost = null)
    {
        decimal? cardCost = saleCardId.HasValue
                ? item.Entree.EntreePrice + OrderCalculations.CalculateOptionsCost(item.SelectedOptions)
                : null;

        var foodItem = await InsertFoodItemAsync(
                item.Entree.EntreeName, item.Entree.StationId, null,
                orderId, saleCardId, saleSwipeId, swipeCost, cardCost, special: false, transaction);

        foreach (var opt in item.SelectedOptions)
            foodItem.Options.Add(await InsertFoodItemOptionAsync(foodItem.Id, opt.Option.FoodOptionName, transaction));

        return foodItem;
    }

    private async Task<FoodItemDto> InsertSideAsync(
            OrderSideItem item, int orderId, int? saleCardId, int? saleSwipeId, IDbTransaction transaction, int? swipeCost = null)
    {
        decimal? cardCost = saleCardId.HasValue
                ? item.Side.SidePrice + OrderCalculations.CalculateOptionsCost(item.SelectedOptions)
                : null;

        var foodItem = await InsertFoodItemAsync(
                item.Side.SideName, item.Side.StationId, null,
                orderId, saleCardId, saleSwipeId, swipeCost, cardCost, special: false, transaction);

        foreach (var opt in item.SelectedOptions)
            foodItem.Options.Add(await InsertFoodItemOptionAsync(foodItem.Id, opt.Option.FoodOptionName, transaction));

        return foodItem;
    }

    private async Task<FoodItemDto> InsertDrinkAsync(
            DrinkDto drink, int orderId, int? saleCardId, int? saleSwipeId, IDbTransaction transaction, int? swipeCost = null)
    {
        decimal? cardCost = saleCardId.HasValue ? drink.DrinkPrice : null;

        return await InsertFoodItemAsync(
                drink.DrinkName, null, drink.LocationId,
                orderId, saleCardId, saleSwipeId, swipeCost, cardCost, special: false, transaction);
    }

    private async Task<int> GetCustomerBadgerIdAsync(string email, IDbTransaction transaction)
    {
        const string sql = @"
                        SELECT badger_id
                        FROM cafeteria.customer
                        WHERE email = @Email";

        var badgerId = await _dbConnection.QuerySingleOrDefaultAsync<int?>(sql, new { Email = email }, transaction);

        if (badgerId == null)
            throw new InvalidOperationException($"Customer with email {email} not found");

        return badgerId.Value;
    }

    private async Task<OrderDto> InsertOrderAsync(int customerBadgerId, decimal? totalPrice, decimal tax, int? totalSwipe, IDbTransaction transaction)
    {
        const string sql = @"
                        INSERT INTO cafeteria.order (customer_badger_id, total_price, tax, total_swipe)
                        VALUES (@CustomerBadgerId, @TotalPrice, @Tax, @TotalSwipe)
                        RETURNING id AS Id, order_time AS OrderTime, total_price AS TotalPrice, tax AS Tax, total_swipe AS TotalSwipe;";

        return await _dbConnection.QuerySingleAsync<OrderDto>(
                sql,
                new { CustomerBadgerId = customerBadgerId, TotalPrice = totalPrice, Tax = tax, TotalSwipe = totalSwipe },
                transaction);
    }

    private async Task<int> InsertSaleCardAsync(int orderId, IDbTransaction transaction)
    {
        const string sql = @"
                        INSERT INTO cafeteria.sale_card (order_id)
                        VALUES (@OrderId)
                        RETURNING id;";

        return await _dbConnection.QuerySingleAsync<int>(sql, new { OrderId = orderId }, transaction);
    }

    private async Task<int> InsertSaleSwipeAsync(int orderId, IDbTransaction transaction)
    {
        const string sql = @"
                        INSERT INTO cafeteria.sale_swipe (order_id)
                        VALUES (@OrderId)
                        RETURNING id;";

        return await _dbConnection.QuerySingleAsync<int>(sql, new { OrderId = orderId }, transaction);
    }

    private async Task<FoodItemDto> InsertFoodItemAsync(
            string name, int? stationId, int? locationId,
            int orderId, int? saleCardId, int? saleSwipeId, int? swipeCost, decimal? cardCost, bool special,
            IDbTransaction transaction)
    {
        const string sql = @"
                        INSERT INTO cafeteria.food_item
                                (name, order_id, station_id, location_id, sale_card_id, sale_swipe_id, swipe_cost, card_cost, special)
                        VALUES
                                (@Name, @OrderId, @StationId, @LocationId, @SaleCardId, @SaleSwipeId, @SwipeCost, @CardCost, @Special)
                        RETURNING id AS Id, name AS Name, order_id AS OrderId, station_id AS StationId, location_id AS LocationId,
                                sale_card_id AS SaleCardId, sale_swipe_id AS SaleSwipeId,
                                swipe_cost AS SwipeCost, card_cost AS CardCost, special AS Special;";

        return await _dbConnection.QuerySingleAsync<FoodItemDto>(
                sql,
                new
                {
                    Name = name,
                    OrderId = orderId,
                    StationId = stationId,
                    LocationId = locationId,
                    SaleCardId = saleCardId,
                    SaleSwipeId = saleSwipeId,
                    SwipeCost = swipeCost,
                    CardCost = cardCost,
                    Special = special
                },
                transaction);
    }

    private async Task DeductSwipeBalanceAsync(int badgerId, int swipeCount, IDbTransaction transaction)
    {
        const string sql = @"
                        UPDATE cafeteria.customer_swipe
                        SET swipe_balance = swipe_balance - @SwipeCount
                        WHERE badger_id = @BadgerId
                        AND (end_date IS NULL OR end_date >= CURRENT_DATE)";

        await _dbConnection.ExecuteAsync(sql, new { BadgerId = badgerId, SwipeCount = swipeCount }, transaction);
    }

    private async Task<FoodItemOptionDto> InsertFoodItemOptionAsync(int foodItemId, string optionName, IDbTransaction transaction)
    {
        const string sql = @"
                        INSERT INTO cafeteria.food_item_option (food_item_order_id, food_option_name)
                        VALUES (@FoodItemId, @FoodOptionName)
                        RETURNING id AS Id, food_item_order_id AS FoodItemId, food_option_name AS FoodOptionName;";

        return await _dbConnection.QuerySingleAsync<FoodItemOptionDto>(
                sql,
                new { FoodItemId = foodItemId, FoodOptionName = optionName },
                transaction);
    }

    private static OrderDto ConvertToOrderDto(BrowserOrder browserOrder)
    {
        ArgumentNullException.ThrowIfNull(browserOrder);

        return new OrderDto
        {
            TotalPrice = browserOrder.IsCardOrder ? OrderCalculations.CalculateTotalPrice(browserOrder) : null,
            Tax = OrderCalculations.CalculateTax(browserOrder),
            TotalSwipe = browserOrder.IsCardOrder ? null : OrderCalculations.CalculateTotalSwipe(browserOrder)
        };
    }
}
