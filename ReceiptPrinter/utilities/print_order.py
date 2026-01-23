import os

from escpos.printer import Usb

DEFAULT_VENDOR_ID = int(os.environ.get("PRINTER_VENDOR_ID", "04b8"), 16)
DEFAULT_PRODUCT_ID = int(os.environ.get("PRINTER_PRODUCT_ID", "0202"), 16)
LINE_WIDTH = int(os.environ.get("PRINTER_LINE_WIDTH", "48"))


def open_printer(vendor_id=DEFAULT_VENDOR_ID, product_id=DEFAULT_PRODUCT_ID):
    return Usb(vendor_id, product_id, 0)


def print_lines(printer, lines):
    printer.text("\n".join(lines) + "\n\n")
    printer.text("\n" * 8)
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
