from datetime import datetime
from typing import List
import sys
import os

# Add parent directory to path to import DTOs
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from DTOs.PrintOrderDto import PrintOrderDto
from DTOs.FoodItemOrderDto import FoodItemOrderDto
from DTOs.FoodItemOptionDto import FoodItemOptionDto


RECEIPT_WIDTH = 48


def pad_line(line: str) -> str:
    """Ensure a line is exactly RECEIPT_WIDTH characters by padding with spaces."""
    if len(line) > RECEIPT_WIDTH:
        return line[:RECEIPT_WIDTH]
    return line.ljust(RECEIPT_WIDTH)


def format_header(order_id: int, order_time: datetime) -> List[str]:
    """Format the receipt header with order ID and time."""
    lines = []
    
    # Add a separator line
    lines.append(pad_line("=" * RECEIPT_WIDTH))
    
    # Add order ID centered
    order_id_text = f"ORDER #{order_id}"
    lines.append(pad_line(order_id_text.center(RECEIPT_WIDTH)))
    
    # Add order time centered
    time_str = order_time.strftime("%m/%d/%Y %I:%M %p")
    lines.append(pad_line(time_str.center(RECEIPT_WIDTH)))
    
    # Add a separator line
    lines.append(pad_line("=" * RECEIPT_WIDTH))
    
    # Add blank line
    lines.append(pad_line(""))
    
    return lines


def format_food_item_option(option: FoodItemOptionDto) -> str:
    """Format a food item option (indented under the food item)."""
    option_name = option.FoodOptionName or "Unknown Option"
    # Indent with 4 spaces
    indented_text = f"    - {option_name}"
    return pad_line(indented_text)


def format_food_item(item: FoodItemOrderDto) -> List[str]:
    """Format a food item with its price and options."""
    lines = []
    
    # Determine which cost to display (prefer swipe cost)
    if item.SwipeCost is not None:
        swipe_word = "swipe" if item.SwipeCost == 1 else "swipes"
        cost_str = f"{item.SwipeCost} {swipe_word}"
    elif item.CardCost is not None:
        cost_str = f"${item.CardCost:.2f}"
    else:
        cost_str = "$0.00"
    
    # Get item name - using Station ID as placeholder since we don't have the actual name
    item_name = f"Item #{item.Id}"
    if item.Special:
        item_name += " (Special)"
    
    # Format the line with name on left and price on right
    # Calculate available space for the name
    available_space = RECEIPT_WIDTH - len(cost_str) - 1  # -1 for space between
    
    if len(item_name) > available_space:
        item_name = item_name[:available_space]
    
    # Create line with name and price justified
    line = item_name.ljust(available_space) + " " + cost_str
    lines.append(pad_line(line))
    
    # Add options below the item (indented)
    for option in item.Options:
        lines.append(format_food_item_option(option))
    
    return lines


def format_footer(total_price: float) -> List[str]:
    """Format the receipt footer with total price."""
    lines = []
    
    # Add blank line before footer
    lines.append(pad_line(""))
    
    # Add separator line
    lines.append(pad_line("-" * RECEIPT_WIDTH))
    
    # Format total price
    total_str = f"${total_price:.2f}"
    total_label = "TOTAL:"
    
    # Right-align the total
    available_space = RECEIPT_WIDTH - len(total_str) - 1
    line = total_label.ljust(available_space) + " " + total_str
    lines.append(pad_line(line))
    
    # Add separator line
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
    
    # Add header
    receipt_lines.extend(format_header(order.OrderId, order.OrderTime))
    
    # Add food items
    for food_item in order.FoodItems:
        receipt_lines.extend(format_food_item(food_item))
    
    # Add footer with total
    receipt_lines.extend(format_footer(order.TotalPrice))
    
    # Verify all lines are exactly RECEIPT_WIDTH characters
    for i, line in enumerate(receipt_lines):
        if len(line) != RECEIPT_WIDTH:
            # This should never happen, but just in case
            receipt_lines[i] = pad_line(line)
    
    return receipt_lines
