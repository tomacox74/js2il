#!/usr/bin/env bash
set -euo pipefail

sdk_version="$(dotnet --list-sdks | awk '
  BEGIN { maximum = -1 }
  $1 ~ /^11\.0\.[0-9]+$/ {
    split($1, parts, /[.]/)
    if (parts[3] + 0 > maximum) {
      maximum = parts[3] + 0
      version = $1
    }
  }
  END { print version }
')"
if [[ -z "$sdk_version" ]]; then
  echo 'Release builds require a stable .NET 11 SDK. Install SDK 11 before selecting the release toolchain.' >&2
  exit 1
fi

printf '{"sdk":{"version":"%s","rollForward":"disable","allowPrerelease":false}}\n' "$sdk_version" > global.json
resolved_version="$(dotnet --version)"
if [[ "$resolved_version" != "$sdk_version" ]]; then
  echo "Expected release SDK $sdk_version, resolved $resolved_version." >&2
  exit 1
fi
echo "Release SDK: $resolved_version"
