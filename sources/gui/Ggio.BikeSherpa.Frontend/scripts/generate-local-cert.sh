#!/usr/bin/env sh

set -e

HOST="dev.bike.local"
CERT_FILE="dev.bike.local.pem"
KEY_FILE="dev.bike.local-key.pem"

mkcert --install
mkcert -cert-file "$CERT_FILE" -key-file "$KEY_FILE" "$HOST"

echo "Generated:"
echo "  $CERT_FILE"
echo "  $KEY_FILE"