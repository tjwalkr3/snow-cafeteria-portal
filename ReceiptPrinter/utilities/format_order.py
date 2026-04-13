from datetime import datetime
from typing import List
import sys
import os

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from DTOs.BrowserOrder import BrowserOrder
from DTOs.OrderEntreeItem import OrderEntreeItem
from DTOs.OrderSideItem import OrderSideItem
from DTOs.DrinkDto import DrinkDto
from DTOs.SelectedFoodOption import SelectedFoodOption


RECEIPT_WIDTH = 48


def pad_line(line: str) -> str:
    """Ensure a line is exactly RECEIPT_WIDTH characters by padding with spaces."""
    if len(line) > RECEIPT_WIDTH:
        return line[:RECEIPT_WIDTH]
    return line.ljust(RECEIPT_WIDTH)


def pad_line_with_count(line: str, count: int) -> str:
    count_text = f"x{count}"
    available = RECEIPT_WIDTH - len(count_text) - 1
    left = line[:available]
    return f"{left.ljust(available)} {count_text}"


def format_header(user_name: str, location_name: str, order_id: int) -> List[str]:
    """Format the receipt header with customer, location, and time."""
    lines = []

    lines.append(pad_line("=" * RECEIPT_WIDTH))

    display_name = user_name if user_name else "Unknown User"
    display_location = location_name if location_name else "Unknown Location"
    lines.append(pad_line(f"Customer: {display_name}"))
    lines.append(pad_line(f"Order Id: {order_id}"))
    lines.append(pad_line(f"Location: {display_location}"))

    time_str = datetime.now().strftime("%m/%d/%Y %I:%M %p")
    lines.append(pad_line(time_str))

    lines.append(pad_line("=" * RECEIPT_WIDTH))
    lines.append(pad_line(""))

    return lines


def format_selected_option(option: SelectedFoodOption) -> str:
    """Format a selected food option (indented under the food item)."""
    option_name = option.option.foodOptionName or "Unknown Option"
    indented_text = f"    - {option_name}"
    return pad_line(indented_text)


def format_entree_item(item: OrderEntreeItem, count: int = 1) -> List[str]:
    """Format an entree with its selected options."""
    lines = []

    item_name = item.entree.entreeName or f"Entree #{item.entree.id}"
    lines.append(pad_line_with_count(item_name, count))

    for option in item.selectedOptions:
        lines.append(format_selected_option(option))

    return lines


def format_side_item(item: OrderSideItem, count: int = 1) -> List[str]:
    """Format a side with its selected options."""
    lines = []

    item_name = item.side.sideName or f"Side #{item.side.id}"
    lines.append(pad_line_with_count(item_name, count))

    for option in item.selectedOptions:
        lines.append(format_selected_option(option))

    return lines


def format_drink_item(drink: DrinkDto, count: int = 1) -> str:
    """Format a drink."""
    drink_name = drink.drinkName or f"Drink #{drink.id}"
    return pad_line_with_count(drink_name, count)


def group_order_items(
    order: BrowserOrder,
) -> tuple[
    List[tuple[OrderEntreeItem, int]],
    List[tuple[OrderSideItem, int]],
    List[tuple[DrinkDto, int]],
]:
    def group(items, key_fn):
        counts = {}
        grouped = []
        for item in items:
            key = key_fn(item)
            if key in counts:
                idx = counts[key]
                current_item, current_count = grouped[idx]
                grouped[idx] = (current_item, current_count + 1)
            else:
                counts[key] = len(grouped)
                grouped.append((item, 1))
        return grouped

    entree_groups = group(
        order.entrees,
        lambda item: (
            item.entree.id,
            tuple(
                sorted(
                    (option.option.foodOptionName or "")
                    for option in item.selectedOptions
                )
            ),
        ),
    )
    side_groups = group(
        order.sides,
        lambda item: (
            item.side.id,
            tuple(
                sorted(
                    (option.option.foodOptionName or "")
                    for option in item.selectedOptions
                )
            ),
        ),
    )
    drink_groups = group(order.drinks, lambda item: item.id)
    return entree_groups, side_groups, drink_groups


def format_footer() -> List[str]:
    """Format the receipt footer."""
    lines = []

    lines.append(pad_line(""))
    lines.append(pad_line("=" * RECEIPT_WIDTH))

    return lines


def format_order(order: BrowserOrder, order_id: int) -> List[str]:
    """
    Main function to format a complete order for receipt printing.

    Args:
        order: BrowserOrder containing all order information

    Returns:
        List of strings, each exactly 48 characters long, ready for printing
    """
    receipt_lines = []

    location_name = order.location.locationName if order.location else ""
    receipt_lines.extend(format_header(order.userName, location_name, order_id))

    grouped_entrees, grouped_sides, grouped_drinks = group_order_items(order)

    for entree, count in grouped_entrees:
        receipt_lines.extend(format_entree_item(entree, count))

    for side, count in grouped_sides:
        receipt_lines.extend(format_side_item(side, count))

    for drink, count in grouped_drinks:
        receipt_lines.append(format_drink_item(drink, count))

    receipt_lines.extend(format_footer())

    for i, line in enumerate(receipt_lines):
        if len(line) != RECEIPT_WIDTH:
            receipt_lines[i] = pad_line(line)

    return receipt_lines
