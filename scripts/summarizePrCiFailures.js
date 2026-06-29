#!/usr/bin/env node
"use strict";

const cp = require("node:child_process");

function parseArgs(argv) {
  const args = {
    prNumber: null,
    repo: null,
    help: false,
  };

  for (let i = 2; i < argv.length; i++) {
    const current = argv[i];
    switch (current) {
      case "--pr":
      case "-p":
        args.prNumber = argv[++i] ?? null;
        break;
      case "--repo":
        args.repo = argv[++i] ?? null;
        break;
      case "--help":
      case "-h":
        args.help = true;
        break;
      default:
        if (!args.prNumber && !current.startsWith("-")) {
          args.prNumber = current;
          break;
        }
        throw new Error(`Unknown argument: ${current}`);
    }
  }

  return args;
}

function printHelp() {
  process.stdout.write(`Usage: node scripts/summarizePrCiFailures.js <pr-number> [--repo owner/name]

Summarizes failing GitHub Actions / status checks for a pull request.

Options:
  --pr, -p <number>      Pull request number (positional form also supported)
  --repo <owner/name>    Explicit repository override
  --help, -h             Show help
`);
}

function runGh(args) {
  try {
    return cp.execFileSync("gh", args, {
      encoding: "utf8",
      stdio: ["ignore", "pipe", "pipe"],
    }).trim();
  } catch (error) {
    const stdout = error?.stdout ? String(error.stdout).trim() : "";
    const stderr = error?.stderr ? String(error.stderr).trim() : "";
    if (stdout) {
      process.stderr.write(stdout + "\n");
    }
    if (stderr) {
      process.stderr.write(stderr + "\n");
    }
    throw new Error(`Failed to run: gh ${args.join(" ")}`);
  }
}

function inferRepoFromGitRemote() {
  try {
    const remote = cp.execFileSync("git", ["remote", "get-url", "origin"], {
      encoding: "utf8",
      stdio: ["ignore", "pipe", "pipe"],
    }).trim();

    let match = remote.match(/^https?:\/\/github\.com\/(.+?)\/(.+?)(?:\.git)?$/);
    if (match) {
      return `${match[1]}/${match[2]}`;
    }

    match = remote.match(/^git@github\.com:(.+?)\/(.+?)(?:\.git)?$/);
    if (match) {
      return `${match[1]}/${match[2]}`;
    }
  } catch {
    // Ignore and fall back to gh's current-repo inference.
  }

  return null;
}

function normalizeConclusion(check) {
  return String(check?.conclusion ?? "").toUpperCase();
}

function isFailure(check) {
  return [
    "FAILURE",
    "CANCELLED",
    "TIMED_OUT",
    "ACTION_REQUIRED",
    "STALE",
    "STARTUP_FAILURE",
  ].includes(normalizeConclusion(check));
}

function summarizeChecks(checks) {
  const failures = checks.filter(isFailure);

  if (failures.length === 0) {
    process.stdout.write("No failing CI checks found.\n");
    return 0;
  }

  process.stdout.write(`Failing CI checks (${failures.length}):\n`);
  for (const check of failures) {
    const workflow = check.workflowName ? ` [${check.workflowName}]` : "";
    const details = check.detailsUrl ? `\n  ${check.detailsUrl}` : "";
    process.stdout.write(`- ${check.name}${workflow} — ${normalizeConclusion(check)}${details}\n`);
  }

  return 1;
}

function main() {
  const args = parseArgs(process.argv);
  if (args.help) {
    printHelp();
    return 0;
  }

  if (!args.prNumber) {
    throw new Error("Missing pull request number.");
  }

  const repo = args.repo ?? inferRepoFromGitRemote();
  const ghArgs = ["pr", "view", String(args.prNumber), "--json", "statusCheckRollup"];
  if (repo) {
    ghArgs.push("--repo", repo);
  }

  const raw = runGh(ghArgs);
  const payload = JSON.parse(raw);
  const checks = Array.isArray(payload?.statusCheckRollup) ? payload.statusCheckRollup : [];

  process.stdout.write(`PR #${args.prNumber}\n`);
  return summarizeChecks(checks);
}

try {
  process.exitCode = main();
} catch (error) {
  process.stderr.write(`${error?.message ?? String(error)}\n`);
  process.exitCode = 2;
}
