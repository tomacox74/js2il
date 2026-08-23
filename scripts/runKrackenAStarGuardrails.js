#!/usr/bin/env node
"use strict";

const childProcess = require("node:child_process");
const fs = require("node:fs");
const path = require("node:path");

const TARGET_SCENARIO = "kracken-ai-astar";
const SOURCE_SCENARIO = "ai-astar";
const RUNTIME_BY_METHOD = {
  RunJrocTest: "jroc-execute",
  RunOkojoTest: "okojo-execute",
  RunJintTest: "jint-execute",
  RunYantraJsTest: "yantrajs-execute",
};
const REQUIRED_RUNTIMES = Object.values(RUNTIME_BY_METHOD);
const WORKLOAD_REGISTRATION_SCRIPT = [
  "var __jrocKrackenWorkload = null;",
  "var __jrocKrackenIterations = 0;",
  "var runTest = function(workload, iterations) {",
  "    __jrocKrackenWorkload = workload;",
  "    __jrocKrackenIterations = iterations;",
  "};",
].join("\n");
const BENCHMARK_RUNNER_SCRIPT = [
  "export function runBenchmark() {",
  "    for (var i = 0; i < __jrocKrackenIterations; i++) {",
  "        __jrocKrackenWorkload();",
  "    }",
  "    return 'done';",
  "}",
].join("\n");

function printUsage() {
  console.log("Usage: node scripts/runKrackenAStarGuardrails.js [options]");
  console.log("");
  console.log("Runs only kracken-ai-astar plus its focused cache/Array controls.");
  console.log("");
  console.log("Options:");
  console.log("  --dry                         Use quick Dry BenchmarkDotNet jobs.");
  console.log("  --no-run                      Parse an existing Kraken report without running it.");
  console.log("  --no-microbenchmarks          Skip the focused cache/Array microbenchmarks.");
  console.log("  --il                          Compile the exact fixture and report hot-method IL counters.");
  console.log("  --keep-il-artifacts           Keep the temporary composed source, assembly, and IL.");
  console.log("  --results-file PATH           Override the Kraken full-compressed JSON report.");
  console.log("  --micro-results-file PATH     Override the inline-cache benchmark JSON report.");
  console.log("  --baseline-report PATH        Compare this raw Kraken report with the candidate.");
  console.log("  --baseline-sha SHA            Source SHA associated with --baseline-report.");
  console.log("  --candidate-sha SHA           Source SHA associated with the candidate report.");
  console.log("  --tolerance-percent NUMBER    Allowed same-host mean regression (default: 5).");
  console.log("  --allow-regression            Report a regression without returning a failure code.");
  console.log("  --output-json PATH            Write the complete machine-readable summary.");
  console.log("  -h, --help                    Show this help.");
}

function parseArgs(argv) {
  const args = {
    dry: false,
    noRun: false,
    runMicrobenchmarks: true,
    inspectIl: false,
    keepIlArtifacts: false,
    resultsFile: "",
    microResultsFile: "",
    baselineReport: "",
    baselineSha: "",
    candidateSha: "",
    tolerancePercent: 5,
    allowRegression: false,
    outputJson: "",
  };

  const valueOptions = new Map([
    ["--results-file", "resultsFile"],
    ["--micro-results-file", "microResultsFile"],
    ["--baseline-report", "baselineReport"],
    ["--baseline-sha", "baselineSha"],
    ["--candidate-sha", "candidateSha"],
    ["--output-json", "outputJson"],
  ]);

  for (let index = 0; index < argv.length; index += 1) {
    const token = argv[index];
    if (valueOptions.has(token)) {
      index += 1;
      const value = argv[index] || "";
      if (!value) {
        throw new Error(`Missing value for ${token}`);
      }
      args[valueOptions.get(token)] = value;
      continue;
    }

    switch (token) {
      case "--dry":
        args.dry = true;
        break;
      case "--no-run":
        args.noRun = true;
        break;
      case "--no-microbenchmarks":
        args.runMicrobenchmarks = false;
        break;
      case "--il":
        args.inspectIl = true;
        break;
      case "--keep-il-artifacts":
        args.keepIlArtifacts = true;
        break;
      case "--tolerance-percent":
        index += 1;
        args.tolerancePercent = Number(argv[index]);
        if (!Number.isFinite(args.tolerancePercent) || args.tolerancePercent < 0) {
          throw new Error("--tolerance-percent must be a non-negative number");
        }
        break;
      case "--allow-regression":
        args.allowRegression = true;
        break;
      case "-h":
      case "--help":
        printUsage();
        process.exit(0);
      default:
        throw new Error(`Unknown argument: ${token}`);
    }
  }

  if (args.baselineSha && !args.baselineReport) {
    throw new Error("--baseline-sha requires --baseline-report");
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
      throw new Error(`Could not locate repository root from: ${startDir}`);
    }
    current = parent;
  }
}

function runChecked(command, args, options = {}) {
  console.log(`> ${command} ${args.join(" ")}`);
  const result = childProcess.spawnSync(command, args, {
    cwd: options.cwd,
    stdio: options.stdio || "inherit",
    encoding: "utf8",
    shell: false,
    maxBuffer: options.maxBuffer || 256 * 1024 * 1024,
    env: {
      ...process.env,
      MSBUILDDISABLENODEREUSE: "1",
      DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER: "1",
      ...options.env,
    },
  });

  if (result.error) {
    throw result.error;
  }
  if ((result.status ?? 1) !== 0) {
    if (options.stdio === "pipe") {
      if (result.stdout) process.stdout.write(result.stdout);
      if (result.stderr) process.stderr.write(result.stderr);
    }
    throw new Error(`Command failed (${result.status ?? "unknown"}): ${command}`);
  }
  return result;
}

function getGitProvenance(repoRoot) {
  const sha = runChecked("git", ["rev-parse", "HEAD"], {
    cwd: repoRoot,
    stdio: "pipe",
  }).stdout.trim();
  const status = runChecked("git", ["status", "--porcelain"], {
    cwd: repoRoot,
    stdio: "pipe",
  }).stdout;
  return {
    sha,
    sourceTreeDirty: status.length > 0,
  };
}

function parseScenarioName(entry) {
  for (const candidate of [entry?.Parameters, entry?.FullName, entry?.DisplayInfo]) {
    const match = String(candidate || "").match(
      /ScriptName\s*[:=]\s*"?([A-Za-z0-9._-]+)"?/i
    );
    if (match) {
      return match[1].replace(/\.js$/i, "");
    }
  }
  return "";
}

function parseKrackenReport(
  reportPath,
  sha = "unknown",
  sourceTreeDirty = null
) {
  if (!fs.existsSync(reportPath)) {
    throw new Error(`Kraken benchmark report not found: ${reportPath}`);
  }

  const report = JSON.parse(fs.readFileSync(reportPath, "utf8"));
  const rows = [];
  for (const benchmark of report?.Benchmarks || []) {
    const runtime = RUNTIME_BY_METHOD[benchmark?.Method];
    if (!runtime || parseScenarioName(benchmark) !== TARGET_SCENARIO) {
      continue;
    }
    rows.push({
      scenario: TARGET_SCENARIO,
      runtime,
      meanNs: finiteOrNull(benchmark?.Statistics?.Mean),
      medianNs: finiteOrNull(benchmark?.Statistics?.Median),
      sampleCount: finiteOrNull(benchmark?.Statistics?.N),
      allocatedBytes: finiteOrNull(
        benchmark?.Memory?.BytesAllocatedPerOperation
      ),
      displayInfo: benchmark?.DisplayInfo || "",
    });
  }

  for (const runtime of REQUIRED_RUNTIMES) {
    if (!rows.some((row) => row.runtime === runtime)) {
      throw new Error(
        `Missing benchmark row for ${TARGET_SCENARIO}|${runtime} in ${reportPath}`
      );
    }
  }

  const host = report?.HostEnvironmentInfo || {};
  return {
    reportPath: path.resolve(reportPath),
    sha,
    sourceTreeDirty,
    benchmarkDotNetVersion: host.BenchmarkDotNetVersion || "unknown",
    host: {
      osVersion: host.OsVersion || "unknown",
      processorName: host.ProcessorName || "unknown",
      physicalProcessorCount: finiteOrNull(host.PhysicalProcessorCount),
      physicalCoreCount: finiteOrNull(host.PhysicalCoreCount),
      logicalCoreCount: finiteOrNull(host.LogicalCoreCount),
      runtimeVersion: host.RuntimeVersion || "unknown",
      architecture: host.Architecture || "unknown",
      configuration: host.Configuration || "unknown",
      dotNetCliVersion: host.DotNetCliVersion || "unknown",
    },
    rows,
  };
}

function finiteOrNull(value) {
  return typeof value === "number" && Number.isFinite(value) ? value : null;
}

function hostCompatibilityProblems(baseline, candidate) {
  const fields = [
    "osVersion",
    "processorName",
    "physicalProcessorCount",
    "physicalCoreCount",
    "logicalCoreCount",
    "runtimeVersion",
    "architecture",
  ];
  const problems = [];
  if (baseline.benchmarkDotNetVersion !== candidate.benchmarkDotNetVersion) {
    problems.push(
      `BenchmarkDotNet ${baseline.benchmarkDotNetVersion} != ${candidate.benchmarkDotNetVersion}`
    );
  }
  for (const field of fields) {
    if (baseline.host[field] !== candidate.host[field]) {
      problems.push(`${field} ${baseline.host[field]} != ${candidate.host[field]}`);
    }
  }
  return problems;
}

function compareJrocReports(baseline, candidate, tolerancePercent) {
  const compatibilityProblems = hostCompatibilityProblems(baseline, candidate);
  if (compatibilityProblems.length > 0) {
    throw new Error(
      `Refusing cross-host comparison: ${compatibilityProblems.join("; ")}`
    );
  }

  const baselineRow = baseline.rows.find((row) => row.runtime === "jroc-execute");
  const candidateRow = candidate.rows.find((row) => row.runtime === "jroc-execute");
  if (!Number.isFinite(baselineRow?.meanNs) || !Number.isFinite(candidateRow?.meanNs)) {
    throw new Error("Both reports must contain finite JROC mean values");
  }

  const meanChangePercent =
    ((candidateRow.meanNs - baselineRow.meanNs) / baselineRow.meanNs) * 100;
  const allocationChangePercent =
    Number.isFinite(baselineRow.allocatedBytes) &&
    Number.isFinite(candidateRow.allocatedBytes) &&
    baselineRow.allocatedBytes !== 0
      ? ((candidateRow.allocatedBytes - baselineRow.allocatedBytes) /
          baselineRow.allocatedBytes) *
        100
      : null;

  return {
    baselineSha: baseline.sha,
    candidateSha: candidate.sha,
    tolerancePercent,
    meanChangePercent,
    allocationChangePercent,
    regression: meanChangePercent > tolerancePercent,
    lowSampleWarning:
      baselineRow.sampleCount < 3 || candidateRow.sampleCount < 3,
  };
}

function printKrackenReport(label, result) {
  const sourceState =
    result.sourceTreeDirty === true ? ", dirty source tree" : "";
  console.log("");
  console.log(`${label} (${result.sha}${sourceState})`);
  console.log(
    "=".repeat(
      Math.max(label.length + result.sha.length + sourceState.length + 3, 24)
    )
  );
  console.log(
    `Host: ${result.host.processorName}; ${result.host.osVersion}; ` +
      `${result.host.runtimeVersion}; ${result.host.logicalCoreCount} logical cores`
  );
  console.log(
    `BenchmarkDotNet ${result.benchmarkDotNetVersion}; report ${result.reportPath}`
  );
  console.log("Runtime             Mean(s)  Median(s)   N  Allocated(bytes)");
  for (const row of result.rows) {
    console.log(
      `${row.runtime.padEnd(19)} ${format(row.meanNs / 1e9).padStart(8)}  ` +
        `${format(row.medianNs / 1e9).padStart(9)}  ` +
        `${String(row.sampleCount ?? "n/a").padStart(2)}  ` +
        `${String(row.allocatedBytes ?? "n/a").padStart(16)}`
    );
  }
}

function printComparison(comparison) {
  console.log("");
  console.log("Same-host JROC comparison");
  console.log("=========================");
  console.log(`Baseline SHA:  ${comparison.baselineSha}`);
  console.log(`Candidate SHA: ${comparison.candidateSha}`);
  console.log(`Mean change:   ${formatSigned(comparison.meanChangePercent)}%`);
  console.log(
    `Allocation:    ${
      Number.isFinite(comparison.allocationChangePercent)
        ? `${formatSigned(comparison.allocationChangePercent)}%`
        : "n/a"
    }`
  );
  console.log(
    `${comparison.regression ? "REGRESSION" : "PASS"}: allowed mean regression ` +
      `${comparison.tolerancePercent.toFixed(2)}%`
  );
  if (comparison.lowSampleWarning) {
    console.log(
      "WARNING: at least one JROC row has fewer than 3 samples; " +
        "use a non-Dry run before drawing performance conclusions."
    );
  }
}

function format(value, decimals = 3) {
  return Number.isFinite(value) ? value.toFixed(decimals) : "n/a";
}

function formatSigned(value) {
  if (!Number.isFinite(value)) return "n/a";
  return `${value >= 0 ? "+" : ""}${value.toFixed(2)}`;
}

function runKrackenBenchmark(repoRoot, args, reportPath) {
  if (args.noRun) {
    console.log("Skipping Kraken benchmark execution (--no-run).");
    if (!args.resultsFile) {
      console.log(
        "No explicit --results-file was supplied; benchmark summary omitted " +
          "rather than attributing a stale report to HEAD."
      );
      return null;
    }
    if (!fs.existsSync(reportPath)) {
      throw new Error(`Kraken benchmark report not found: ${reportPath}`);
    }
    return parseKrackenReport(
      reportPath,
      args.candidateSha || "unknown"
    );
  }

  const benchmarkProject = path.join(
    repoRoot,
    "tests",
    "performance",
    "Benchmarks",
    "Benchmarks.csproj"
  );
  const dotnetArgs = [
    "run",
    "-c",
    "Release",
    "--project",
    benchmarkProject,
    "--",
    "--kracken",
    "--filter",
    "*KrackenExecutionBenchmarks*",
    "--scenario",
    SOURCE_SCENARIO,
  ];
  appendDryJobArgs(dotnetArgs, args.dry);
  runChecked("dotnet", dotnetArgs, { cwd: path.dirname(benchmarkProject) });
  const provenance = getGitProvenance(repoRoot);
  return parseKrackenReport(
    reportPath,
    args.candidateSha || provenance.sha,
    provenance.sourceTreeDirty
  );
}

function appendDryJobArgs(dotnetArgs, dry) {
  if (!dry) return;
  dotnetArgs.push(
    "--job",
    "Dry",
    "--launchCount",
    "1",
    "--iterationCount",
    "1",
    "--warmupCount",
    "1"
  );
}

function runMicrobenchmarks(repoRoot, args, reportPath) {
  if (!args.runMicrobenchmarks) {
    console.log("Skipping focused microbenchmarks (--no-microbenchmarks).");
    return null;
  }
  if (args.noRun) {
    if (!fs.existsSync(reportPath)) {
      if (args.microResultsFile) {
        throw new Error(`Microbenchmark report not found: ${reportPath}`);
      }
      console.log("No existing microbenchmark report found; controls omitted.");
      return null;
    }
    return parseMicrobenchmarkReport(reportPath);
  }

  const benchmarkProject = path.join(
    repoRoot,
    "tests",
    "performance",
    "Benchmarks",
    "Benchmarks.csproj"
  );
  const dotnetArgs = [
    "run",
    "-c",
    "Release",
    "--project",
    benchmarkProject,
    "--",
    "--dynamic-inline-caches",
    "--filter",
    "*GenericPropertyRead*",
    "*CachedPropertyHit*",
    "*CachedPolymorphicHit*",
    "*MegamorphicFallback*",
    "*ArrayLengthBoxedThenConsumed*",
    "*ArrayLengthDirectNumber*",
    "*ArrayLengthGuardedCandidate*",
  ];
  appendDryJobArgs(dotnetArgs, args.dry);
  runChecked("dotnet", dotnetArgs, { cwd: path.dirname(benchmarkProject) });
  return parseMicrobenchmarkReport(reportPath);
}

function parseMicrobenchmarkReport(reportPath) {
  if (!fs.existsSync(reportPath)) {
    throw new Error(`Microbenchmark report not found: ${reportPath}`);
  }
  const requiredMethods = [
    "GenericPropertyRead",
    "CachedPropertyHit",
    "CachedPropertyHit_SameShapeAcrossInstances",
    "CachedPolymorphicHit",
    "MegamorphicFallback",
    "ArrayLengthBoxedThenConsumed",
    "ArrayLengthDirectNumber",
    "ArrayLengthGuardedCandidate",
  ];
  const report = JSON.parse(fs.readFileSync(reportPath, "utf8"));
  const rows = (report?.Benchmarks || [])
    .filter((benchmark) => requiredMethods.includes(benchmark?.Method))
    .map((benchmark) => ({
      method: benchmark.Method,
      meanNs: finiteOrNull(benchmark?.Statistics?.Mean),
      medianNs: finiteOrNull(benchmark?.Statistics?.Median),
      sampleCount: finiteOrNull(benchmark?.Statistics?.N),
      allocatedBytes: finiteOrNull(
        benchmark?.Memory?.BytesAllocatedPerOperation
      ),
    }));

  for (const method of requiredMethods) {
    if (!rows.some((row) => row.method === method)) {
      throw new Error(`Missing microbenchmark row for ${method} in ${reportPath}`);
    }
  }

  return {
    reportPath: path.resolve(reportPath),
    host: report?.HostEnvironmentInfo || {},
    rows,
  };
}

function printMicrobenchmarks(result) {
  if (!result) return;
  console.log("");
  console.log("Focused cache and Array controls");
  console.log("================================");
  console.log("Method                                        Mean(ns)   N  Allocated(bytes)");
  for (const row of result.rows) {
    console.log(
      `${row.method.padEnd(45)} ${format(row.meanNs).padStart(9)}  ` +
        `${String(row.sampleCount ?? "n/a").padStart(2)}  ` +
        `${String(row.allocatedBytes ?? "n/a").padStart(16)}`
    );
  }

  const generic = result.rows.find((row) => row.method === "GenericPropertyRead");
  const megamorphic = result.rows.find((row) => row.method === "MegamorphicFallback");
  const boxed = result.rows.find(
    (row) => row.method === "ArrayLengthBoxedThenConsumed"
  );
  const direct = result.rows.find(
    (row) => row.method === "ArrayLengthDirectNumber"
  );
  const guarded = result.rows.find(
    (row) => row.method === "ArrayLengthGuardedCandidate"
  );
  const sameShape = result.rows.find(
    (row) => row.method === "CachedPropertyHit_SameShapeAcrossInstances"
  );
  console.log(
    `Megamorphic vs generic: ${format(megamorphic.meanNs / generic.meanNs, 2)}x`
  );
  console.log(
    `Same-shape cross-instance vs generic: ` +
      `${format(sameShape.meanNs / generic.meanNs, 2)}x`
  );
  console.log(
    `Array length allocation, boxed/guarded/direct: ${boxed.allocatedBytes}/` +
      `${guarded.allocatedBytes}/${direct.allocatedBytes} B/op`
  );
}

function composeFixture(repoRoot) {
  const scenarioDirectory = path.join(
    repoRoot,
    "tests",
    "performance",
    "Benchmarks",
    "Scenarios",
    "kracken-1.1"
  );
  const data = fs.readFileSync(
    path.join(scenarioDirectory, `${SOURCE_SCENARIO}-data.js`),
    "utf8"
  );
  const test = fs.readFileSync(
    path.join(scenarioDirectory, `${SOURCE_SCENARIO}.js`),
    "utf8"
  );
  return [
    data,
    WORKLOAD_REGISTRATION_SCRIPT,
    test,
    BENCHMARK_RUNNER_SCRIPT,
  ].join("\n");
}

function findFindGraphNodeOwner(source) {
  const lines = source.split(/\r?\n/);
  const marker = "Array.prototype.findGraphNode = function";
  const lineIndex = lines.findIndex((line) => line.includes(marker));
  if (lineIndex < 0) {
    throw new Error("Could not locate Array.prototype.findGraphNode");
  }
  const functionColumn = lines[lineIndex].indexOf("function");
  if (functionColumn < 0) {
    throw new Error("Could not locate findGraphNode function expression");
  }
  return `FunctionExpression_L${lineIndex + 1}C${functionColumn}`;
}

function extractSourceMethodBodies(il) {
  const starts = [...il.matchAll(/^[ \t]*\.method\b/gm)].map(
    (match) => match.index
  );
  const bodies = [];
  const endPattern =
    /^\s*} \/\/ end of method ([^\r\n]+::__js_(?:call|module_init)__)\s*$/gm;
  for (const end of il.matchAll(endPattern)) {
    const start = starts.filter((value) => value < end.index).at(-1);
    if (start === undefined) {
      throw new Error(`Could not find method start for ${end[1]}`);
    }
    bodies.push({
      owner: end[1],
      text: il.slice(start, end.index + end[0].length),
    });
  }
  return bodies;
}

function countMatches(text, pattern) {
  const matches = text.match(pattern);
  return matches ? matches.length : 0;
}

function inspectHotMethodIl(il, source) {
  const owner = findFindGraphNodeOwner(source);
  const bodies = extractSourceMethodBodies(il);
  const suffix = `${owner}::__js_call__`;
  const matches = bodies.filter((body) => body.owner === suffix);
  if (matches.length !== 1) {
    throw new Error(
      `Expected one canonical body for ${suffix}, found ${matches.length}`
    );
  }

  const body = matches[0].text;
  const result = {
    owner,
    codeSize: Number(
      /Code size:\s+\d+\s+\(0x([0-9a-f]+)\)/i.exec(body)?.[1]
        ? parseInt(
            /Code size:\s+\d+\s+\(0x([0-9a-f]+)\)/i.exec(body)[1],
            16
          )
        : NaN
    ),
    dynamicCachePropertyReads: countMatches(
      body,
      /DynamicLookupInlineCache::GetItem\(/g
    ),
    genericItemPropertyReads: countMatches(
      body,
      /ObjectRuntime::(?:GetItem|GetProperty)\(/g
    ),
    guardedConstructorFieldReads: countMatches(
      body,
      /isinst [^\r\n]*\/ObjectLiterals\/Ctor_[^\r\n]+[\s\S]{0,160}?callvirt instance object [^\r\n]*::get_[A-Za-z_$][A-Za-z0-9_$]*\(\)/g
    ),
    arrayLengthGenericNumericReads: countMatches(
      body,
      /ObjectRuntime::GetItemAsNumber\(/g
    ),
    boxInstructions: countMatches(body, /\bbox\s+/g),
    arity1MemberDispatches: countMatches(
      body,
      /ObjectRuntime::CallMember1\(/g
    ),
    cachedArity1MemberDispatches: countMatches(
      body,
      /DynamicLookupInlineCache::CallMember1\(/g
    ),
    findGraphNodeArity1CallSites: 0,
    cachedFindGraphNodeArity1CallSites: 0,
  };

  for (const callable of bodies) {
    result.findGraphNodeArity1CallSites += countMatches(
      callable.text,
      /ldstr "findGraphNode"[\s\S]{0,500}?ObjectRuntime::CallMember1\(/g
    );
    result.cachedFindGraphNodeArity1CallSites += countMatches(
      callable.text,
      /ldstr "findGraphNode"[\s\S]{0,500}?DynamicLookupInlineCache::CallMember1\(/g
    );
  }
  return result;
}

function runIlInspection(repoRoot, keepArtifacts) {
  runChecked("ilspycmd", ["--version"], { cwd: repoRoot, stdio: "pipe" });
  const artifactsDirectory = path.join(repoRoot, "artifacts");
  fs.mkdirSync(artifactsDirectory, { recursive: true });
  const artifactRoot = fs.mkdtempSync(
    path.join(artifactsDirectory, "jroc-kracken-ai-astar-")
  );
  try {
    const source = composeFixture(repoRoot);
    const sourcePath = path.join(artifactRoot, `${TARGET_SCENARIO}.js`);
    const outputDirectory = path.join(artifactRoot, "compiled");
    fs.mkdirSync(outputDirectory, { recursive: true });
    fs.writeFileSync(sourcePath, source, "utf8");

    runChecked(
      "dotnet",
      [
        "run",
        "-c",
        "Release",
        "--project",
        path.join(repoRoot, "src", "Cli"),
        "--",
        sourcePath,
        outputDirectory,
      ],
      { cwd: repoRoot, stdio: "pipe" }
    );
    const dllPath = path.join(outputDirectory, `${TARGET_SCENARIO}.dll`);
    const il = runChecked("ilspycmd", ["-il", dllPath], {
      cwd: repoRoot,
      stdio: "pipe",
    }).stdout;
    fs.writeFileSync(path.join(artifactRoot, `${TARGET_SCENARIO}.il`), il, "utf8");

    const result = inspectHotMethodIl(il, source);
    result.artifactRoot = keepArtifacts ? artifactRoot : null;
    printIlCounters(result);
    return result;
  } finally {
    if (keepArtifacts) {
      console.log(`Kept IL artifacts: ${artifactRoot}`);
    } else {
      fs.rmSync(artifactRoot, { recursive: true, force: true });
    }
  }
}

function printIlCounters(result) {
  console.log("");
  console.log("findGraphNode generated-IL counters");
  console.log("===================================");
  console.log(`Owner: ${result.owner}::__js_call__`);
  console.log(`Method IL bytes:                         ${result.codeSize}`);
  console.log(
    `Dynamic-cache property reads:            ${result.dynamicCachePropertyReads}`
  );
  console.log(
    `Generic item/property reads:              ${result.genericItemPropertyReads}`
  );
  console.log(
    `Guarded constructor field reads:          ${result.guardedConstructorFieldReads}`
  );
  console.log(
    `Generic numeric Array.length reads:       ${result.arrayLengthGenericNumericReads}`
  );
  console.log(
    `Explicit box instructions:                ${result.boxInstructions}`
  );
  console.log(
    `Arity-1 dispatches inside findGraphNode:   ${result.arity1MemberDispatches}`
  );
  console.log(
    `Cached arity-1 dispatches in findGraphNode:${result.cachedArity1MemberDispatches}`
  );
  console.log(
    `Arity-1 calls to findGraphNode elsewhere: ${result.findGraphNodeArity1CallSites}`
  );
  console.log(
    `Cached calls to findGraphNode elsewhere:  ${result.cachedFindGraphNodeArity1CallSites}`
  );
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  const repoRoot = findRepoRoot(__dirname);
  const resultsDirectory = path.join(
    repoRoot,
    "tests",
    "performance",
    "Benchmarks",
    "BenchmarkDotNet.Artifacts",
    "results"
  );
  const reportPath = args.resultsFile
    ? path.resolve(repoRoot, args.resultsFile)
    : path.join(
        resultsDirectory,
        "Benchmarks.KrackenExecutionBenchmarks-report-full-compressed.json"
      );
  const microReportPath = args.microResultsFile
    ? path.resolve(repoRoot, args.microResultsFile)
    : path.join(
        resultsDirectory,
        "Benchmarks.DynamicLookupInlineCacheBenchmarks-report-full-compressed.json"
      );

  const candidate = runKrackenBenchmark(repoRoot, args, reportPath);
  if (candidate) {
    printKrackenReport("Kraken candidate", candidate);
  }
  const microbenchmarks = runMicrobenchmarks(repoRoot, args, microReportPath);
  printMicrobenchmarks(microbenchmarks);
  const ilCounters = args.inspectIl
    ? runIlInspection(repoRoot, args.keepIlArtifacts)
    : null;

  let baseline = null;
  let comparison = null;
  if (args.baselineReport) {
    if (!candidate) {
      throw new Error("A candidate Kraken report is required for comparison");
    }
    baseline = parseKrackenReport(
      path.resolve(repoRoot, args.baselineReport),
      args.baselineSha || "unknown"
    );
    printKrackenReport("Kraken baseline", baseline);
    comparison = compareJrocReports(
      baseline,
      candidate,
      args.tolerancePercent
    );
    printComparison(comparison);
  }

  const summary = {
    generatedAt: new Date().toISOString(),
    dry: args.dry,
    tolerancePercent: args.tolerancePercent,
    candidate,
    baseline,
    comparison,
    microbenchmarks,
    ilCounters,
  };
  const outputPath = args.outputJson
    ? path.resolve(repoRoot, args.outputJson)
    : path.join(resultsDirectory, "KrackenAiAStarGuardrails-summary.json");
  fs.mkdirSync(path.dirname(outputPath), { recursive: true });
  fs.writeFileSync(outputPath, `${JSON.stringify(summary, null, 2)}\n`, "utf8");
  console.log(`\nWrote summary JSON: ${outputPath}`);

  if (comparison?.regression && !args.allowRegression) {
    process.exitCode = 2;
  }
}

module.exports = {
  compareJrocReports,
  extractSourceMethodBodies,
  findFindGraphNodeOwner,
  hostCompatibilityProblems,
  inspectHotMethodIl,
  parseKrackenReport,
  parseMicrobenchmarkReport,
};

if (require.main === module) {
  try {
    main();
  } catch (error) {
    console.error(`\nERROR: ${error.message}`);
    process.exit(1);
  }
}
