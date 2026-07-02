#!/usr/bin/env node
"use strict";

const cp = require("node:child_process");

const workflowFile = "update-generator-snapshots.yml";

function parseArgs(argv) {
  const args = {
    prNumber: null,
    repo: null,
    ref: "master",
    watch: false,
    dryRun: false,
    help: false,
  };

  for (let i = 0; i < argv.length; i++) {
    const current = argv[i];
    if (current.startsWith("--pr=")) {
      args.prNumber = current.slice("--pr=".length);
      continue;
    }
    if (current.startsWith("-p=")) {
      args.prNumber = current.slice("-p=".length);
      continue;
    }
    if (current.startsWith("--repo=")) {
      args.repo = current.slice("--repo=".length);
      continue;
    }
    if (current.startsWith("--ref=")) {
      args.ref = current.slice("--ref=".length);
      continue;
    }

    switch (current) {
      case "--pr":
      case "-p":
        args.prNumber = argv[++i] ?? null;
        break;
      case "--repo":
        args.repo = argv[++i] ?? null;
        break;
      case "--ref":
        args.ref = argv[++i] ?? null;
        break;
      case "--watch":
        args.watch = true;
        break;
      case "--dry-run":
        args.dryRun = true;
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
  process.stdout.write(`Usage: node scripts/dispatchUpdateGeneratorSnapshotsWorkflow.js <pr-number> [options]

Dispatches the manual GitHub Actions workflow that updates generator snapshots for a PR.

Options:
  --pr, -p <number>      Pull request number (positional form also supported)
  --repo <owner/name>    Explicit repository override (defaults to origin remote)
  --ref <branch>         Branch/ref containing the workflow file (default: master)
  --watch                Wait for the dispatched run to complete
  --dry-run              Print the gh command without dispatching
  --help, -h             Show help
`);
}

function run(command, args, options = {}) {
  try {
    return cp.execFileSync(command, args, {
      encoding: "utf8",
      stdio: options.stdio ?? ["ignore", "pipe", "pipe"],
    });
  } catch (error) {
    const stdout = error?.stdout ? String(error.stdout).trim() : "";
    const stderr = error?.stderr ? String(error.stderr).trim() : "";
    if (stdout) process.stderr.write(stdout + "\n");
    if (stderr) process.stderr.write(stderr + "\n");
    throw new Error(`Failed to run: ${command} ${args.join(" ")}`);
  }
}

function inferRepoFromGitRemote() {
  try {
    const remote = run("git", ["remote", "get-url", "origin"]).trim();

    let match = remote.match(/^https?:\/\/github\.com\/(.+?)\/(.+?)(?:\.git)?$/);
    if (match) return `${match[1]}/${match[2]}`;

    match = remote.match(/^git@github\.com:(.+?)\/(.+?)(?:\.git)?$/);
    if (match) return `${match[1]}/${match[2]}`;
  } catch {
    // Let gh infer the repository if git remote lookup is unavailable.
  }

  return null;
}

function shellQuote(args) {
  return args
    .map((arg) => {
      if (/^[A-Za-z0-9_./:=@-]+$/.test(arg)) return arg;
      return `"${arg.replaceAll('"', '\\"')}"`;
    })
    .join(" ");
}

function getRecentRuns(repo, limit) {
  const runListArgs = [
    "run",
    "list",
    "--workflow",
    workflowFile,
    "--limit",
    String(limit),
    "--json",
    "databaseId,url,status,conclusion,headBranch,event,createdAt",
  ];
  if (repo) runListArgs.push("--repo", repo);

  const json = run("gh", runListArgs).trim();
  return json ? JSON.parse(json) : [];
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  if (args.help) {
    printHelp();
    return;
  }

  if (!args.prNumber || !/^\d+$/.test(String(args.prNumber))) {
    throw new Error("A numeric pull request number is required.");
  }

  if (!args.ref) {
    throw new Error("--ref requires a value.");
  }

  const repo = args.repo ?? inferRepoFromGitRemote();
  const workflowArgs = ["workflow", "run", workflowFile, "--ref", args.ref, "-f", `pr_number=${args.prNumber}`];
  if (repo) workflowArgs.push("--repo", repo);

  if (args.dryRun) {
    process.stdout.write(`gh ${shellQuote(workflowArgs)}\n`);
    return;
  }

  const latestBefore = getRecentRuns(repo, 1)[0]?.databaseId ?? 0;
  run("gh", workflowArgs, { stdio: "inherit" });

  // Give GitHub a moment to register the run before asking for the latest workflow_dispatch run.
  Atomics.wait(new Int32Array(new SharedArrayBuffer(4)), 0, 0, 3000);
  const recentRuns = getRecentRuns(repo, 5);
  const latestRun = recentRuns.find((candidate) => Number(candidate.databaseId) > Number(latestBefore))
    ?? recentRuns[0]
    ?? null;
  if (latestRun?.url) {
    process.stdout.write(`Dispatched ${workflowFile} for PR #${args.prNumber}: ${latestRun.url}\n`);
  } else {
    process.stdout.write(`Dispatched ${workflowFile} for PR #${args.prNumber}.\n`);
  }

  if (args.watch && latestRun?.databaseId) {
    const watchArgs = ["run", "watch", String(latestRun.databaseId), "--exit-status"];
    if (repo) watchArgs.push("--repo", repo);
    run("gh", watchArgs, { stdio: "inherit" });
  }
}

try {
  main();
} catch (error) {
  process.stderr.write(`${error.message}\n`);
  process.exit(1);
}
