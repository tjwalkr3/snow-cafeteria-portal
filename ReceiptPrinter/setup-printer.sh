#!/bin/bash

set -euo pipefail

VENDOR_ID="${PRINTER_VENDOR_ID:-04b8}"
PRODUCT_ID="${PRINTER_PRODUCT_ID:-}"
RULE_FILE="/etc/udev/rules.d/99-escpos.rules"
BLACKLIST_FILE="/etc/modprobe.d/blacklist-usblp.conf"

detect_product_id() {
  if [ -n "$PRODUCT_ID" ]; then
    echo "${PRODUCT_ID,,}"
    return 0
  fi

  if ! command -v lsusb >/dev/null 2>&1; then
    return 1
  fi

  local first_epson_pid=""
  while IFS= read -r line; do
    local id
    local pid
    local description

    id=$(awk '{print $6}' <<< "$line")
    description=$(cut -d' ' -f7- <<< "$line")

    if [[ "${id,,}" =~ ^${VENDOR_ID,,}:[0-9a-f]{4}$ ]]; then
      pid="${id#*:}"
      pid="${pid,,}"

      if [ -z "$first_epson_pid" ]; then
        first_epson_pid="$pid"
      fi

      if grep -Eqi "TM-?m30III" <<< "$description"; then
        echo "$pid"
        return 0
      fi
    fi
  done < <(lsusb)

  if [ -n "$first_epson_pid" ]; then
    echo "$first_epson_pid"
    return 0
  fi

  return 1
}

if [ "$EUID" -ne 0 ]; then
  echo "Please run as root or with sudo"
  exit 1
fi

if detected_product_id="$(detect_product_id)"; then
  echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"$VENDOR_ID\", ATTRS{idProduct}==\"$detected_product_id\", MODE=\"0666\"" > "$RULE_FILE"
  echo "Configured udev rule for Epson vendor $VENDOR_ID product $detected_product_id"
else
  echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"$VENDOR_ID\", MODE=\"0666\"" > "$RULE_FILE"
  echo "Could not auto-detect product ID. Configured vendor-wide Epson rule for $VENDOR_ID."
  echo "Set PRINTER_PRODUCT_ID if you want to target a single USB product ID."
fi

udevadm control --reload-rules
udevadm trigger

if lsmod | grep -q "usblp"; then
  rmmod usblp
fi

if [ ! -f "$BLACKLIST_FILE" ] || ! grep -q "^blacklist usblp$" "$BLACKLIST_FILE"; then
  echo "blacklist usblp" > "$BLACKLIST_FILE"
  if command -v update-initramfs >/dev/null 2>&1; then
    update-initramfs -u
  else
    echo "update-initramfs not found; reboot may be required for blacklist changes to apply."
  fi
fi

echo "Setup complete! Unplug and replug the printer if needed."
