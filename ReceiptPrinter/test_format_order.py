"""Tests for format_order utility functions."""

import pytest
from datetime import datetime
from utilities.format_order import (
    pad_line,
    format_header,
    format_food_item_option,
    format_food_item,
    format_footer,
    format_order,
    RECEIPT_WIDTH,
)
from DTOs.PrintOrderDto import PrintOrderDto
from DTOs.FoodItemDto import FoodItemDto
from DTOs.FoodItemOptionDto import FoodItemOptionDto


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


class TestFormatFoodItemOption:
    """Tests for format_food_item_option function."""

    def test_format_food_item_option_basic(self):
        """Test basic option formatting."""
        option = FoodItemOptionDto(id=1, foodItemId=100, foodOptionName="Extra Cheese")
        result = format_food_item_option(option)

        assert len(result) == RECEIPT_WIDTH
        assert "    - Extra Cheese" in result

    def test_format_food_item_option_long_name(self):
        """Test option with a very long name."""
        option = FoodItemOptionDto(id=1, foodItemId=100, foodOptionName="x" * 100)
        result = format_food_item_option(option)

        assert len(result) == RECEIPT_WIDTH

    def test_format_food_item_option_none_name(self):
        """Test option with None as name."""
        option = FoodItemOptionDto(id=1, foodItemId=100, foodOptionName=None)
        result = format_food_item_option(option)

        assert len(result) == RECEIPT_WIDTH
        assert "Unknown Option" in result


class TestFormatFoodItem:
    """Tests for format_food_item function."""

    def test_format_food_item_no_options(self):
        """Test formatting a food item without options."""
        item = FoodItemDto(id=1, name="Cheeseburger", special=False, options=[])
        result = format_food_item(item)

        assert len(result) == 1
        assert "Cheeseburger" in result[0]
        assert all(len(line) == RECEIPT_WIDTH for line in result)

    def test_format_food_item_with_options(self):
        """Test formatting a food item with options."""
        item = FoodItemDto(
            id=1,
            name="Deli Sandwich",
            special=False,
            options=[
                FoodItemOptionDto(id=1, foodItemId=1, foodOptionName="White Bread"),
                FoodItemOptionDto(id=2, foodItemId=1, foodOptionName="Ham"),
                FoodItemOptionDto(id=3, foodItemId=1, foodOptionName="Lettuce"),
            ],
        )
        result = format_food_item(item)

        assert len(result) == 4  # item name + 3 options
        assert "Deli Sandwich" in result[0]
        assert "White Bread" in result[1]
        assert "Ham" in result[2]
        assert "Lettuce" in result[3]
        assert all(len(line) == RECEIPT_WIDTH for line in result)

    def test_format_food_item_special(self):
        """Test formatting a special food item."""
        item = FoodItemDto(id=1, name="Pizza", special=True, options=[])
        result = format_food_item(item)

        assert "(Special)" in result[0]
        assert "Pizza" in result[0]

    def test_format_food_item_empty_name(self):
        """Test formatting a food item with no name."""
        item = FoodItemDto(id=42, name="", special=False, options=[])
        result = format_food_item(item)

        assert "Item #42" in result[0]


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
        """Test formatting a complete order."""
        order = PrintOrderDto(
            id=12345,
            orderTime=datetime(2026, 1, 25, 14, 30, 0),
            foodItems=[
                FoodItemDto(
                    id=1,
                    name="Cheeseburger",
                    special=False,
                    options=[
                        FoodItemOptionDto(
                            id=1, foodItemId=1, foodOptionName="Extra Cheese"
                        ),
                        FoodItemOptionDto(
                            id=2, foodItemId=1, foodOptionName="No Onions"
                        ),
                    ],
                ),
                FoodItemDto(id=2, name="Fries", special=False, options=[]),
            ],
        )
        result = format_order(order)

        # Check all lines are correct width
        assert all(len(line) == RECEIPT_WIDTH for line in result)

        # Check structure: header (5) + item1 (3) + item2 (1) + footer (2) = 11
        assert len(result) == 11

        # Verify content
        receipt_text = "\n".join(result)
        assert "ORDER #12345" in receipt_text
        assert "Cheeseburger" in receipt_text
        assert "Extra Cheese" in receipt_text
        assert "No Onions" in receipt_text
        assert "Fries" in receipt_text

    def test_format_order_empty_items(self):
        """Test formatting an order with no items."""
        order = PrintOrderDto(
            id=1, orderTime=datetime(2026, 1, 1, 0, 0, 0), foodItems=[]
        )
        result = format_order(order)

        assert all(len(line) == RECEIPT_WIDTH for line in result)
        # header (5) + footer (2) = 7
        assert len(result) == 7

    def test_format_order_multiple_special_items(self):
        """Test formatting an order with multiple special items."""
        order = PrintOrderDto(
            id=777,
            orderTime=datetime(2026, 6, 15, 12, 0, 0),
            foodItems=[
                FoodItemDto(
                    id=1,
                    name="Custom Pizza",
                    special=True,
                    options=[
                        FoodItemOptionDto(
                            id=1, foodItemId=1, foodOptionName="Pepperoni"
                        ),
                        FoodItemOptionDto(
                            id=2, foodItemId=1, foodOptionName="Mushrooms"
                        ),
                        FoodItemOptionDto(id=3, foodItemId=1, foodOptionName="Olives"),
                    ],
                ),
                FoodItemDto(id=2, name="Special Salad", special=True, options=[]),
            ],
        )
        result = format_order(order)

        assert all(len(line) == RECEIPT_WIDTH for line in result)
        receipt_text = "\n".join(result)
        assert "Custom Pizza (Special)" in receipt_text
        assert "Special Salad (Special)" in receipt_text
        assert "Pepperoni" in receipt_text
        assert "Mushrooms" in receipt_text
        assert "Olives" in receipt_text
