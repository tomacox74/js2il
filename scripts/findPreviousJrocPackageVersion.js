#!/usr/bin/env node
"use strict";

const DEFAULT_SOURCE = "https://api.nuget.org/v3-flatcontainer";

function parseArgs(argv) {
  const args = {
    packages: [],
    source: DEFAULT_SOURCE,
  };

  for (let i = 2; i < argv.length; i++) {
    const current = argv[i];
    switch (current) {
      case "--current":
      case "-c":
        args.currentVersion = argv[++i];
        break;
      case "--package":
      case "-p":
        args.packages.push(argv[++i]);
        break;
      case "--source":
        args.source = argv[++i] || args.source;
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
  process.stdout.write(`Usage: node scripts/findPreviousJrocPackageVersion.js --current <version> --package <id> [--package <id> ...]

Finds the highest stable package version in the current major/minor release line
with a lower patch number. The version must exist for every specified package.

Options:
  --current, -c <version>  Current Jroc version
  --package, -p <id>       Required package id (repeatable)
  --source <url>           Flat-container base URL (default: ${DEFAULT_SOURCE})
  --help, -h               Show help
`);
}

function parseStableVersion(value) {
  const match = String(value ?? "").trim().replace(/^v/i, "").match(/^(\d+)\.(\d+)\.(\d+)$/);
  if (!match) {
    return null;
  }

  return {
    major: Number.parseInt(match[1], 10),
    minor: Number.parseInt(match[2], 10),
    patch: Number.parseInt(match[3], 10),
  };
}

function parseVersionCore(value) {
  const match = String(value ?? "")
    .trim()
    .replace(/^v/i, "")
    .match(/^(\d+)\.(\d+)\.(\d+)(?:[-+].*)?$/);
  if (!match) {
    return null;
  }

  return {
    major: Number.parseInt(match[1], 10),
    minor: Number.parseInt(match[2], 10),
    patch: Number.parseInt(match[3], 10),
  };
}

function findPreviousCommonPatchVersion(currentVersion, packageVersions) {
  const current = parseVersionCore(currentVersion);
  if (!current) {
    throw new Error(`Current version must contain a major.minor.patch version: ${currentVersion}`);
  }

  if (!Array.isArray(packageVersions) || packageVersions.length === 0) {
    throw new Error("At least one package version list is required.");
  }

  const commonVersions = packageVersions
    .map((versions) => new Set(versions))
    .reduce((common, versions) =>
      new Set([...common].filter((version) => versions.has(version))));

  const candidates = [...commonVersions]
    .map((version) => ({ version, parsed: parseStableVersion(version) }))
    .filter(({ parsed }) =>
      parsed
      && parsed.major === current.major
      && parsed.minor === current.minor
      && parsed.patch < current.patch)
    .sort((left, right) => right.parsed.patch - left.parsed.patch);

  return candidates[0]?.version ?? null;
}

async function fetchPackageVersions(source, packageId) {
  const normalizedPackageId = packageId.trim().toLowerCase();
  const url = `${source.replace(/\/$/, "")}/${encodeURIComponent(normalizedPackageId)}/index.json`;
  const response = await fetch(url, {
    headers: {
      "User-Agent": "jroc-benchmark-workflow/1.0",
    },
  });

  if (!response.ok) {
    throw new Error(`${packageId} package index returned HTTP ${response.status}.`);
  }

  const payload = await response.json();
  if (!Array.isArray(payload?.versions)) {
    throw new Error(`${packageId} package index did not contain a versions array.`);
  }

  return payload.versions;
}

async function main() {
  const args = parseArgs(process.argv);
  if (args.help) {
    printHelp();
    return;
  }
  if (!args.currentVersion) {
    throw new Error("Missing required --current version.");
  }
  if (args.packages.length === 0) {
    throw new Error("Specify at least one --package.");
  }

  const packageIds = [...new Set(args.packages)];
  const packageVersions = await Promise.all(
    packageIds.map((packageId) => fetchPackageVersions(args.source, packageId)));
  const previousVersion = findPreviousCommonPatchVersion(
    args.currentVersion,
    packageVersions);

  if (previousVersion) {
    process.stdout.write(`${previousVersion}\n`);
  } else {
    process.stderr.write(
      `No previous stable patch version is shared by ${packageIds.join(", ")} ` +
      `for ${args.currentVersion}.\n`);
  }
}

if (require.main === module) {
  main().catch((error) => {
    process.stderr.write(`${error?.stack || String(error)}\n`);
    process.exit(1);
  });
}

module.exports = {
  findPreviousCommonPatchVersion,
  parseStableVersion,
  parseVersionCore,
};
