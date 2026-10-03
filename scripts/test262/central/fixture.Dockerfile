# Build once on a trusted runner before its database secret is provided.
# Node's Debian-built executable runs on the newer glibc in the .NET Ubuntu image.
FROM node:24-bookworm-slim AS node
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble
COPY --from=node /usr/local/bin/node /usr/local/bin/node
ENV HOME=/work DOTNET_NOLOGO=1 DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /work
USER 65532:65532
