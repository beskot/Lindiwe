# GitHub Actions self-hosted runner + arm64 cross-toolchain for Native AOT.

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble

ARG DEBIAN_FRONTEND=noninteractive

ARG RUNNER_VERSION="2.337.0"

RUN dpkg --add-architecture arm64

RUN if [ -f /etc/apt/sources.list.d/ubuntu.sources ]; then \
      if grep -q '^Architectures:' /etc/apt/sources.list.d/ubuntu.sources; then \
        sed -i 's/^Architectures:.*/Architectures: amd64/' /etc/apt/sources.list.d/ubuntu.sources; \
      else \
        sed -i '/^Types: deb/a Architectures: amd64' /etc/apt/sources.list.d/ubuntu.sources; \
      fi; \
    fi \
    && if [ -f /etc/apt/sources.list ] && [ -s /etc/apt/sources.list ]; then \
      sed -i -e 's/^deb http/deb [arch=amd64] http/g' \
             -e 's/^deb-src http/deb-src [arch=amd64] http/g' \
             /etc/apt/sources.list; \
    fi

RUN printf '%s\n' \
    'deb [arch=arm64] http://ports.ubuntu.com/ubuntu-ports/ noble main restricted universe multiverse' \
    'deb [arch=arm64] http://ports.ubuntu.com/ubuntu-ports/ noble-updates main restricted universe multiverse' \
    'deb [arch=arm64] http://ports.ubuntu.com/ubuntu-ports/ noble-security main restricted universe multiverse' \
    > /etc/apt/sources.list.d/arm64-ports.list

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       curl jq git sudo ca-certificates \
       clang llvm binutils-aarch64-linux-gnu gcc-aarch64-linux-gnu \
       zlib1g-dev:arm64 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /actions-runner

RUN set -eux; \
    echo "Using actions/runner version: ${RUNNER_VERSION}"; \
    curl -f -A "docker-build-lindiwe" -o runner.tar.gz -L \
      "https://github.com/actions/runner/releases/download/v${RUNNER_VERSION}/actions-runner-linux-x64-${RUNNER_VERSION}.tar.gz"; \
    tar xzf runner.tar.gz; \
    rm runner.tar.gz; \
    ./bin/installdependencies.sh

COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]