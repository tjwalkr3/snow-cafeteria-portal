#!/bin/bash

# Obtain a TLS cert via certbot in Docker and emit a Kubernetes secret manifest.

set -euo pipefail

if ! command -v docker >/dev/null 2>&1; then
  echo "docker is required" >&2
  exit 1
fi

if ! command -v script >/dev/null 2>&1; then
  echo "script (util-linux) is required" >&2
  exit 1
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CERTBOT_IMAGE="certbot/certbot:latest"
NAMESPACE="cafeteria"

read -rp "Enter the domain to issue a certificate for: " DOMAIN
if [[ -z "${DOMAIN}" ]]; then
  echo "Domain cannot be empty" >&2
  exit 1
fi

read -rp "Enter the secret name: " SECRET_NAME
if [[ -z "${SECRET_NAME}" ]]; then
  echo "Secret name cannot be empty" >&2
  exit 1
fi

OUTPUT_FILE="${SCRIPT_DIR}/${SECRET_NAME}.yml"

echo "This will start a one-off certbot container."
echo "When the DNS challenge appears, copy the TXT record and add it to your domain."
echo "Return here and press Enter inside the container when ready."
echo

SESSION_LOG=$(mktemp)
KEEP_LOG=0
cleanup() {
  if [[ ${KEEP_LOG} -eq 0 ]]; then
    rm -f "${SESSION_LOG}"
  else
    echo "Debug log kept at ${SESSION_LOG}" >&2
  fi
}
trap cleanup EXIT

CONTAINER_SCRIPT_FILE=$(mktemp)
cat > "${CONTAINER_SCRIPT_FILE}" <<'EOS'
set -e
DOMAIN="${CERT_DOMAIN}"
echo "Using domain: ${DOMAIN}"
certbot certonly \
  --manual \
  --preferred-challenges dns \
  --agree-tos \
  --register-unsafely-without-email \
  -d "${DOMAIN}"
echo "__CERT_START__"
base64 -w0 "/etc/letsencrypt/live/${DOMAIN}/fullchain.pem"
echo
echo "__CERT_END__"
echo "__KEY_START__"
base64 -w0 "/etc/letsencrypt/live/${DOMAIN}/privkey.pem"
echo
echo "__KEY_END__"
EOS

script -q -f "${SESSION_LOG}" -c \
  "docker run --rm -it --entrypoint sh -e CERT_DOMAIN='${DOMAIN}' -v '${CONTAINER_SCRIPT_FILE}:/tmp/certbot-script.sh' '${CERTBOT_IMAGE}' /tmp/certbot-script.sh"

rm -f "${CONTAINER_SCRIPT_FILE}"

CERT_DATA=$(awk '/__CERT_START__/{flag=1; next}/__CERT_END__/{flag=0}flag' "${SESSION_LOG}" | tr -d '\r\n')
KEY_DATA=$(awk '/__KEY_START__/{flag=1; next}/__KEY_END__/{flag=0}flag' "${SESSION_LOG}" | tr -d '\r\n')

if [[ -z "${CERT_DATA}" || -z "${KEY_DATA}" ]]; then
  KEEP_LOG=1
  echo "Failed to extract certificate material. Review ${SESSION_LOG}" >&2
  exit 1
fi

cat > "${OUTPUT_FILE}" <<EOF
apiVersion: v1
kind: Secret
metadata:
  name: ${SECRET_NAME}
  namespace: ${NAMESPACE}
type: kubernetes.io/tls
data:
  tls.crt: ${CERT_DATA}
  tls.key: ${KEY_DATA}
EOF

echo "Kubernetes TLS secret written to ${OUTPUT_FILE}"
echo "Apply it with: kubectl apply -f ${OUTPUT_FILE}"