#!/bin/bash

ENV=${1:-dev}
KEYVAULT_NAME="robot-orch-kv-${ENV}"
CERT_NAME="cert"

echo "Deploying cert ${CERT_NAME} to ${KEYVAULT_NAME}"
az keyvault certificate create --vault-name "${KEYVAULT_NAME}" -n "${CERT_NAME}" \
                          -p "$(az keyvault certificate get-default-policy)"
