"use strict";

const assert = require("node:assert/strict");
const childProcess = require("node:child_process");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const test = require("node:test");
const {
  compareJrocReports,
  evaluateFinalValidation,
  inspectHotMethodIl,
  parseKrackenReport,
} = require("./runKrackenAStarGuardrails");
const scriptPath = path.join(__dirname, "runKrackenAStarGuardrails.js");

const runtimes = [
  ["RunJrocTest", 100, 90, 10, 15, 200],
  ["RunOkojoTest", 120, 118, 12, 13, 20],
  ["RunJintTest", 110, 108, 11, 14, 30],
  ["RunYantraJsTest", 95, 93, 9, 15, 400],
];

function createReport(filePath, jrocMean = 100, processorName = "Test CPU") {
  const report = {
    Title: "Benchmarks.KrackenExecutionBenchmarks",
    HostEnvironmentInfo: {
      BenchmarkDotNetVersion: "0.15.8",
      OsVersion: "Test OS",
      ProcessorName: processorName,
      PhysicalProcessorCount: 1,
      PhysicalCoreCount: 2,
      LogicalCoreCount: 4,
      RuntimeVersion: ".NET 10",
      Architecture: "X64",
      Configuration: "RELEASE",
      DotNetCliVersion: "10.0.100",
    },
    Benchmarks: runtimes.map(
      ([
        method,
        mean,
        median,
        standardDeviation,
        sampleCount,
        allocatedBytes,
      ]) => ({
        Method: method,
        Parameters: "ScriptName=kracken-ai-astar.js",
        DisplayInfo:
          `${method} [ScriptName=kracken-ai-astar.js]`,
        Statistics: {
          Mean: method === "RunJrocTest" ? jrocMean : mean,
          Median: median,
          StandardDeviation: standardDeviation,
          N: sampleCount,
        },
        Memory: {
          BytesAllocatedPerOperation: allocatedBytes,
        },
        Metrics: [
          {
            Value: 1,
            Descriptor: { Id: "Gen0Collects" },
          },
          {
            Value: 0,
            Descriptor: { Id: "Gen1Collects" },
          },
          {
            Value: 0,
            Descriptor: { Id: "Gen2Collects" },
          },
        ],
      })
    ),
  };
  fs.writeFileSync(filePath, `${JSON.stringify(report)}\n`, "utf8");
}

test("parses exact Kraken rows with raw provenance", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "jroc-kraken-report-"));
  try {
    const reportPath = path.join(directory, "report.json");
    createReport(reportPath);
    const parsed = parseKrackenReport(reportPath, "abc123");

    assert.equal(parsed.sha, "abc123");
    assert.equal(parsed.host.processorName, "Test CPU");
    assert.equal(parsed.rows.length, 4);
    assert.deepEqual(
      parsed.rows.find((row) => row.runtime === "jroc-execute"),
      {
        scenario: "kracken-ai-astar",
        runtime: "jroc-execute",
        meanNs: 100,
        medianNs: 90,
        standardDeviationNs: 10,
        sampleCount: 15,
        allocatedBytes: 200,
        gen0CollectionsPer1000Operations: 1,
        gen1CollectionsPer1000Operations: 0,
        gen2CollectionsPer1000Operations: 0,
        displayInfo:
          "RunJrocTest [ScriptName=kracken-ai-astar.js]",
      }
    );
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test("evaluates required and stretch Phase 6 gates", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "jroc-kraken-final-"));
  try {
    const reportPath = path.join(directory, "report.json");
    createReport(reportPath);
    const validation = evaluateFinalValidation(
      parseKrackenReport(reportPath, "candidate"),
      {
        arrayLengthGenericNumericReads: 0,
        guardedConstructorFieldReads: 2,
        dynamicCachePropertyReads: 0,
        findGraphNodeArity1CallSites: 2,
        cachedFindGraphNodeArity1CallSites: 2,
      }
    );

    assert.equal(validation.requiredPassed, true);
    assert.equal(validation.stretchPassed, true);
    assert.equal(validation.checks.length, 8);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test("fails Phase 6 when JROC trails a required competitor", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "jroc-kraken-fail-"));
  try {
    const reportPath = path.join(directory, "report.json");
    createReport(reportPath, 130);
    const validation = evaluateFinalValidation(
      parseKrackenReport(reportPath, "candidate"),
      {
        arrayLengthGenericNumericReads: 0,
        guardedConstructorFieldReads: 2,
        dynamicCachePropertyReads: 0,
        findGraphNodeArity1CallSites: 2,
        cachedFindGraphNodeArity1CallSites: 2,
      }
    );

    assert.equal(validation.requiredPassed, false);
    assert.equal(
      validation.checks.find(
        (check) => check.name === "JROC no slower than Jint"
      ).passed,
      false
    );
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test("flags same-host regressions above tolerance", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "jroc-kraken-compare-"));
  try {
    const baselinePath = path.join(directory, "baseline.json");
    const candidatePath = path.join(directory, "candidate.json");
    createReport(baselinePath, 100);
    createReport(candidatePath, 106);
    const comparison = compareJrocReports(
      parseKrackenReport(baselinePath, "base"),
      parseKrackenReport(candidatePath, "candidate"),
      5
    );

    assert.equal(comparison.regression, true);
    assert.equal(comparison.meanChangePercent, 6);
    assert.equal(comparison.lowSampleWarning, false);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test("marks single-sample comparisons as low confidence", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "jroc-kraken-dry-"));
  try {
    const baselinePath = path.join(directory, "baseline.json");
    const candidatePath = path.join(directory, "candidate.json");
    createReport(baselinePath, 100);
    createReport(candidatePath, 101);
    for (const reportPath of [baselinePath, candidatePath]) {
      const report = JSON.parse(fs.readFileSync(reportPath, "utf8"));
      report.Benchmarks.find(
        (benchmark) => benchmark.Method === "RunJrocTest"
      ).Statistics.N = 1;
      fs.writeFileSync(reportPath, JSON.stringify(report), "utf8");
    }

    const comparison = compareJrocReports(
      parseKrackenReport(baselinePath, "base"),
      parseKrackenReport(candidatePath, "candidate"),
      5
    );

    assert.equal(comparison.regression, false);
    assert.equal(comparison.lowSampleWarning, true);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test("returns exit code 2 for a regression above tolerance", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "jroc-kraken-exit-"));
  try {
    const baselinePath = path.join(directory, "baseline.json");
    const candidatePath = path.join(directory, "candidate.json");
    const summaryPath = path.join(directory, "summary.json");
    createReport(baselinePath, 100);
    createReport(candidatePath, 106);

    const result = childProcess.spawnSync(
      process.execPath,
      [
        scriptPath,
        "--no-run",
        "--no-microbenchmarks",
        "--results-file",
        candidatePath,
        "--candidate-sha",
        "candidate",
        "--baseline-report",
        baselinePath,
        "--baseline-sha",
        "base",
        "--tolerance-percent",
        "5",
        "--output-json",
        summaryPath,
      ],
      {
        encoding: "utf8",
      }
    );

    assert.equal(result.status, 2, result.stderr);
    assert.match(result.stdout, /REGRESSION: allowed mean regression 5\.00%/);
    assert.equal(JSON.parse(fs.readFileSync(summaryPath, "utf8")).comparison.regression, true);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test("refuses cross-host comparisons", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "jroc-kraken-host-"));
  try {
    const baselinePath = path.join(directory, "baseline.json");
    const candidatePath = path.join(directory, "candidate.json");
    createReport(baselinePath, 100, "CPU A");
    createReport(candidatePath, 100, "CPU B");

    assert.throws(
      () =>
        compareJrocReports(
          parseKrackenReport(baselinePath, "base"),
          parseKrackenReport(candidatePath, "candidate"),
          5
        ),
      /Refusing cross-host comparison/
    );
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test("counts only the canonical findGraphNode method and its call sites", () => {
  const source = [
    "var x = 1;",
    "Array.prototype.findGraphNode = function(obj) { return obj; };",
  ].join("\n");
  const il = `
.method public hidebysig static object __js_call__ () cil managed
{
  // Code size: 42 (0x2a)
  call float64 JavaScriptRuntime.ObjectRuntime::GetItemAsNumber(object, object)
  call object JavaScriptRuntime.ObjectRuntime::GetItem(object, float64)
  call object JavaScriptRuntime.DynamicLookupInlineCache::GetItem(object, string, string)
  call object JavaScriptRuntime.DynamicLookupInlineCache::GetItem(object, string, string)
} // end of method FunctionExpression_L2C32::__js_call__

.method public hidebysig static object __js_call__ () cil managed
{
  // Code size: 20 (0x14)
  ldstr "findGraphNode"
  call object JavaScriptRuntime.ObjectRuntime::CallMember1(object, string, object)
} // end of method caller::__js_call__
`;

  assert.deepEqual(inspectHotMethodIl(il, source), {
    owner: "FunctionExpression_L2C32",
    codeSize: 42,
    dynamicCachePropertyReads: 2,
    genericItemPropertyReads: 1,
    guardedConstructorFieldReads: 0,
    arrayLengthGenericNumericReads: 1,
    boxInstructions: 0,
    arity1MemberDispatches: 0,
    cachedArity1MemberDispatches: 0,
    findGraphNodeArity1CallSites: 1,
    cachedFindGraphNodeArity1CallSites: 0,
  });
});
