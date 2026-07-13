#!/usr/bin/env node
"use strict";

const cp = require("node:child_process");

const workflowFile = "benchmark-branch-comparison.yml";

function parseArgs(argv) {
  const args = {
    branch: null,
    scenario: null,
    benchmarkSuite: "phased",
    repo: null,
    ref: "master",
    watch: false,
    dryRun: false,
    help: false,
  };

  for (let i = 0; i < argv.length; i += 1) {
    const current = argv[i];
    switch (current) {
      case "--branch":
      case "-b":
        args.branch = argv[++i] ?? null;
        break;
      case "--scenario":
      case "-s":
        args.scenario = argv[++i] ?? null;
        break;
      case "--benchmark":
      case "--suite":
        args.benchmarkSuite = argv[++i] ?? null;
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
        if (!current.startsWith("-") && !args.branch) {
          args.branch = current;
        } else if (!current.startsWith("-") && !args.scenario) {
          args.scenario = current;
        } else {
          throw new Error(`Unknown argument: ${current}`);
        }
    }
  }

  return args;
}

function printHelp() {
  process.stdout.write(`Usage: node scripts/dispatchBenchmarkBranchComparisonWorkflow.js <private-branch> <scenario-name> [options]

Dispatches the manual workflow that benchmarks master first, then a private branch,
using the selected BenchmarkDotNet suite. The workflow uploads both raw result sets
and console output as an artifact.

Options:
  --branch, -b <branch>       Private branch/ref to compare with master.
  --scenario, -s <scenario>  Scenario or filter name for the selected suite.
  --benchmark, --suite <name> Benchmark suite: phased (default) or kracken.
  --repo <owner/name>         Explicit repository override (defaults to origin).
  --ref <branch>              Branch/ref containing the workflow file (default: master).
  --watch                     Wait for the dispatched run to finish.
  --dry-run                   Print the gh command without dispatching.
  --help, -h                  Show this help.

Examples:
  node scripts/dispatchBenchmarkBranchComparisonWorkflow.js perf/object-shapes dromaeo-3d-cube
  node scripts/dispatchBenchmarkBranchComparisonWorkflow.js --branch perf/object-shapes --scenario dromaeo-3d-cube --watch
  node scripts/dispatchBenchmarkBranchComparisonWorkflow.js v0.11.21 ai-astar --benchmark kracken --watch
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
    if (stdout) process.stderr.write(`${stdout}\n`);
    if (stderr) process.stderr.write(`${stderr}\n`);
    throw new Error(`Failed to run: ${command} ${args.join(" ")}`);
  }
}

function inferRepoFromGitRemote() {
  try {
    const remote = run("git", ["remote", "get-url", "origin"]).trim();
    const httpsMatch = remote.match(/^https?:\/\/github\.com\/(.+?)\/(.+?)(?:\.git)?$/);
    if (httpsMatch) return `${httpsMatch[1]}/${httpsMatch[2]}`;

    const sshMatch = remote.match(/^git@github\.com:(.+?)\/(.+?)(?:\.git)?$/);
    if (sshMatch) return `${sshMatch[1]}/${sshMatch[2]}`;
  } catch {
    // Let gh infer the repository if the origin remote is unavailable.
  }

  return null;
}

function shellQuote(args) {
  return args
    .map((arg) => (/^[A-Za-z0-9_./:=@-]+$/.test(arg) ? arg : `"${arg.replaceAll('"', '\\"')}"`))
    .join(" ");
}

function getRecentRuns(repo) {
  const args = [
    "run",
    "list",
    "--workflow",
    workflowFile,
    "--limit",
    "5",
    "--json",
    "databaseId,url,status,conclusion,createdAt",
  ];
  if (repo) args.push("--repo", repo);

  const json = run("gh", args).trim();
  return json ? JSON.parse(json) : [];
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  if (args.help) {
    printHelp();
    return;
  }

  if (!args.branch) throw new Error("A private branch/ref is required.");
  if (!args.scenario) throw new Error("A benchmark scenario name is required.");
  if (!["phased", "kracken"].includes(args.benchmarkSuite)) {
    throw new Error("--benchmark must be either 'phased' or 'kracken'.");
  }
  if (!args.ref) throw new Error("--ref requires a value.");

  const repo = args.repo ?? inferRepoFromGitRemote();
  const workflowArgs = [
    "workflow",
    "run",
    workflowFile,
    "--ref",
    args.ref,
    "-f",
    `private_branch=${args.branch}`,
    "-f",
    `scenario_name=${args.scenario}`,
    "-f",
    `benchmark_suite=${args.benchmarkSuite}`,
  ];
  if (repo) workflowArgs.push("--repo", repo);

  if (args.dryRun) {
    process.stdout.write(`gh ${shellQuote(workflowArgs)}\n`);
    return;
  }

  const latestBefore = getRecentRuns(repo)[0]?.databaseId ?? 0;
  run("gh", workflowArgs, { stdio: "inherit" });

  Atomics.wait(new Int32Array(new SharedArrayBuffer(4)), 0, 0, 3000);
  const latestRun = getRecentRuns(repo).find((run) => Number(run.databaseId) > Number(latestBefore)) ?? null;
  if (latestRun?.url) {
    process.stdout.write(`Dispatched ${workflowFile}: ${latestRun.url}\n`);
  } else {
    process.stdout.write(`Dispatched ${workflowFile}.\n`);
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
