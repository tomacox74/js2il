#!/usr/bin/env node
"use strict";

const childProcess = require("node:child_process");
const fs = require("node:fs");
const path = require("node:path");

function printUsage() {
  console.log("Usage: node scripts/runPhasedBenchmarkScenario.js <scenario-name>");
  console.log("");
  console.log("Examples:");
  console.log("  node scripts/runPhasedBenchmarkScenario.js dromaeo-object-regexp");
  console.log("  node scripts/runPhasedBenchmarkScenario.js dromaeo-object-regexp.js");
}

function normalizeScenarioName(input) {
  const trimmed = String(input ?? "").trim();
  if (!trimmed) return "";
  const base = path.basename(trimmed);
  return base.replace(/\.js$/i, "");
}

function main() {
  const argv = process.argv.slice(2);
  if (argv.length !== 1 || argv[0] === "--help" || argv[0] === "-h") {
    printUsage();
    process.exit(argv[0] === "--help" || argv[0] === "-h" ? 0 : 1);
  }

  const scenarioName = normalizeScenarioName(argv[0]);
  if (!scenarioName) {
    console.error("A valid scenario name is required.");
    process.exit(1);
  }

  const repoRoot = path.resolve(__dirname, "..");
  const benchmarksDir = path.join(repoRoot, "tests", "performance", "Benchmarks");

  if (!fs.existsSync(benchmarksDir)) {
    console.error(`Benchmark directory not found: ${benchmarksDir}`);
    process.exit(1);
  }

  const filter = `*${scenarioName}*`;
  const dotnetArgs = ["run", "-c", "Release", "--", "--phased", "--filter", filter];

  console.log(`Running phased benchmark for scenario: ${scenarioName}`);
  console.log(`Command: dotnet ${dotnetArgs.join(" ")}`);

  const result = childProcess.spawnSync("dotnet", dotnetArgs, {
    cwd: benchmarksDir,
    stdio: "inherit",
    shell: false,
  });

  process.exit(result.status ?? 1);
}

main();
