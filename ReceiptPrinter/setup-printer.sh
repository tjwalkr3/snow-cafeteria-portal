#!/bin/bash

VENDOR_ID=04b8
PRODUCT_ID=0202
RULE_FILE="/etc/udev/rules.d/99-escpos.rules"
BLACKLIST_FILE="/etc/modprobe.d/blacklist-usblp.conf"

if [ "$EUID" -ne 0 ]; then
  echo "Please run as root or with sudo"
  exit 1
fi

echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"$VENDOR_ID\", ATTRS{idProduct}==\"$PRODUCT_ID\", MODE=\"0666\"" > "$RULE_FILE"

udevadm control --reload-rules
udevadm trigger

if lsmod | grep -q "usblp"; then
  rmmod usblp
fi

if [ ! -f "$BLACKLIST_FILE" ]; then
  echo "blacklist usblp" > "$BLACKLIST_FILE"
  update-initramfs -u
fi

echo "Setup complete! Unplug and replug the printer if needed."
