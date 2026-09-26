#!/usr/bin/env bash
set -Eeuo pipefail

cd /actions-runner

: "${RUNNER_URL:?RUNNER_URL is required}"
: "${RUNNER_TOKEN:?RUNNER_TOKEN is required}"

RUNNER_NAME="${RUNNER_NAME:-$(hostname)}"
RUNNER_LABELS="${RUNNER_LABELS:-native-aot}"
RUNNER_WORKDIR="${RUNNER_WORKDIR:-_work}"

if [[ ! -f .runner ]]; then
  echo "Configuring GitHub Actions runner: ${RUNNER_NAME}"

  ./config.sh \
    --unattended \
    --url "${RUNNER_URL}" \
    --token "${RUNNER_TOKEN}" \
    --name "${RUNNER_NAME}" \
    --labels "${RUNNER_LABELS}" \
    --work "${RUNNER_WORKDIR}" \
    --replace
else
  echo "GitHub Actions runner is already configured; starting it."
fi

exec ./run.sh