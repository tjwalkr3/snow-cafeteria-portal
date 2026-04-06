#!/usr/bin/env bash
set -e

SERVICE_NAME=receipt-printer
SERVICE_FILE=/etc/systemd/system/${SERVICE_NAME}.service
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
VENV_DIR="${PROJECT_DIR}/.venv"
USER_NAME="${SUDO_USER:-$(id -un)}"

UVICORN="${VENV_DIR}/bin/uvicorn"
PYTHON="${VENV_DIR}/bin/python"

PORT=8000
HOST=0.0.0.0

LD_PATH="$HOME/.nix-profile/lib"

require_sudo() {
    if [[ $EUID -ne 0 ]]; then
        exec sudo "$0" "$@"
    fi
}

ensure_venv() {
    if [[ ! -x "$UVICORN" ]]; then
        echo "❌ Virtualenv not found. Run 'make install' first."
        exit 1
    fi
}

install_service() {
    require_sudo "$@"
    ensure_venv

    echo "📦 Installing systemd service: $SERVICE_NAME"
    USER_HOME=$(eval echo "~$USER_NAME")

    cat > "$SERVICE_FILE" <<EOF
[Unit]
Description=Receipt Printer FastAPI Service
After=network.target

[Service]
Type=simple
User=$USER_NAME
SupplementaryGroups=lp plugdev
WorkingDirectory=$PROJECT_DIR
Environment=LD_LIBRARY_PATH=${USER_HOME}/.nix-profile/lib
ExecStart=$UVICORN main:app --host $HOST --port $PORT
Restart=always
RestartSec=5

[Install]
WantedBy=multi-user.target
EOF

    systemctl daemon-reload
    systemctl enable "$SERVICE_NAME"

    echo "✅ Service installed and enabled"
}

uninstall_service() {
    require_sudo "$@"

    echo "🗑 Removing service: $SERVICE_NAME"

    systemctl stop "$SERVICE_NAME" || true
    systemctl disable "$SERVICE_NAME" || true
    rm -f "$SERVICE_FILE"
    systemctl daemon-reload

    echo "✅ Service removed"
}

start_service() {
    require_sudo "$@"
    systemctl start "$SERVICE_NAME"
}

stop_service() {
    require_sudo "$@"
    systemctl stop "$SERVICE_NAME"
}

restart_service() {
    require_sudo "$@"
    systemctl restart "$SERVICE_NAME"
}

status_service() {
    systemctl status "$SERVICE_NAME" --no-pager
}

logs_service() {
    journalctl -u "$SERVICE_NAME" -f
}

case "$1" in
    install)
        install_service "$@"
        ;;
    uninstall)
        uninstall_service "$@"
        ;;
    start)
        start_service "$@"
        ;;
    stop)
        stop_service "$@"
        ;;
    restart)
        restart_service "$@"
        ;;
    status)
        status_service
        ;;
    logs)
        logs_service
        ;;
    *)
        echo "Usage: $0 {install|uninstall|start|stop|restart|status|logs}"
        exit 1
        ;;
esac
