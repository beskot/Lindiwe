# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble-aot

ARG RUNNER_VERSION=2.337.0
ARG TARGETARCH

ENV DEBIAN_FRONTEND=noninteractive \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1 \
    RUNNER_ALLOW_RUNASROOT=1

RUN rm -f /etc/apt/sources.list.d/ubuntu.sources && \
    { \
      echo "Types: deb"; \
      echo "URIs: http://archive.ubuntu.com/ubuntu/"; \
      echo "Suites: noble noble-updates noble-backports"; \
      echo "Components: main restricted universe multiverse"; \
      echo "Architectures: amd64"; \
      echo "Signed-By: /usr/share/keyrings/ubuntu-archive-keyring.gpg"; \
      echo ""; \
      echo "Types: deb"; \
      echo "URIs: http://security.ubuntu.com/ubuntu/"; \
      echo "Suites: noble-security"; \
      echo "Components: main restricted universe multiverse"; \
      echo "Architectures: amd64"; \
      echo "Signed-By: /usr/share/keyrings/ubuntu-archive-keyring.gpg"; \
      echo ""; \
      echo "Types: deb"; \
      echo "URIs: http://ports.ubuntu.com/ubuntu-ports/"; \
      echo "Suites: noble noble-updates noble-backports noble-security"; \
      echo "Components: main restricted universe multiverse"; \
      echo "Architectures: arm64"; \
      echo "Signed-By: /usr/share/keyrings/ubuntu-archive-keyring.gpg"; \
    } > /etc/apt/sources.list.d/ubuntu.sources

RUN dpkg --add-architecture arm64 \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        curl \
        git \
        clang \
        llvm \
        binutils-aarch64-linux-gnu \
        gcc-aarch64-linux-gnu \
        zlib1g-dev:arm64 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /actions-runner

RUN RUNNER_ARCH=$(echo "$TARGETARCH" | sed 's/amd64/x64/') \
    && curl -fsSL \
      "https://github.com/actions/runner/releases/download/v${RUNNER_VERSION}/actions-runner-linux-${RUNNER_ARCH}-${RUNNER_VERSION}.tar.gz" \
      -o runner.tar.gz \
    && tar xzf runner.tar.gz \
    && rm runner.tar.gz \
    && ./bin/installdependencies.sh

COPY entrypoint.sh /actions-runner/entrypoint.sh

RUN sed -i 's/\r$//' /actions-runner/entrypoint.sh \
    && chmod +x /actions-runner/entrypoint.sh

ENTRYPOINT ["/actions-runner/entrypoint.sh"]