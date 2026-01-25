import os

from escpos.printer import Usb

DEFAULT_VENDOR_ID = int(os.environ.get("PRINTER_VENDOR_ID", "04b8"), 16)
DEFAULT_PRODUCT_ID = int(os.environ.get("PRINTER_PRODUCT_ID", "0202"), 16)
LINE_WIDTH = int(os.environ.get("PRINTER_LINE_WIDTH", "48"))
LOGO_PATH = os.environ.get("PRINTER_LOGO_PATH", "snow_logo.bmp")


def open_printer(vendor_id=DEFAULT_VENDOR_ID, product_id=DEFAULT_PRODUCT_ID):
    return Usb(vendor_id, product_id)


def print_logo(printer, logo_path=LOGO_PATH):
    """
    Print the logo image to the receipt.
    The logo replaces the blank lines that would normally be printed.
    """
    if not os.path.isabs(logo_path):
        current_dir = os.path.dirname(os.path.abspath(__file__))
        logo_path = os.path.join(os.path.dirname(current_dir), logo_path)

    if os.path.exists(logo_path):
        printer.image(logo_path, center=True)
    else:
        printer.text("\n" * 8)


def print_lines(printer, lines):
    printer.text("\n".join(lines) + "\n\n")
    print_logo(printer)
    printer.text("\x0c")


def print_receipt(lines, vendor_id=DEFAULT_VENDOR_ID, product_id=DEFAULT_PRODUCT_ID):
    """
    Open printer, print lines, and properly close the connection.
    This ensures the printer resource is released after use.
    """
    printer = None
    try:
        printer = open_printer(vendor_id, product_id)
        print_lines(printer, lines)
    finally:
        if printer is not None:
            printer.close()
