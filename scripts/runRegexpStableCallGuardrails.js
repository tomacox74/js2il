#!/usr/bin/env node
"use strict";

const childProcess = require("node:child_process");
const fs = require("node:fs");
const path = require("node:path");

const TARGET_SCENARIOS = [
  "dromaeo-object-regexp",
  "dromaeo-object-regexp-modern",
];
const RUNTIME_BY_METHOD = {
  Jroc_ExecuteOnly: "jroc-execute",
  Jint_ExecutePrepared: "jint-execute-prepared",
  Okojo_ExecuteOnly: "okojo-execute",
};
const DISPATCH_PATTERN =
  /(?:Closure::InvokeFunctionCallWithArgs\d*|Closure::InvokeWithArgs\d*|CallableOperations::Call)\b/g;

function printUsage() {
  console.log("Usage: node scripts/runRegexpStableCallGuardrails.js [options]");
  console.log("");
  console.log("Runs only the classic/modern dromaeo-object-regexp phased pair.");
  console.log("");
  console.log("Options:");
  console.log("  --dry                Use a quick Dry BenchmarkDotNet job.");
  console.log("  --no-run             Skip benchmarks (combine with --il for IL-only validation).");
  console.log("  --il                 Compile/decompile the pair and enforce stable-call IL guardrails.");
  console.log("  --keep-il-artifacts  Keep generated assemblies and IL under artifacts/.");
  console.log("  --results-file PATH  Override the BenchmarkDotNet full-compressed JSON path.");
  console.log("  --output-json PATH   Write the combined benchmark/IL summary.");
  console.log("  -h, --help           Show this help.");
}

function parseArgs(argv) {
  const args = {
    dry: false,
    noRun: false,
    inspectIl: false,
    keepIlArtifacts: false,
    resultsFile: "",
    outputJson: "",
  };

  for (let i = 0; i < argv.length; i += 1) {
    switch (argv[i]) {
      case "--dry":
        args.dry = true;
        break;
      case "--no-run":
        args.noRun = true;
        break;
      case "--il":
        args.inspectIl = true;
        break;
      case "--keep-il-artifacts":
        args.keepIlArtifacts = true;
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
        throw new Error(`Unknown argument: ${argv[i]}`);
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

function parseRows(reportPath, scenario) {
  const report = JSON.parse(fs.readFileSync(reportPath, "utf8"));
  const rows = [];
  for (const benchmark of report?.Benchmarks || []) {
    const runtime = RUNTIME_BY_METHOD[benchmark?.Method];
    if (!runtime || parseScenarioName(benchmark) !== scenario) {
      continue;
    }

    rows.push({
      scenario,
      runtime,
      meanMs: Number.isFinite(benchmark?.Statistics?.Mean)
        ? benchmark.Statistics.Mean / 1_000_000
        : null,
      allocatedMiB: Number.isFinite(benchmark?.Memory?.BytesAllocatedPerOperation)
        ? benchmark.Memory.BytesAllocatedPerOperation / (1024 * 1024)
        : null,
      sampleCount: Number.isFinite(benchmark?.Statistics?.N)
        ? benchmark.Statistics.N
        : null,
    });
  }

  for (const runtime of Object.values(RUNTIME_BY_METHOD)) {
    if (!rows.some((row) => row.runtime === runtime)) {
      throw new Error(`Missing benchmark row for ${scenario}|${runtime}`);
    }
  }
  return rows;
}

function printRows(rows) {
  if (rows.length === 0) {
    return;
  }

  console.log("");
  console.log("Exact regexp phased pair");
  console.log("========================");
  console.log("Scenario                         Runtime                  Mean(ms)  Alloc(MiB)  N");
  for (const row of rows) {
    console.log(
      `${row.scenario.padEnd(32)} ${row.runtime.padEnd(24)} ` +
      `${format(row.meanMs).padStart(8)}  ${format(row.allocatedMiB).padStart(10)}  ` +
      `${String(row.sampleCount ?? "n/a")}`
    );
  }
}

function format(value) {
  return Number.isFinite(value) ? value.toFixed(2) : "n/a";
}

function runBenchmarks(repoRoot, args, reportPath) {
  if (args.noRun) {
    console.log("Skipping benchmark execution (--no-run).");
    if (!fs.existsSync(reportPath)) {
      return [];
    }

    const rows = [];
    for (const scenario of TARGET_SCENARIOS) {
      try {
        rows.push(...parseRows(reportPath, scenario));
      } catch (error) {
        if (args.resultsFile) {
          throw error;
        }
        console.log(`Existing report does not contain ${scenario}; skipping its benchmark rows.`);
      }
    }
    return rows;
  }

  const benchmarkProject = path.join(
    repoRoot,
    "tests",
    "performance",
    "Benchmarks",
    "Benchmarks.csproj"
  );
  const benchmarkDirectory = path.dirname(benchmarkProject);
  const rows = [];

  for (const scenario of TARGET_SCENARIOS) {
    const dotnetArgs = [
      "run",
      "-c",
      "Release",
      "--project",
      benchmarkProject,
      "--",
      "--dromaeo",
      "--filter",
      "*DromaeoExecutionBenchmarks*",
      "--scenario",
      scenario,
    ];
    if (args.dry) {
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

    runChecked("dotnet", dotnetArgs, { cwd: benchmarkDirectory });
    rows.push(...parseRows(reportPath, scenario));
  }

  return rows;
}

function countMatches(text, pattern) {
  const matches = text.match(pattern);
  return matches ? matches.length : 0;
}

function escapeRegex(value) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function countSourceCalls(source, name) {
  const allCallShapes = countMatches(
    source,
    new RegExp(`\\b${escapeRegex(name)}\\s*\\(`, "g")
  );
  const declarations = countMatches(
    source,
    new RegExp(`\\bfunction\\s+${escapeRegex(name)}\\s*\\(`, "g")
  );
  return allCallShapes - declarations;
}

function findConstArrowOwner(source, name) {
  const lines = source.split(/\r?\n/);
  const declaration = new RegExp(`^\\s*const\\s+${escapeRegex(name)}\\s*=\\s*`);
  for (let index = 0; index < lines.length; index += 1) {
    const match = declaration.exec(lines[index]);
    if (match) {
      const column = match[0].length + 1;
      return `ArrowFunction_L${index + 1}C${column}`;
    }
  }
  throw new Error(`Could not locate const arrow initializer for ${name}`);
}

function extractSourceMethodBodies(il) {
  const methodStarts = [...il.matchAll(/^[ \t]*\.method\b/gm)].map((match) => match.index);
  const bodies = [];
  const endPattern =
    /^\s*} \/\/ end of method ([^\r\n]+::__js_(?:call|module_init)__)\s*$/gm;

  for (const end of il.matchAll(endPattern)) {
    const start = methodStarts.filter((value) => value < end.index).at(-1);
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

function getCallableBody(bodies, owner) {
  const suffix = `${owner}::__js_call__`;
  const matches = bodies.filter((body) => body.owner === suffix);
  if (matches.length !== 1) {
    throw new Error(`Expected one canonical body for ${suffix}, found ${matches.length}`);
  }
  return matches[0].text;
}

function countDirectCalls(bodies, owner) {
  const pattern = new RegExp(
    `\\bcall\\s+[^\\r\\n]*\\/${escapeRegex(owner)}::__js_call__\\(`,
    "g"
  );
  return bodies.reduce((total, body) => total + countMatches(body.text, pattern), 0);
}

function assertEqual(actual, expected, message) {
  if (actual !== expected) {
    throw new Error(`${message}: expected ${expected}, actual ${actual}`);
  }
}

function inspectScenarioIl(repoRoot, scenario, artifactRoot) {
  const sourcePath = path.join(
    repoRoot,
    "tests",
    "performance",
    "Benchmarks",
    "Scenarios",
    "dromaeo",
    `${scenario}.js`
  );
  const outputDirectory = path.join(artifactRoot, scenario);
  fs.mkdirSync(outputDirectory, { recursive: true });

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

  const dllPath = path.join(outputDirectory, `${scenario}.dll`);
  const il = runChecked("ilspycmd", ["-il", dllPath], {
    cwd: repoRoot,
    stdio: "pipe",
  }).stdout;
  fs.writeFileSync(path.join(artifactRoot, `${scenario}.il`), il, "utf8");

  const source = fs.readFileSync(sourcePath, "utf8");
  const modern = scenario.endsWith("-modern");
  const randomOwner = modern
    ? findConstArrowOwner(source, "randomChar")
    : "randomChar";
  const generateOwner = modern
    ? findConstArrowOwner(source, "generateTestStrings")
    : "generateTestStrings";
  const bodies = extractSourceMethodBodies(il);
  const randomSourceCalls = countSourceCalls(source, "randomChar");
  const generateSourceCalls = countSourceCalls(source, "generateTestStrings");
  const randomDirectCalls = countDirectCalls(bodies, randomOwner);
  const generateDirectCalls = countDirectCalls(bodies, generateOwner);

  const result = {
    scenario,
    randomOwner,
    generateOwner,
    randomSourceCalls,
    randomDirectCalls,
    generateSourceCalls,
    generateDirectCalls,
    randomBodyDispatch: countMatches(
      getCallableBody(bodies, randomOwner),
      DISPATCH_PATTERN
    ),
    generateBodyDispatch: countMatches(
      getCallableBody(bodies, generateOwner),
      DISPATCH_PATTERN
    ),
    prepCallbackDispatch: countMatches(
      getCallableBody(bodies, "prep"),
      DISPATCH_PATTERN
    ),
    testCallbackDispatch: countMatches(
      getCallableBody(bodies, "test"),
      DISPATCH_PATTERN
    ),
  };

  if (modern) {
    assertEqual(
      result.randomDirectCalls,
      result.randomSourceCalls,
      `${scenario} randomChar source calls must all target the canonical MethodDef`
    );
    assertEqual(
      result.generateDirectCalls,
      result.generateSourceCalls,
      `${scenario} generateTestStrings source calls must all target the canonical MethodDef`
    );
    assertEqual(result.randomBodyDispatch, 0, `${scenario} randomChar hot body dispatch`);
    assertEqual(result.generateBodyDispatch, 0, `${scenario} generateTestStrings hot body dispatch`);
    assertEqual(result.prepCallbackDispatch, 1, `${scenario} prep callback boundary dispatch`);
    assertEqual(result.testCallbackDispatch, 1, `${scenario} test callback boundary dispatch`);
  }

  return result;
}

function runIlGuardrails(repoRoot, keepArtifacts) {
  runChecked("ilspycmd", ["--version"], { cwd: repoRoot, stdio: "pipe" });
  const artifactRoot = path.join(
    repoRoot,
    "artifacts",
    `issue768-regexp-stable-calls-${process.pid}`
  );
  fs.mkdirSync(artifactRoot, { recursive: true });

  try {
    const results = TARGET_SCENARIOS.map((scenario) =>
      inspectScenarioIl(repoRoot, scenario, artifactRoot)
    );
    console.log("");
    console.log("Stable-call IL guardrails");
    console.log("=========================");
    for (const result of results) {
      console.log(
        `${result.scenario}: ` +
        `randomChar direct ${result.randomDirectCalls}/${result.randomSourceCalls}, ` +
        `generateTestStrings direct ${result.generateDirectCalls}/${result.generateSourceCalls}, ` +
        `hot dispatch ${result.randomBodyDispatch}/${result.generateBodyDispatch}, ` +
        `prep/test callback dispatch ${result.prepCallbackDispatch}/${result.testCallbackDispatch}`
      );
    }
    console.log("Modern MethodDef call-site guardrails passed.");
    return results;
  } finally {
    if (keepArtifacts) {
      console.log(`Kept IL artifacts: ${artifactRoot}`);
    } else {
      fs.rmSync(artifactRoot, { recursive: true, force: true });
    }
  }
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  const repoRoot = findRepoRoot(__dirname);
  const reportPath = args.resultsFile
    ? path.resolve(repoRoot, args.resultsFile)
    : path.join(
        repoRoot,
        "tests",
        "performance",
        "Benchmarks",
        "BenchmarkDotNet.Artifacts",
        "results",
        "Benchmarks.DromaeoExecutionBenchmarks-report-full-compressed.json"
      );

  const rows = runBenchmarks(repoRoot, args, reportPath);
  printRows(rows);
  let ilGuardrails = [];
  if (args.inspectIl) {
    ilGuardrails = runIlGuardrails(repoRoot, args.keepIlArtifacts);
  }

  if (args.outputJson) {
    const outputPath = path.resolve(repoRoot, args.outputJson);
    fs.mkdirSync(path.dirname(outputPath), { recursive: true });
    fs.writeFileSync(
      outputPath,
      `${JSON.stringify({
        generatedAt: new Date().toISOString(),
        dry: args.dry,
        noRun: args.noRun,
        scenarios: TARGET_SCENARIOS,
        rows,
        ilGuardrails,
      }, null, 2)}\n`,
      "utf8"
    );
    console.log(`Wrote summary JSON: ${outputPath}`);
  }
}

try {
  main();
} catch (error) {
  console.error(`\nERROR: ${error.message}`);
  process.exit(1);
}
