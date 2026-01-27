import textwrap
import os

from escpos.printer import Usb

DEFAULT_VENDOR_ID = int(os.environ.get("PRINTER_VENDOR_ID", "04b8"), 16)
DEFAULT_PRODUCT_ID = int(os.environ.get("PRINTER_PRODUCT_ID", "0202"), 16)
LINE_WIDTH = int(os.environ.get("PRINTER_LINE_WIDTH", "48"))


def open_printer(vendor_id=DEFAULT_VENDOR_ID, product_id=DEFAULT_PRODUCT_ID):
    return Usb(vendor_id, product_id)


def print_lines(printer, lines):
    printer.text("\n".join(lines) + "\n\n")
    printer.text("\n" * 8)
    printer.text("\x0c")


def hello_world():
    lorem_ipsum = (
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, "
        "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. "
        "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris "
        "nisi ut aliquip ex ea commodo consequat."
    )
    wrapped_lines = textwrap.wrap(lorem_ipsum, width=LINE_WIDTH)
    lines = ["Hello, world!", ""] + wrapped_lines
    return lines


def main():
    lines = hello_world()
    printer = open_printer()
    print_lines(printer, lines)


if __name__ == "__main__":
    main()
