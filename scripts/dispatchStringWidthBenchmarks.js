#!/usr/bin/env node
"use strict";

const { spawnSync } = require("node:child_process");

const WORKFLOW = "mitata-suite.yml";
const RUNTIMES = "node,jroc,clearscript,jint,okojo";

function usage() {
  console.log(`Usage: node scripts/dispatchStringWidthBenchmarks.js <jroc-version> [options]

Dispatch the string-width Mitata suite using a published JROC tool version.

Options:
  --ref <branch>       Branch containing the workflow (default: master)
  --repo <owner/name>  Repository override (default: current repository)
  --dry-run            Print the gh command without dispatching
  --help, -h           Show this help

Example:
  node scripts/dispatchStringWidthBenchmarks.js 0.12.29 --dry-run
  node scripts/dispatchStringWidthBenchmarks.js 0.12.29`);
}

function parseArgs(argv) {
  const args = { version: null, ref: "master", repo: null, dryRun: false, help: false };

  for (let i = 0; i < argv.length; i += 1) {
    const arg = argv[i];
    switch (arg) {
      case "--ref":
        args.ref = argv[++i] ?? null;
        break;
      case "--repo":
        args.repo = argv[++i] ?? null;
        break;
      case "--dry-run":
        args.dryRun = true;
        break;
      case "--help":
      case "-h":
        args.help = true;
        break;
      default:
        if (!arg.startsWith("-") && args.version === null) {
          args.version = arg;
        } else {
          throw new Error(`Unknown argument: ${arg}`);
        }
    }
  }

  if (args.help) return args;
  if (!/^v?\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?(?:\+[0-9A-Za-z.-]+)?$/.test(args.version ?? "")) {
    throw new Error("A published JROC version is required (for example 0.12.29).");
  }
  if (!/^[A-Za-z0-9._/-]+$/.test(args.ref ?? "") || args.ref.includes("..")) {
    throw new Error("--ref requires a branch or tag name.");
  }
  if (args.repo !== null && !/^[A-Za-z0-9_.-]+\/[A-Za-z0-9_.-]+$/.test(args.repo)) {
    throw new Error("--repo must be owner/name.");
  }
  args.version = args.version.replace(/^v/, "");
  return args;
}

function quote(value) {
  return /^[A-Za-z0-9_./:=@,-]+$/.test(value)
    ? value
    : `"${value.replaceAll('"', '\\"')}"`;
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  if (args.help) return usage();

  const command = [
    "workflow", "run", WORKFLOW,
    "--ref", args.ref,
    "--raw-field", "benchmark=string-width",
    "--raw-field", `runtimes=${RUNTIMES}`,
    "--raw-field", `jroc_package_version=${args.version}`,
  ];
  if (args.repo) command.push("--repo", args.repo);

  if (args.dryRun) {
    console.log(`gh ${command.map(quote).join(" ")}`);
    return;
  }

  const result = spawnSync("gh", command, { stdio: "inherit", shell: false });
  if (result.error) throw result.error;
  if (result.status !== 0) {
    process.exitCode = result.status ?? 1;
    return;
  }
  console.log(`Dispatched ${WORKFLOW} for string-width with published JROC ${args.version}.`);
}

try {
  main();
} catch (error) {
  console.error(error.message);
  process.exitCode = 1;
}
