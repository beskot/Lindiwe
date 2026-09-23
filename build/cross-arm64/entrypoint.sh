#!/usr/bin/env bash
set -euo pipefail

: "${RUNNER_URL:?RUNNER_URL is required}"
: "${RUNNER_TOKEN:?RUNNER_TOKEN is required}"

cleanup() {
  ./config.sh remove --unattended --token "$RUNNER_TOKEN" || true
}

trap 'cleanup; exit 130' INT
trap 'cleanup; exit 143' TERM

./config.sh \
  --unattended \
  --url "$RUNNER_URL" \
  --token "$RUNNER_TOKEN" \
  --name "${RUNNER_NAME:-$(hostname)}" \
  --labels "${RUNNER_LABELS:-native-aot}" \
  --work _work \
  --ephemeral \
  --replace

./run.sh