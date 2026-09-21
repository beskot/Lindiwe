#!/usr/bin/env bash
set -e
./config.sh --url "${REPO_URL}" --token "${RUNNER_TOKEN}" \
  --name "arm64-runner" \
  --labels self-hosted,linux,dotnet-aot-arm64 \
  --unattended
./run.sh