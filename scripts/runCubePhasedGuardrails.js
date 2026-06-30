#!/usr/bin/env node
"use strict";

const childProcess = require("node:child_process");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");

const TARGET_SCENARIOS = ["dromaeo-3d-cube", "dromaeo-3d-cube-modern"];
const RUNTIME_BY_METHOD = {
  Jroc_ExecuteOnly: "jroc-execute",
  Jint_ExecutePrepared: "jint-execute-prepared",
  Okojo_ExecuteOnly: "okojo-execute",
};

const SMELL_PATTERNS = [
  { key: "newarrObject", label: "newarr System.Object", regex: /newarr\s+(?:class\s+)?(?:\[[^\]]+\])?System\.Object\b/g },
  { key: "box", label: "box", regex: /\bbox\s+/g },
  { key: "closureInvoke", label: "Closure::InvokeWithArgs*", regex: /Closure::InvokeWithArgs\d*/g },
  { key: "toNumber", label: "TypeUtilities::ToNumber", regex: /TypeUtilities::ToNumber\b/g },
  { key: "getItem", label: "ObjectRuntime::GetItem(", regex: /ObjectRuntime::GetItem\(/g },
];

function normalizeScenarioName(value) {
  const trimmed = String(value || "").trim();
  if (!trimmed) return "";
  return path.basename(trimmed).replace(/\.js$/i, "");
}

function printUsage() {
  console.log("Usage: node scripts/runCubePhasedGuardrails.js [options]");
  console.log("");
  console.log("Runs only the dromaeo-3d-cube phased benchmarks and prints");
  console.log("jroc-execute, jint-execute-prepared, and okojo-execute counters.");
  console.log("");
  console.log("Options:");
  console.log("  --dry                    Use a quick Dry BenchmarkDotNet job.");
  console.log("  --no-run                 Skip running benchmarks and only parse existing results.");
  console.log("  --il-smells              Also compile/decompile both cube scenarios and count IL smells.");
  console.log("  --keep-il-artifacts      Keep temporary IL artifacts produced by --il-smells.");
  console.log("  --scenario <name>        Limit run/output to specific scenario(s). Repeatable.");
  console.log("  --results-file <path>    Override BenchmarkDotNet full-compressed JSON path.");
  console.log("  --output-json <path>     Write machine-readable summary JSON.");
  console.log("  -h, --help               Show this help.");
  console.log("");
  console.log("Examples:");
  console.log("  node scripts/runCubePhasedGuardrails.js --dry");
  console.log("  node scripts/runCubePhasedGuardrails.js --dry --scenario dromaeo-3d-cube");
  console.log("  node scripts/runCubePhasedGuardrails.js --dry --il-smells");
  console.log("  node scripts/runCubePhasedGuardrails.js --no-run --results-file tests/performance/Benchmarks/BenchmarkDotNet.Artifacts/results/Benchmarks.JrocPhasedBenchmarks-report-full-compressed.json");
}

function parseArgs(argv) {
  const args = {
    dry: false,
    noRun: false,
    ilSmells: false,
    keepIlArtifacts: false,
    scenarios: [],
    resultsFile: "",
    outputJson: "",
  };

  for (let i = 0; i < argv.length; i += 1) {
    const token = argv[i];
    switch (token) {
      case "--dry":
        args.dry = true;
        break;
      case "--no-run":
        args.noRun = true;
        break;
      case "--il-smells":
        args.ilSmells = true;
        break;
      case "--keep-il-artifacts":
        args.keepIlArtifacts = true;
        break;
      case "--scenario":
        i += 1;
        {
          const scenario = normalizeScenarioName(argv[i] || "");
          if (!scenario) {
            throw new Error("Missing value for --scenario");
          }
          args.scenarios.push(scenario);
        }
        break;
      case "--results-file":
        i += 1;
        args.resultsFile = argv[i] || "";
        if (!args.resultsFile) {
          throw new Error("Missing value for --results-file");
        }
        break;
      case "--output-json":
        i += 1;
        args.outputJson = argv[i] || "";
        if (!args.outputJson) {
          throw new Error("Missing value for --output-json");
        }
        break;
      case "-h":
      case "--help":
        printUsage();
        process.exit(0);
      default:
        throw new Error(`Unknown argument: ${token}`);
    }
  }

  return args;
}

function findRepoRoot(startDir) {
  let current = path.resolve(startDir);
  while (true) {
    if (
      fs.existsSync(path.join(current, "jroc.sln")) &&
      fs.existsSync(path.join(current, "tests", "performance", "Benchmarks"))
    ) {
      return current;
    }

    const parent = path.dirname(current);
    if (!parent || parent === current) {
      break;
    }
    current = parent;
  }

  throw new Error(`Could not locate repository root from: ${startDir}`);
}

function runChecked(command, args, options = {}) {
  const pretty = `${command} ${args.join(" ")}`;
  console.log(`> ${pretty}`);

  const result = childProcess.spawnSync(command, args, {
    cwd: options.cwd,
    stdio: options.stdio || "inherit",
    encoding: options.encoding || "utf8",
    shell: false,
    maxBuffer: options.maxBuffer || (64 * 1024 * 1024),
  });

  if (result.error) {
    throw result.error;
  }

  if ((result.status ?? 1) !== 0) {
    if (options.stdio === "pipe") {
      if (result.stdout) process.stdout.write(result.stdout);
      if (result.stderr) process.stderr.write(result.stderr);
    }
    throw new Error(`Command failed (${result.status ?? "unknown"}): ${pretty}`);
  }

  return result;
}

function parseScenarioName(entry) {
  const candidates = [entry?.Parameters, entry?.FullName, entry?.DisplayInfo];
  for (const candidate of candidates) {
    if (!candidate) continue;
    const match = String(candidate).match(/ScriptName\s*[:=]\s*"?([A-Za-z0-9._-]+)"?/i);
    if (match) {
      return match[1].replace(/\.js$/i, "");
    }
  }
  return "";
}

function toNumber(value) {
  return typeof value === "number" && Number.isFinite(value) ? value : null;
}

function getMetric(entry, id) {
  const metrics = Array.isArray(entry?.Metrics) ? entry.Metrics : [];
  for (const metric of metrics) {
    if (metric?.Descriptor?.Id === id && Number.isFinite(metric?.Value)) {
      return metric.Value;
    }
  }
  return null;
}

function formatFloat(value, decimals = 2) {
  if (!Number.isFinite(value)) return "n/a";
  return value.toFixed(decimals);
}

function pad(value, width) {
  const text = String(value);
  if (text.length >= width) return text;
  return `${text}${" ".repeat(width - text.length)}`;
}

function resolveScenarios(argsScenarios) {
  const chosen = (argsScenarios && argsScenarios.length > 0) ? argsScenarios : TARGET_SCENARIOS;
  const unique = [];
  for (const scenario of chosen) {
    if (!unique.includes(scenario)) {
      unique.push(scenario);
    }
  }
  return unique;
}

function parsePhasedRows(reportPath, targetScenarios) {
  if (!fs.existsSync(reportPath)) {
    throw new Error(`Benchmark report not found: ${reportPath}`);
  }

  const report = JSON.parse(fs.readFileSync(reportPath, "utf8"));
  const benchmarks = Array.isArray(report?.Benchmarks) ? report.Benchmarks : [];
  if (benchmarks.length === 0) {
    throw new Error(`Benchmark report has no benchmark rows: ${reportPath}`);
  }

  const byKey = new Map();
  for (const bench of benchmarks) {
    const runtime = RUNTIME_BY_METHOD[bench?.Method];
    if (!runtime) continue;

    const scenario = parseScenarioName(bench);
    if (!targetScenarios.includes(scenario)) continue;

    const meanNs = toNumber(bench?.Statistics?.Mean);
    const allocatedBytes = toNumber(bench?.Memory?.BytesAllocatedPerOperation);
    const row = {
      scenario,
      runtime,
      meanNs,
      allocatedBytes,
      gen0Per1000Ops: getMetric(bench, "Gen0Collects"),
      gen1Per1000Ops: getMetric(bench, "Gen1Collects"),
      gen2Per1000Ops: getMetric(bench, "Gen2Collects"),
      displayInfo: bench?.DisplayInfo || "",
    };

    byKey.set(`${scenario}|${runtime}`, row);
  }

  const rows = [];
  for (const scenario of targetScenarios) {
    for (const runtime of ["jroc-execute", "jint-execute-prepared", "okojo-execute"]) {
      const key = `${scenario}|${runtime}`;
      if (!byKey.has(key)) {
        throw new Error(`Missing benchmark row for ${key} in ${reportPath}`);
      }
      rows.push(byKey.get(key));
    }
  }

  return rows;
}

function printPhasedSummary(rows) {
  console.log("");
  console.log("Phased cube counters (mean + allocation)");
  console.log("=======================================");
  console.log(
    `${pad("Scenario", 24)}  ${pad("Runtime", 22)}  ${pad("Mean(ms)", 10)}  ${pad("Alloc(MiB)", 10)}  ${pad("Gen0/1k", 8)}  ${pad("Gen1/1k", 8)}  ${pad("Gen2/1k", 8)}`
  );

  for (const row of rows) {
    const meanMs = Number.isFinite(row.meanNs) ? row.meanNs / 1_000_000 : NaN;
    const allocMiB = Number.isFinite(row.allocatedBytes) ? row.allocatedBytes / (1024 * 1024) : NaN;
    console.log(
      `${pad(row.scenario, 24)}  ${pad(row.runtime, 22)}  ${pad(formatFloat(meanMs), 10)}  ${pad(formatFloat(allocMiB), 10)}  ${pad(formatFloat(row.gen0Per1000Ops, 2), 8)}  ${pad(formatFloat(row.gen1Per1000Ops, 2), 8)}  ${pad(formatFloat(row.gen2Per1000Ops, 2), 8)}`
    );
  }
}

function computeRatios(rows, scenarios) {
  const ratios = [];
  for (const scenario of scenarios) {
    const scenarioRows = rows.filter((r) => r.scenario === scenario);
    const jroc = scenarioRows.find((r) => r.runtime === "jroc-execute");
    const jint = scenarioRows.find((r) => r.runtime === "jint-execute-prepared");
    const okojo = scenarioRows.find((r) => r.runtime === "okojo-execute");

    ratios.push({
      scenario,
      jrocVsJintTime: jroc?.meanNs && jint?.meanNs ? jroc.meanNs / jint.meanNs : null,
      jrocVsOkojoTime: jroc?.meanNs && okojo?.meanNs ? jroc.meanNs / okojo.meanNs : null,
      jrocVsJintAlloc: jroc?.allocatedBytes && jint?.allocatedBytes ? jroc.allocatedBytes / jint.allocatedBytes : null,
      jrocVsOkojoAlloc: jroc?.allocatedBytes && okojo?.allocatedBytes ? jroc.allocatedBytes / okojo.allocatedBytes : null,
    });
  }
  return ratios;
}

function printRatios(ratios) {
  console.log("");
  console.log("JROC relative ratios");
  console.log("====================");
  console.log(
    `${pad("Scenario", 24)}  ${pad("Time vs Jint", 12)}  ${pad("Time vs Okojo", 13)}  ${pad("Alloc vs Jint", 13)}  ${pad("Alloc vs Okojo", 14)}`
  );
  for (const ratio of ratios) {
    console.log(
      `${pad(ratio.scenario, 24)}  ${pad(formatFloat(ratio.jrocVsJintTime), 12)}  ${pad(formatFloat(ratio.jrocVsOkojoTime), 13)}  ${pad(formatFloat(ratio.jrocVsJintAlloc), 13)}  ${pad(formatFloat(ratio.jrocVsOkojoAlloc), 14)}`
    );
  }
}

function countMatches(text, regex) {
  const matches = text.match(regex);
  return matches ? matches.length : 0;
}

function runIlSmellScan(repoRoot, keepArtifacts, scenarios) {
  runChecked("ilspycmd", ["--version"], { cwd: repoRoot, stdio: "pipe" });

  const tempRoot = fs.mkdtempSync(path.join(os.tmpdir(), "jroc-cube-il-smells-"));
  const scanResults = [];

  try {
    for (const scenario of scenarios) {
      const scenarioPath = path.join(repoRoot, "tests", "performance", "Benchmarks", "Scenarios", `${scenario}.js`);
      const outDir = path.join(tempRoot, scenario);
      fs.mkdirSync(outDir, { recursive: true });

      runChecked(
        "dotnet",
        ["run", "-c", "Release", "--project", path.join(repoRoot, "src", "Cli"), "--", scenarioPath, outDir],
        { cwd: repoRoot, stdio: "pipe", maxBuffer: 256 * 1024 * 1024 }
      );

      const dllPath = path.join(outDir, `${scenario}.dll`);
      if (!fs.existsSync(dllPath)) {
        throw new Error(`Compiled assembly not found for IL scan: ${dllPath}`);
      }

      const il = runChecked("ilspycmd", ["-il", dllPath], {
        cwd: repoRoot,
        stdio: "pipe",
        maxBuffer: 256 * 1024 * 1024,
      }).stdout;

      const counts = {};
      for (const pattern of SMELL_PATTERNS) {
        counts[pattern.key] = countMatches(il, pattern.regex);
      }

      scanResults.push({
        scenario,
        dllPath,
        ilBytes: Buffer.byteLength(il, "utf8"),
        counts,
      });
    }
  } finally {
    if (keepArtifacts) {
      console.log(`Kept IL smell scan artifacts: ${tempRoot}`);
    } else {
      fs.rmSync(tempRoot, { recursive: true, force: true });
    }
  }

  return scanResults;
}

function printIlSmellSummary(ilSmells) {
  if (!ilSmells || ilSmells.length === 0) return;

  console.log("");
  console.log("Generated IL smell counters (optional guardrail)");
  console.log("===============================================");
  const header = [
    pad("Scenario", 24),
    ...SMELL_PATTERNS.map((p) => pad(p.label, 24)),
  ].join("  ");
  console.log(header);

  for (const row of ilSmells) {
    const parts = [pad(row.scenario, 24)];
    for (const pattern of SMELL_PATTERNS) {
      parts.push(pad(String(row.counts[pattern.key]), 24));
    }
    console.log(parts.join("  "));
  }
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  const repoRoot = findRepoRoot(__dirname);
  const benchmarksDir = path.join(repoRoot, "tests", "performance", "Benchmarks");
  const targetScenarios = resolveScenarios(args.scenarios);
  const reportPath = args.resultsFile
    ? path.resolve(repoRoot, args.resultsFile)
    : path.join(
        repoRoot,
        "tests",
        "performance",
        "Benchmarks",
        "BenchmarkDotNet.Artifacts",
        "results",
        "Benchmarks.JrocPhasedBenchmarks-report-full-compressed.json"
      );

  let rows = [];
  if (!args.noRun) {
    const byKey = new Map();
    for (const scenario of targetScenarios) {
      const dotnetArgs = [
        "run",
        "-c",
        "Release",
        "--project",
        path.join(repoRoot, "tests", "performance", "Benchmarks", "Benchmarks.csproj"),
        "--",
        "--phased",
        "--filter",
        "*JrocPhasedBenchmarks*",
        "--scenario",
        scenario,
      ];

      if (args.dry) {
        dotnetArgs.push("--job", "Dry", "--launchCount", "1", "--iterationCount", "1", "--warmupCount", "1");
      }

      runChecked("dotnet", dotnetArgs, { cwd: benchmarksDir });
      const scenarioRows = parsePhasedRows(reportPath, [scenario]);
      for (const row of scenarioRows) {
        byKey.set(`${row.scenario}|${row.runtime}`, row);
      }
    }

    for (const scenario of targetScenarios) {
      for (const runtime of ["jroc-execute", "jint-execute-prepared", "okojo-execute"]) {
        const key = `${scenario}|${runtime}`;
        if (!byKey.has(key)) {
          throw new Error(`Missing benchmark row for ${key} after benchmark run`);
        }
        rows.push(byKey.get(key));
      }
    }
  } else {
    console.log("Skipping benchmark execution (--no-run).");
    rows = parsePhasedRows(reportPath, targetScenarios);
  }

  const ratios = computeRatios(rows, targetScenarios);
  printPhasedSummary(rows);
  printRatios(ratios);

  let ilSmells = [];
  if (args.ilSmells) {
    ilSmells = runIlSmellScan(repoRoot, args.keepIlArtifacts, targetScenarios);
    printIlSmellSummary(ilSmells);
  }

  const summary = {
    generatedAt: new Date().toISOString(),
    reportPath,
    dry: args.dry,
    noRun: args.noRun,
    scenarios: targetScenarios,
    rows,
    ratios,
    ilSmells,
  };

  if (args.outputJson) {
    const outputPath = path.resolve(repoRoot, args.outputJson);
    fs.mkdirSync(path.dirname(outputPath), { recursive: true });
    fs.writeFileSync(outputPath, `${JSON.stringify(summary, null, 2)}\n`, "utf8");
    console.log(`\nWrote summary JSON: ${outputPath}`);
  }

  if (process.env.GITHUB_RUN_ID) {
    console.log(
      `\nCI run context: run_id=${process.env.GITHUB_RUN_ID}, attempt=${process.env.GITHUB_RUN_ATTEMPT || "1"}, ref=${process.env.GITHUB_REF_NAME || "unknown"}`
    );
  }
}

try {
  main();
} catch (error) {
  console.error(`\nERROR: ${error.message}`);
  process.exit(1);
}
