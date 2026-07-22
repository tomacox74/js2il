#!/usr/bin/env node
"use strict";

const childProcess = require("node:child_process");

const WORKFLOW_FILE = "mitata-suite.yml";
const ALL_RUNTIMES = "node,jroc,clearscript,jint,yantrajs,okojo";

function printUsage() {
  console.log("Usage: node scripts/dispatchMitataBenchmarkWorkflow.js <benchmark-name> [options]");
  console.log("");
  console.log("Options:");
  console.log("  --ref <branch>        Branch/ref containing the workflow (default: master).");
  console.log("  --repo <owner/name>   Repository override (defaults to the current repository).");
  console.log("  --runtimes <names>    Comma-separated runtime names (default: all supported runtimes).");
  console.log("  --dry-run             Print the gh command without dispatching.");
  console.log("  --help, -h            Show this help.");
  console.log("");
  console.log("Example:");
  console.log("  node scripts/dispatchMitataBenchmarkWorkflow.js runtime-baseline");
}

function parseArgs(argv) {
  const args = {
    benchmark: null,
    ref: "master",
    repo: null,
    runtimes: ALL_RUNTIMES,
    dryRun: false,
  };

  for (let index = 0; index < argv.length; index += 1) {
    const current = argv[index];
    switch (current) {
      case "--ref":
        args.ref = argv[++index] ?? null;
        break;
      case "--repo":
        args.repo = argv[++index] ?? null;
        break;
      case "--runtimes":
        args.runtimes = argv[++index] ?? null;
        break;
      case "--dry-run":
        args.dryRun = true;
        break;
      case "--help":
      case "-h":
        printUsage();
        process.exit(0);
        break;
      default:
        if (!current.startsWith("-") && !args.benchmark) {
          args.benchmark = current;
        } else {
          throw new Error(`Unknown argument: ${current}`);
        }
        break;
    }
  }

  if (!args.benchmark?.trim()) {
    throw new Error("A Mitata benchmark name is required.");
  }
  if (!args.ref?.trim()) {
    throw new Error("--ref requires a value.");
  }
  if (args.repo !== null && !args.repo.trim()) {
    throw new Error("--repo requires a value.");
  }
  if (!args.runtimes?.trim()) {
    throw new Error("--runtimes requires at least one runtime.");
  }

  return args;
}

function quoteForDisplay(value) {
  return /^[A-Za-z0-9_./:=@-]+$/.test(value)
    ? value
    : `"${value.replaceAll('"', '\\"')}"`;
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  if (!args.dryRun && ["true", "1", "yes"].includes(process.env.npm_config_dry_run)) {
    args.dryRun = true;
  }

  const workflowArgs = [
    "workflow",
    "run",
    WORKFLOW_FILE,
    "--ref",
    args.ref,
    "--raw-field",
    `benchmark=${args.benchmark}`,
    "--raw-field",
    `runtimes=${args.runtimes}`,
  ];

  if (args.repo) {
    workflowArgs.push("--repo", args.repo);
  }

  if (args.dryRun) {
    console.log(`gh ${workflowArgs.map(quoteForDisplay).join(" ")}`);
    return;
  }

  const result = childProcess.spawnSync("gh", workflowArgs, {
    stdio: "inherit",
    shell: false,
  });

  if (result.error) {
    throw result.error;
  }

  process.exit(result.status ?? 1);
}

try {
  main();
} catch (error) {
  console.error(error.message);
  process.exit(1);
}
