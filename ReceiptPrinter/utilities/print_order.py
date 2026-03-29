import os
from PIL import Image
from escpos.printer import Usb
import usb.core


def _parse_hex_id(value, fallback=None):
    if value is None or value == "":
        return fallback
    normalized = value.lower().removeprefix("0x")
    return int(normalized, 16)


DEFAULT_VENDOR_ID = _parse_hex_id(os.environ.get("PRINTER_VENDOR_ID", "04b8"))
DEFAULT_PRODUCT_ID = _parse_hex_id(os.environ.get("PRINTER_PRODUCT_ID"), None)
LINE_WIDTH = int(os.environ.get("PRINTER_LINE_WIDTH", "48"))
LOGO_PATH = os.environ.get("PRINTER_LOGO_PATH", "snow_logo.bmp")


def open_printer(vendor_id=DEFAULT_VENDOR_ID, product_id=DEFAULT_PRODUCT_ID):
    if product_id is None:
        detected = usb.core.find(idVendor=vendor_id)
        if detected is None:
            raise RuntimeError(
                "Could not auto-detect Epson printer product ID. "
                "Set PRINTER_PRODUCT_ID to the printer USB product ID in hex (from lsusb)."
            )

        # pyusb's runtime Device exposes idProduct, but type stubs may not.
        # Use getattr so static analyzers do not fail on missing stub attributes.
        product_id_value = getattr(detected, "idProduct", None)
        if product_id_value is None:
            raise RuntimeError(
                "Detected USB device but could not read idProduct. "
                "Set PRINTER_PRODUCT_ID to the printer USB product ID in hex (from lsusb)."
            )
        product_id = int(product_id_value)

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
        printer.text("\n" * 2)
        img = Image.open(logo_path)
        new_height = img.height // 2
        img_resized = img.resize((img.width, new_height), Image.Resampling.LANCZOS)
        printer.set(align="center")
        printer.image(img_resized)
        printer.set(align="left")
        printer.text("\n" * 1)
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
