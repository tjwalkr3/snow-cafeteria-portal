import os
import textwrap

from escpos.printer import Usb

DEFAULT_VENDOR_ID = int(os.environ.get("PRINTER_VENDOR_ID", "04b8"), 16)
DEFAULT_PRODUCT_ID = int(os.environ.get("PRINTER_PRODUCT_ID", "0202"), 16)
LINE_WIDTH = int(os.environ.get("PRINTER_LINE_WIDTH", "48"))


def open_printer(vendor_id=DEFAULT_VENDOR_ID, product_id=DEFAULT_PRODUCT_ID):
    return Usb(vendor_id, product_id, 0)


def print_lines(printer, lines):
    text = "\n".join(lines) + "\n\n"
    extra_feed = "\n" * 8
    printer.text(text)
    printer.text(extra_feed)
    printer.text("\x0c")


def hello_world():
    lorem_ipsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat."
    wrapped_lines = textwrap.wrap(lorem_ipsum, width=LINE_WIDTH)
    lines = ["Hello, world!", ""] + wrapped_lines
    printer = open_printer()
    print_lines(printer, lines)


def main():
    hello_world()


if __name__ == "__main__":
    main()
