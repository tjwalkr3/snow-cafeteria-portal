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
        img = Image.open(logo_path)
        printer.set(align="center")
        printer.image(img)
        printer.set(align="left")
        printer.text("\n")
    else:
        printer.text("\n")


def print_lines(printer, lines):
    print_logo(printer)
    printer.text("\n".join(lines))
    try:
        printer.cut(mode="PART")
    except Exception:
        # Fallback for printers/drivers that do not implement cut() cleanly.
        printer._raw(b"\x1d\x56\x01")


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
