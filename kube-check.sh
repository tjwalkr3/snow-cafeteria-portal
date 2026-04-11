set -euo pipefail

BASE_DOMAIN="example.com"
CONTAINER_VERSION="dev"
ACME_EMAIL="admin@example.com"
export BASE_DOMAIN CONTAINER_VERSION ACME_EMAIL

failed=0

for f in kube/*.yml; do
	echo "== $f =="

	if envsubst '${BASE_DOMAIN} ${CONTAINER_VERSION} ${ACME_EMAIL}' < "$f" | \
		kubectl apply --dry-run=client --validate=false -f - -o name >/dev/null; then
		echo "OK"
	else
		echo "FAIL"
		failed=1
	fi
done

echo "Completed with failed=$failed"