"""Tests for format_order utility functions."""

import pytest
from datetime import datetime
from decimal import Decimal
from utilities.format_order import (
    pad_line,
    format_header,
    format_selected_option,
    format_entree_item,
    format_side_item,
    format_drink_item,
    format_footer,
    format_order,
    RECEIPT_WIDTH,
)
from DTOs.PrintOrderDto import PrintOrderDto
from DTOs.OrderEntreeItem import OrderEntreeItem
from DTOs.OrderSideItem import OrderSideItem
from DTOs.DrinkDto import DrinkDto
from DTOs.EntreeDto import EntreeDto
from DTOs.SideDto import SideDto
from DTOs.SelectedFoodOption import SelectedFoodOption
from DTOs.FoodOptionDto import FoodOptionDto
from DTOs.FoodOptionTypeDto import FoodOptionTypeDto


def make_option_type(**kwargs) -> FoodOptionTypeDto:
    defaults = dict(
        id=1,
        foodOptionTypeName="Type",
        requiredAmount=0,
        includedAmount=0,
        maxAmount=1,
        foodOptionPrice=Decimal("0"),
    )
    return FoodOptionTypeDto(**{**defaults, **kwargs})


class TestPadLine:
    """Tests for pad_line function."""

    def test_pad_line_short_string(self):
        """Test padding a string shorter than RECEIPT_WIDTH."""
        result = pad_line("Hello")
        assert len(result) == RECEIPT_WIDTH
        assert result.startswith("Hello")
        assert result.endswith(" ")

    def test_pad_line_exact_width(self):
        """Test a string that is exactly RECEIPT_WIDTH."""
        text = "x" * RECEIPT_WIDTH
        result = pad_line(text)
        assert len(result) == RECEIPT_WIDTH
        assert result == text

    def test_pad_line_too_long(self):
        """Test truncating a string longer than RECEIPT_WIDTH."""
        text = "x" * (RECEIPT_WIDTH + 10)
        result = pad_line(text)
        assert len(result) == RECEIPT_WIDTH
        assert result == "x" * RECEIPT_WIDTH

    def test_pad_line_empty_string(self):
        """Test padding an empty string."""
        result = pad_line("")
        assert len(result) == RECEIPT_WIDTH
        assert result == " " * RECEIPT_WIDTH


class TestFormatHeader:
    """Tests for format_header function."""

    def test_format_header_basic(self):
        """Test basic header formatting."""
        order_id = 12345
        order_time = datetime(2026, 1, 25, 14, 30, 0)
        result = format_header(order_id, order_time)

        assert len(result) == 5  # separator, order ID, time, separator, blank line
        assert all(len(line) == RECEIPT_WIDTH for line in result)
        assert "ORDER #12345" in result[1]
        assert "01/25/2026 02:30 PM" in result[2]
        assert "=" * RECEIPT_WIDTH == result[0].strip()

    def test_format_header_large_order_id(self):
        """Test header with a large order ID."""
        order_id = 999999
        order_time = datetime(2026, 12, 31, 23, 59, 59)
        result = format_header(order_id, order_time)

        assert "ORDER #999999" in result[1]
        assert "12/31/2026 11:59 PM" in result[2]

    def test_format_header_single_digit_order_id(self):
        """Test header with single digit order ID."""
        order_id = 1
        order_time = datetime(2026, 1, 1, 0, 0, 0)
        result = format_header(order_id, order_time)

        assert "ORDER #1" in result[1]
        assert all(len(line) == RECEIPT_WIDTH for line in result)


class TestFormatSelectedOption:
    """Tests for format_selected_option function."""

    def test_format_selected_option_basic(self):
        """Test basic option formatting."""
        option = SelectedFoodOption(
            option=FoodOptionDto(id=1, foodOptionName="Extra Cheese"),
            optionType=make_option_type(),
        )
        result = format_selected_option(option)

        assert len(result) == RECEIPT_WIDTH
        assert "    - Extra Cheese" in result

    def test_format_selected_option_long_name(self):
        """Test option with a very long name."""
        option = SelectedFoodOption(
            option=FoodOptionDto(id=1, foodOptionName="x" * 100),
            optionType=make_option_type(),
        )
        result = format_selected_option(option)

        assert len(result) == RECEIPT_WIDTH

    def test_format_selected_option_empty_name(self):
        """Test option with empty string as name falls back to Unknown Option."""
        option = SelectedFoodOption(
            option=FoodOptionDto(id=1, foodOptionName=""),
            optionType=make_option_type(),
        )
        result = format_selected_option(option)

        assert len(result) == RECEIPT_WIDTH
        assert "Unknown Option" in result


class TestFormatEntreeItem:
    """Tests for format_entree_item function."""

    def test_format_entree_item_no_options(self):
        """Test formatting an entree without options."""
        item = OrderEntreeItem(
            entree=EntreeDto(
                id=1, stationId=1, entreeName="Cheeseburger", entreePrice=Decimal("5.99")
            ),
            selectedOptions=[],
        )
        result = format_entree_item(item)

        assert len(result) == 1
        assert "Cheeseburger" in result[0]
        assert all(len(line) == RECEIPT_WIDTH for line in result)

    def test_format_entree_item_with_options(self):
        """Test formatting an entree with options."""
        item = OrderEntreeItem(
            entree=EntreeDto(
                id=1, stationId=1, entreeName="Deli Sandwich", entreePrice=Decimal("6.99")
            ),
            selectedOptions=[
                SelectedFoodOption(
                    option=FoodOptionDto(id=1, foodOptionName="White Bread"),
                    optionType=make_option_type(id=1, foodOptionTypeName="Bread"),
                ),
                SelectedFoodOption(
                    option=FoodOptionDto(id=2, foodOptionName="Ham"),
                    optionType=make_option_type(id=2, foodOptionTypeName="Protein"),
                ),
                SelectedFoodOption(
                    option=FoodOptionDto(id=3, foodOptionName="Lettuce"),
                    optionType=make_option_type(id=3, foodOptionTypeName="Toppings"),
                ),
            ],
        )
        result = format_entree_item(item)

        assert len(result) == 4  # entree name + 3 options
        assert "Deli Sandwich" in result[0]
        assert "White Bread" in result[1]
        assert "Ham" in result[2]
        assert "Lettuce" in result[3]
        assert all(len(line) == RECEIPT_WIDTH for line in result)

    def test_format_entree_item_empty_name(self):
        """Test formatting an entree with no name."""
        item = OrderEntreeItem(
            entree=EntreeDto(id=42, stationId=1, entreeName="", entreePrice=Decimal("0")),
            selectedOptions=[],
        )
        result = format_entree_item(item)

        assert "Entree #42" in result[0]


class TestFormatSideItem:
    """Tests for format_side_item function."""

    def test_format_side_item_no_options(self):
        """Test formatting a side without options."""
        item = OrderSideItem(
            side=SideDto(id=1, stationId=1, sideName="Fries", sidePrice=Decimal("2.99")),
            selectedOptions=[],
        )
        result = format_side_item(item)

        assert len(result) == 1
        assert "Fries" in result[0]
        assert all(len(line) == RECEIPT_WIDTH for line in result)

    def test_format_side_item_with_options(self):
        """Test formatting a side with options."""
        item = OrderSideItem(
            side=SideDto(id=1, stationId=1, sideName="Salad", sidePrice=Decimal("3.99")),
            selectedOptions=[
                SelectedFoodOption(
                    option=FoodOptionDto(id=1, foodOptionName="Ranch"),
                    optionType=make_option_type(id=1, foodOptionTypeName="Dressing"),
                ),
            ],
        )
        result = format_side_item(item)

        assert len(result) == 2
        assert "Salad" in result[0]
        assert "Ranch" in result[1]

    def test_format_side_item_empty_name(self):
        """Test formatting a side with no name."""
        item = OrderSideItem(
            side=SideDto(id=7, stationId=1, sideName="", sidePrice=Decimal("0")),
            selectedOptions=[],
        )
        result = format_side_item(item)

        assert "Side #7" in result[0]


class TestFormatDrinkItem:
    """Tests for format_drink_item function."""

    def test_format_drink_item_basic(self):
        """Test basic drink formatting."""
        drink = DrinkDto(id=1, locationId=1, drinkName="Coke", drinkPrice=Decimal("1.99"))
        result = format_drink_item(drink)

        assert len(result) == RECEIPT_WIDTH
        assert "Coke" in result

    def test_format_drink_item_empty_name(self):
        """Test formatting a drink with no name."""
        drink = DrinkDto(id=5, locationId=1, drinkName="", drinkPrice=Decimal("0"))
        result = format_drink_item(drink)

        assert len(result) == RECEIPT_WIDTH
        assert "Drink #5" in result


class TestFormatFooter:
    """Tests for format_footer function."""

    def test_format_footer(self):
        """Test footer formatting."""
        result = format_footer()

        assert len(result) == 2  # blank line + separator
        assert all(len(line) == RECEIPT_WIDTH for line in result)
        assert result[0].strip() == ""
        assert "=" * RECEIPT_WIDTH == result[1].strip()


class TestFormatOrder:
    """Tests for format_order function."""

    def test_format_order_complete(self):
        """Test formatting a complete order with entrees, sides, and drinks."""
        order = PrintOrderDto(
            id=12345,
            orderTime=datetime(2026, 1, 25, 14, 30, 0),
            isCardOrder=True,
            stationId=1,
            stationName="Main Station",
            entrees=[
                OrderEntreeItem(
                    entree=EntreeDto(
                        id=1,
                        stationId=1,
                        entreeName="Cheeseburger",
                        entreePrice=Decimal("5.99"),
                    ),
                    selectedOptions=[
                        SelectedFoodOption(
                            option=FoodOptionDto(id=1, foodOptionName="Extra Cheese"),
                            optionType=make_option_type(
                                id=1,
                                foodOptionTypeName="Cheese",
                                foodOptionPrice=Decimal("0.50"),
                            ),
                        ),
                        SelectedFoodOption(
                            option=FoodOptionDto(id=2, foodOptionName="No Onions"),
                            optionType=make_option_type(id=2, foodOptionTypeName="Toppings"),
                        ),
                    ],
                ),
            ],
            sides=[
                OrderSideItem(
                    side=SideDto(
                        id=1, stationId=1, sideName="Fries", sidePrice=Decimal("2.99")
                    ),
                    selectedOptions=[],
                ),
            ],
            drinks=[
                DrinkDto(id=1, locationId=1, drinkName="Coke", drinkPrice=Decimal("1.99")),
            ],
        )
        result = format_order(order)

        assert all(len(line) == RECEIPT_WIDTH for line in result)
        # header (5) + entree (3) + side (1) + drink (1) + footer (2) = 12
        assert len(result) == 12

        receipt_text = "\n".join(result)
        assert "ORDER #12345" in receipt_text
        assert "Cheeseburger" in receipt_text
        assert "Extra Cheese" in receipt_text
        assert "No Onions" in receipt_text
        assert "Fries" in receipt_text
        assert "Coke" in receipt_text

    def test_format_order_empty(self):
        """Test formatting an order with no items."""
        order = PrintOrderDto(id=1, orderTime=datetime(2026, 1, 1, 0, 0, 0))
        result = format_order(order)

        assert all(len(line) == RECEIPT_WIDTH for line in result)
        # header (5) + footer (2) = 7
        assert len(result) == 7

    def test_format_order_only_drinks(self):
        """Test formatting an order with only drinks."""
        order = PrintOrderDto(
            id=2,
            orderTime=datetime(2026, 3, 10, 9, 0, 0),
            drinks=[
                DrinkDto(id=1, locationId=1, drinkName="Water", drinkPrice=Decimal("0")),
                DrinkDto(id=2, locationId=1, drinkName="OJ", drinkPrice=Decimal("2.49")),
            ],
        )
        result = format_order(order)

        assert all(len(line) == RECEIPT_WIDTH for line in result)
        # header (5) + 2 drinks + footer (2) = 9
        assert len(result) == 9

        receipt_text = "\n".join(result)
        assert "Water" in receipt_text
        assert "OJ" in receipt_text
