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


def format_header(user_name: str, location_name: str) -> List[str]:
    """Format the receipt header with customer, location, and time."""
    lines = []

    lines.append(pad_line("=" * RECEIPT_WIDTH))

    display_name = user_name if user_name else "Unknown User"
    display_location = location_name if location_name else "Unknown Location"
    lines.append(pad_line(f"Customer: {display_name}"))
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


def format_entree_item(item: OrderEntreeItem) -> List[str]:
    """Format an entree with its selected options."""
    lines = []

    item_name = item.entree.entreeName or f"Entree #{item.entree.id}"
    lines.append(pad_line(item_name))

    for option in item.selectedOptions:
        lines.append(format_selected_option(option))

    return lines


def format_side_item(item: OrderSideItem) -> List[str]:
    """Format a side with its selected options."""
    lines = []

    item_name = item.side.sideName or f"Side #{item.side.id}"
    lines.append(pad_line(item_name))

    for option in item.selectedOptions:
        lines.append(format_selected_option(option))

    return lines


def format_drink_item(drink: DrinkDto) -> str:
    """Format a drink."""
    drink_name = drink.drinkName or f"Drink #{drink.id}"
    return pad_line(drink_name)


def format_footer() -> List[str]:
    """Format the receipt footer."""
    lines = []

    lines.append(pad_line(""))
    lines.append(pad_line("=" * RECEIPT_WIDTH))

    return lines


def format_order(order: BrowserOrder) -> List[str]:
    """
    Main function to format a complete order for receipt printing.

    Args:
        order: BrowserOrder containing all order information

    Returns:
        List of strings, each exactly 48 characters long, ready for printing
    """
    receipt_lines = []

    location_name = order.location.locationName if order.location else ""
    receipt_lines.extend(format_header(order.userName, location_name))

    for entree in order.entrees:
        receipt_lines.extend(format_entree_item(entree))

    for side in order.sides:
        receipt_lines.extend(format_side_item(side))

    for drink in order.drinks:
        receipt_lines.append(format_drink_item(drink))

    receipt_lines.extend(format_footer())

    for i, line in enumerate(receipt_lines):
        if len(line) != RECEIPT_WIDTH:
            receipt_lines[i] = pad_line(line)

    return receipt_lines
