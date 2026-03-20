from datetime import datetime
from typing import List
import sys
import os

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from DTOs.PrintOrderDto import PrintOrderDto
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


def format_header(order_id: int, order_time: datetime) -> List[str]:
    """Format the receipt header with order ID and time."""
    lines = []

    lines.append(pad_line("=" * RECEIPT_WIDTH))

    order_id_text = f"ORDER #{order_id}"
    lines.append(pad_line(order_id_text.center(RECEIPT_WIDTH)))

    time_str = order_time.strftime("%m/%d/%Y %I:%M %p")
    lines.append(pad_line(time_str.center(RECEIPT_WIDTH)))

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


def format_order(order: PrintOrderDto) -> List[str]:
    """
    Main function to format a complete order for receipt printing.

    Args:
        order: PrintOrderDto containing all order information

    Returns:
        List of strings, each exactly 48 characters long, ready for printing
    """
    receipt_lines = []

    receipt_lines.extend(format_header(order.id, order.orderTime))

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
