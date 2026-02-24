using System.Data;
using Dapper;
using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;

namespace Cafeteria.Api.Services.Orders;

public class CreateOrderService(IDbConnection dbConnection) : ICreateOrderService
{
    private readonly IDbConnection _dbConnection = dbConnection;
    private const decimal TaxRate = 0.0775m;

    public async Task<OrderDto> CreateOrder(BrowserOrder browserOrder, string customerEmail)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        using var transaction = _dbConnection.BeginTransaction();

        try
        {
            var customerBadgerId = await GetCustomerBadgerIdAsync(customerEmail, transaction);

            decimal? totalPrice = browserOrder.IsCardOrder ? CalculateTotalPrice(browserOrder) : null;
            decimal tax = CalculateTax(browserOrder);
            int? totalSwipe = browserOrder.IsCardOrder ? null : CalculateTotalSwipe(browserOrder);

            var order = await InsertOrderAsync(customerBadgerId, totalPrice, tax, totalSwipe, transaction);

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
                int swipeCount = CalculateTotalSwipe(browserOrder);

                for (int i = 0; i < swipeCount; i++)
                {
                    order.FoodItems.Add(await InsertEntreeAsync(browserOrder.Entrees[i], order.Id, null, saleSwipeId, transaction, swipeCost: 1));
                    order.FoodItems.Add(await InsertSideAsync(browserOrder.Sides[i], order.Id, null, saleSwipeId, transaction, swipeCost: 0));
                    order.FoodItems.Add(await InsertDrinkAsync(browserOrder.Drinks[i], order.Id, null, saleSwipeId, transaction, swipeCost: 0));
                }
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

    private async Task<FoodItemDto> InsertEntreeAsync(
            OrderEntreeItem item, int orderId, int? saleCardId, int? saleSwipeId, IDbTransaction transaction, int? swipeCost = null)
    {
        decimal? cardCost = saleCardId.HasValue
                ? item.Entree.EntreePrice + CalculateOptionsCost(item.SelectedOptions)
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
                ? item.Side.SidePrice + CalculateOptionsCost(item.SelectedOptions)
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
                new { Name = name, OrderId = orderId, StationId = stationId, LocationId = locationId,
                      SaleCardId = saleCardId, SaleSwipeId = saleSwipeId, SwipeCost = swipeCost, CardCost = cardCost, Special = special },
                transaction);
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
        return Math.Min(order.Entrees.Count,
                Math.Min(order.Sides.Count, order.Drinks.Count));
    }
}
