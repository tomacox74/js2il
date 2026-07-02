#!/usr/bin/env node
"use strict";

const cp = require("node:child_process");

function parseArgs(argv) {
  const args = {
    prNumber: null,
    repo: null,
    help: false,
    includeLogs: true,
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
      case "--no-logs":
        args.includeLogs = false;
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
  --no-logs              Do not fetch failed job logs for failed test names
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

function parseActionsJobUrl(detailsUrl) {
  const match = String(detailsUrl ?? "").match(/\/actions\/runs\/(\d+)\/job\/(\d+)/);
  if (!match) {
    return null;
  }

  return {
    runId: match[1],
    jobId: match[2],
  };
}

function stripActionsLogPrefix(line) {
  const fields = String(line).split("\t");
  const message = fields.length >= 3 ? fields.slice(2).join("\t") : String(line);
  return message
    .replace(/^\uFEFF?/, "")
    .replace(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d+Z\s*/, "")
    .trimEnd();
}

function cleanTestName(name) {
  return String(name ?? "")
    .trim()
    .replace(/\s+\[\d+(?:\.\d+)?\s*(?:ms|s|m)\]$/i, "")
    .trim();
}

function addFailure(failuresByName, name, detail) {
  const cleanName = cleanTestName(name);
  if (!cleanName || cleanName === "Test Run Failed.") {
    return null;
  }

  if (!failuresByName.has(cleanName)) {
    failuresByName.set(cleanName, {
      name: cleanName,
      details: [],
    });
  }

  const failure = failuresByName.get(cleanName);
  if (detail) {
    const cleanDetail = detail.trim();
    if (cleanDetail && !failure.details.includes(cleanDetail)) {
      failure.details.push(cleanDetail);
    }
  }

  return failure;
}

function parseFailedTestsFromLog(log) {
  const failuresByName = new Map();
  let currentFailure = null;
  let collectingError = false;

  for (const rawLine of String(log ?? "").split(/\r?\n/)) {
    const line = stripActionsLogPrefix(rawLine);
    const xunitMatch = line.match(/\]\s+(.+?)\s+\[FAIL\]\s*$/);
    if (xunitMatch) {
      addFailure(failuresByName, xunitMatch[1]);
      collectingError = false;
      continue;
    }

    const vstestMatch = line.match(/^\s*Failed\s+(.+?)\s+\[[^\]]+\]\s*$/);
    if (vstestMatch) {
      currentFailure = addFailure(failuresByName, vstestMatch[1]);
      collectingError = false;
      continue;
    }

    if (/^\s*Error Message:\s*$/.test(line)) {
      collectingError = true;
      continue;
    }

    if (!collectingError || !currentFailure) {
      continue;
    }

    const detail = line.trim();
    if (!detail || /^\s*Stack Trace:\s*$/.test(line) || /^\s*at\s+/.test(line)) {
      collectingError = false;
      continue;
    }

    currentFailure.details.push(detail);
    if (currentFailure.details.length >= 4) {
      collectingError = false;
    }
  }

  return [...failuresByName.values()];
}

function getFailedTests(check, repo) {
  const job = parseActionsJobUrl(check.detailsUrl);
  if (!job) {
    return [];
  }

  const ghArgs = ["run", "view", job.runId, "--job", job.jobId, "--log-failed"];
  if (repo) {
    ghArgs.push("--repo", repo);
  }

  try {
    return parseFailedTestsFromLog(runGh(ghArgs));
  } catch (error) {
    process.stderr.write(`Warning: could not fetch failed job log for ${check.name}: ${error.message}\n`);
    return [];
  }
}

function summarizeChecks(checks, options = {}) {
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

    if (!options.includeLogs) {
      continue;
    }

    const failedTests = getFailedTests(check, options.repo);
    if (failedTests.length === 0) {
      continue;
    }

    process.stdout.write(`  Failed tests (${failedTests.length}):\n`);
    for (const test of failedTests) {
      process.stdout.write(`  - ${test.name}\n`);
      for (const detail of test.details.slice(0, 4)) {
        process.stdout.write(`    ${detail}\n`);
      }
    }
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
  return summarizeChecks(checks, {
    includeLogs: args.includeLogs,
    repo,
  });
}

try {
  process.exitCode = main();
} catch (error) {
  process.stderr.write(`${error?.message ?? String(error)}\n`);
  process.exitCode = 2;
}
