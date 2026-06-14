#!/usr/bin/env node
"use strict";

const DEFAULT_TIMEOUT_SECONDS = 45 * 60;
const DEFAULT_POLL_SECONDS = 30;
const DEFAULT_SOURCE = "https://api.nuget.org/v3-flatcontainer";

function parseArgs(argv) {
  const args = {
    packages: [],
    source: DEFAULT_SOURCE,
    timeoutSeconds: DEFAULT_TIMEOUT_SECONDS,
    pollSeconds: DEFAULT_POLL_SECONDS,
    help: false,
  };

  for (let i = 2; i < argv.length; i++) {
    const current = argv[i];
    switch (current) {
      case "--version":
      case "-v":
        args.version = argv[++i];
        break;
      case "--package":
      case "-p":
        args.packages.push(argv[++i]);
        break;
      case "--source":
        args.source = argv[++i] || args.source;
        break;
      case "--timeout-seconds":
        args.timeoutSeconds = Number.parseInt(argv[++i], 10);
        break;
      case "--poll-seconds":
        args.pollSeconds = Number.parseInt(argv[++i], 10);
        break;
      case "--help":
      case "-h":
        args.help = true;
        break;
      default:
        throw new Error(`Unknown argument: ${current}`);
    }
  }

  return args;
}

function printHelp() {
  process.stdout.write(`Usage: node scripts/waitForNuGetPackageVersions.js --version <version> --package <id> [--package <id> ...]

Options:
  --version, -v <version>        Required package version to wait for
  --package, -p <id>             Required package id (repeatable)
  --source <url>                 Flat-container base URL (default: ${DEFAULT_SOURCE})
  --timeout-seconds <seconds>    Overall timeout (default: ${DEFAULT_TIMEOUT_SECONDS})
  --poll-seconds <seconds>       Poll interval (default: ${DEFAULT_POLL_SECONDS})
  --help, -h                     Show help
`);
}

function validateArgs(args) {
  if (!args.version) {
    throw new Error("Missing required --version.");
  }

  if (args.packages.length === 0) {
    throw new Error("Specify at least one --package.");
  }

  if (!Number.isInteger(args.timeoutSeconds) || args.timeoutSeconds <= 0) {
    throw new Error("--timeout-seconds must be a positive integer.");
  }

  if (!Number.isInteger(args.pollSeconds) || args.pollSeconds <= 0) {
    throw new Error("--poll-seconds must be a positive integer.");
  }
}

function delay(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function normalizePackageId(packageId) {
  return packageId.trim().toLowerCase();
}

async function isVersionAvailable(source, packageId, version) {
  const normalizedPackageId = normalizePackageId(packageId);
  const url = `${source.replace(/\/$/, "")}/${encodeURIComponent(normalizedPackageId)}/index.json`;

  let response;
  try {
    response = await fetch(url, {
      headers: {
        "User-Agent": "jroc-release-workflow/1.0",
      },
    });
  } catch (error) {
    return {
      available: false,
      message: `${packageId}: request failed (${error.message})`,
    };
  }

  if (response.status === 404) {
    return {
      available: false,
      message: `${packageId}: package index not published yet`,
    };
  }

  if (!response.ok) {
    return {
      available: false,
      message: `${packageId}: package index returned HTTP ${response.status}`,
    };
  }

  let payload;
  try {
    payload = await response.json();
  } catch (error) {
    return {
      available: false,
      message: `${packageId}: invalid JSON (${error.message})`,
    };
  }

  const versions = Array.isArray(payload?.versions) ? payload.versions : [];
  return versions.includes(version)
    ? { available: true, message: `${packageId}: ${version} available` }
    : { available: false, message: `${packageId}: ${version} not indexed yet` };
}

async function main() {
  const args = parseArgs(process.argv);
  if (args.help) {
    printHelp();
    return 0;
  }

  validateArgs(args);

  const deadline = Date.now() + (args.timeoutSeconds * 1000);
  const packageIds = [...new Set(args.packages)];
  const unavailable = new Set(packageIds);

  process.stdout.write(
    `Waiting for NuGet version ${args.version} across ${packageIds.join(", ")} ` +
      `(timeout ${args.timeoutSeconds}s, poll ${args.pollSeconds}s)\n`
  );

  let attempt = 0;
  while (Date.now() < deadline) {
    attempt++;
    process.stdout.write(`Attempt ${attempt}: checking ${unavailable.size} package(s)\n`);

    const checks = await Promise.all(
      [...unavailable].map(async (packageId) => ({
        packageId,
        result: await isVersionAvailable(args.source, packageId, args.version),
      }))
    );

    for (const check of checks) {
      process.stdout.write(`- ${check.result.message}\n`);
      if (check.result.available) {
        unavailable.delete(check.packageId);
      }
    }

    if (unavailable.size === 0) {
      process.stdout.write(`All packages are available for version ${args.version}.\n`);
      return 0;
    }

    if (Date.now() + (args.pollSeconds * 1000) >= deadline) {
      break;
    }

    await delay(args.pollSeconds * 1000);
  }

  process.stderr.write(
    `Timed out waiting for version ${args.version}. Missing packages: ${[...unavailable].join(", ")}\n`
  );
  return 1;
}

main()
  .then((exitCode) => {
    if (exitCode !== 0) {
      process.exit(exitCode);
    }
  })
  .catch((error) => {
    process.stderr.write(`${error?.stack || String(error)}\n`);
    process.exit(1);
  });
