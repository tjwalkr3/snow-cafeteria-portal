using Cafeteria.Shared.DTOs.Menu;
using Cafeteria.Shared.DTOs.Order;
using Cafeteria.Shared.Utilities;

namespace Cafeteria.UnitTests.Shared;

public class OrderCalculationsTests
{
    // ---------------------------------------------------------------------------
    // CalculateOptionsCost
    // ---------------------------------------------------------------------------

    [Fact]
    public void CalculateOptionsCost_NullList_ReturnsZero()
    {
        var result = OrderCalculations.CalculateOptionsCost(null!);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateOptionsCost_EmptyList_ReturnsZero()
    {
        var result = OrderCalculations.CalculateOptionsCost([]);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateOptionsCost_AllWithinNumIncluded_ReturnsZero()
    {
        // NumIncluded is 2, only 1 selected — no charge
        var options = new List<SelectedFoodOption>
        {
            MakeFoodOption(optionTypeId: 1, numIncluded: 2, pricePerExtra: 0.50m)
        };

        var result = OrderCalculations.CalculateOptionsCost(options);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateOptionsCost_ExceedsNumIncluded_ChargesForExtras()
    {
        // NumIncluded is 1, selected 3 — 2 extras charged at $0.50 each
        var options = new List<SelectedFoodOption>
        {
            MakeFoodOption(optionTypeId: 1, numIncluded: 1, pricePerExtra: 0.50m),
            MakeFoodOption(optionTypeId: 1, numIncluded: 1, pricePerExtra: 0.50m),
            MakeFoodOption(optionTypeId: 1, numIncluded: 1, pricePerExtra: 0.50m)
        };

        var result = OrderCalculations.CalculateOptionsCost(options);
        Assert.Equal(1.00m, result);
    }

    [Fact]
    public void CalculateOptionsCost_ExactlyNumIncluded_ReturnsZero()
    {
        // NumIncluded is 2, selected exactly 2 — no charge
        var options = new List<SelectedFoodOption>
        {
            MakeFoodOption(optionTypeId: 1, numIncluded: 2, pricePerExtra: 1.00m),
            MakeFoodOption(optionTypeId: 1, numIncluded: 2, pricePerExtra: 1.00m)
        };

        var result = OrderCalculations.CalculateOptionsCost(options);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateOptionsCost_MultipleOptionTypes_ChargesEachTypeIndependently()
    {
        // Type 1: 3 selected, 1 included → 2 extras × $1.00 = $2.00
        // Type 2: 2 selected, 0 included → 2 extras × $0.75 = $1.50
        var options = new List<SelectedFoodOption>
        {
            MakeFoodOption(optionTypeId: 1, numIncluded: 1, pricePerExtra: 1.00m),
            MakeFoodOption(optionTypeId: 1, numIncluded: 1, pricePerExtra: 1.00m),
            MakeFoodOption(optionTypeId: 1, numIncluded: 1, pricePerExtra: 1.00m),
            MakeFoodOption(optionTypeId: 2, numIncluded: 0, pricePerExtra: 0.75m),
            MakeFoodOption(optionTypeId: 2, numIncluded: 0, pricePerExtra: 0.75m)
        };

        var result = OrderCalculations.CalculateOptionsCost(options);
        Assert.Equal(3.50m, result);
    }

    // ---------------------------------------------------------------------------
    // CalculateTotalPrice
    // ---------------------------------------------------------------------------

    [Fact]
    public void CalculateTotalPrice_NullOrder_ReturnsZero()
    {
        var result = OrderCalculations.CalculateTotalPrice(null!);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateTotalPrice_EmptyOrder_ReturnsZero()
    {
        var order = new BrowserOrder();

        var result = OrderCalculations.CalculateTotalPrice(order);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateTotalPrice_EntreesSidesAndDrinks_SumsPricesCorrectly()
    {
        var order = new BrowserOrder
        {
            Entrees =
            [
                new OrderEntreeItem { Entree = new EntreeDto { EntreePrice = 8.00m, StationId = 1, EntreeName = "Burger" } },
                new OrderEntreeItem { Entree = new EntreeDto { EntreePrice = 7.50m, StationId = 1, EntreeName = "Wrap" } }
            ],
            Sides =
            [
                new OrderSideItem { Side = new SideDto { SidePrice = 2.00m, StationId = 1, SideName = "Fries" } }
            ],
            Drinks =
            [
                new DrinkDto { DrinkPrice = 1.50m, LocationId = 1, DrinkName = "Water" }
            ]
        };

        var result = OrderCalculations.CalculateTotalPrice(order);
        Assert.Equal(19.00m, result);
    }

    [Fact]
    public void CalculateTotalPrice_EntreesWithOptionCosts_IncludesOptionCosts()
    {
        // Entree $8.00 + 2 extras × $0.50 = $9.00
        var entreeItem = new OrderEntreeItem
        {
            Entree = new EntreeDto { EntreePrice = 8.00m, StationId = 1, EntreeName = "Pasta" },
            SelectedOptions =
            [
                MakeFoodOption(optionTypeId: 1, numIncluded: 0, pricePerExtra: 0.50m),
                MakeFoodOption(optionTypeId: 1, numIncluded: 0, pricePerExtra: 0.50m)
            ]
        };

        var order = new BrowserOrder { Entrees = [entreeItem] };

        var result = OrderCalculations.CalculateTotalPrice(order);
        Assert.Equal(9.00m, result);
    }

    [Fact]
    public void CalculateTotalPrice_SidesWithOptionCosts_IncludesOptionCosts()
    {
        // Side $2.00 + 1 extra × $1.00 = $3.00
        var sideItem = new OrderSideItem
        {
            Side = new SideDto { SidePrice = 2.00m, StationId = 1, SideName = "Salad" },
            SelectedOptions =
            [
                MakeFoodOption(optionTypeId: 1, numIncluded: 0, pricePerExtra: 1.00m)
            ]
        };

        var order = new BrowserOrder { Sides = [sideItem] };

        var result = OrderCalculations.CalculateTotalPrice(order);
        Assert.Equal(3.00m, result);
    }

    // ---------------------------------------------------------------------------
    // CalculateTax
    // ---------------------------------------------------------------------------

    [Fact]
    public void CalculateTax_NullOrder_ReturnsZero()
    {
        var result = OrderCalculations.CalculateTax(null!);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateTax_SwipeOrder_ReturnsZero()
    {
        var order = new BrowserOrder
        {
            IsCardOrder = false,
            Drinks = [new DrinkDto { DrinkPrice = 10.00m, LocationId = 1, DrinkName = "Juice" }]
        };

        var result = OrderCalculations.CalculateTax(order);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void CalculateTax_CardOrder_ReturnsRoundedTax()
    {
        // Total = $10.00, tax = $10.00 × 0.0775 = $0.775 → rounded to $0.78
        var order = new BrowserOrder
        {
            IsCardOrder = true,
            Drinks = [new DrinkDto { DrinkPrice = 10.00m, LocationId = 1, DrinkName = "Juice" }]
        };

        var result = OrderCalculations.CalculateTax(order);
        Assert.Equal(0.78m, result);
    }

    [Fact]
    public void CalculateTax_CardOrderRoundsHalfUp()
    {
        // $20.00 × 0.0775 = $1.55 — rounds to exactly $1.55
        var order = new BrowserOrder
        {
            IsCardOrder = true,
            Drinks = [new DrinkDto { DrinkPrice = 20.00m, LocationId = 1, DrinkName = "Lemonade" }]
        };

        var result = OrderCalculations.CalculateTax(order);
        Assert.Equal(1.55m, result);
    }

    // ---------------------------------------------------------------------------
    // CalculateTotalSwipe
    // ---------------------------------------------------------------------------

    [Fact]
    public void CalculateTotalSwipe_NullOrder_ReturnsZero()
    {
        var result = OrderCalculations.CalculateTotalSwipe(null!);
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateTotalSwipe_EqualCounts_ReturnsThatCount()
    {
        var order = BuildSwipeOrder(entrees: 2, sides: 2, drinks: 2);
        Assert.Equal(2, OrderCalculations.CalculateTotalSwipe(order));
    }

    [Fact]
    public void CalculateTotalSwipe_DifferentCounts_ReturnsMinimum()
    {
        var order = BuildSwipeOrder(entrees: 3, sides: 1, drinks: 2);
        Assert.Equal(1, OrderCalculations.CalculateTotalSwipe(order));
    }

    [Fact]
    public void CalculateTotalSwipe_OneDrink_IsLimitingFactor()
    {
        var order = BuildSwipeOrder(entrees: 5, sides: 4, drinks: 1);
        Assert.Equal(1, OrderCalculations.CalculateTotalSwipe(order));
    }

    [Fact]
    public void CalculateTotalSwipe_EmptyLists_ReturnsZero()
    {
        var order = new BrowserOrder();
        Assert.Equal(0, OrderCalculations.CalculateTotalSwipe(order));
    }

    // ---------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------

    private static SelectedFoodOption MakeFoodOption(int optionTypeId, int numIncluded, decimal pricePerExtra)
    {
        return new SelectedFoodOption
        {
            Option = new FoodOptionDto { FoodOptionName = "Option" },
            OptionType = new FoodOptionTypeDto
            {
                Id = optionTypeId,
                NumIncluded = (short)numIncluded,
                FoodOptionPrice = pricePerExtra,
                FoodOptionTypeName = "Type"
            }
        };
    }

    private static BrowserOrder BuildSwipeOrder(int entrees, int sides, int drinks)
    {
        return new BrowserOrder
        {
            IsCardOrder = false,
            Entrees = Enumerable.Range(0, entrees)
                .Select(_ => new OrderEntreeItem { Entree = new EntreeDto { EntreePrice = 5m, StationId = 1, EntreeName = "E" } })
                .ToList(),
            Sides = Enumerable.Range(0, sides)
                .Select(_ => new OrderSideItem { Side = new SideDto { SidePrice = 2m, StationId = 1, SideName = "S" } })
                .ToList(),
            Drinks = Enumerable.Range(0, drinks)
                .Select(_ => new DrinkDto { DrinkPrice = 1m, LocationId = 1, DrinkName = "D" })
                .ToList()
        };
    }
}
