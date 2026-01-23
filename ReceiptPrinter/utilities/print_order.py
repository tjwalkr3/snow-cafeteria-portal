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
