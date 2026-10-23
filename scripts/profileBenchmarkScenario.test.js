"use strict";

const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const { resolveScenario, runnerProgram } = require("./profileBenchmarkScenario");

test("oscillator profiling composes the benchmark's unchanged data and workload", () => {
  const directory = path.join(__dirname, "..", "tests", "performance", "Benchmarks", "Scenarios", "kracken-1.1");
  const data = fs.readFileSync(path.join(directory, "audio-dft-data.js"), "utf8");
  const workload = fs.readFileSync(path.join(directory, "audio-oscillator.js"), "utf8");
  const scenario = resolveScenario("audio-oscillator");
  assert.equal(scenario.mode, "calltest");
  assert.ok(scenario.source.startsWith(data + "\n"));
  assert.ok(scenario.source.includes(workload));
  assert.ok(scenario.source.includes("__jrocKrackenWorkload();"));
});

test("profiling uses benchmark compile options and process-wide measured-phase accounting", () => {
  assert.match(runnerProgram, /AssumeUnmodifiedHostGlobals = true/);
  assert.match(runnerProgram, /EmitPdb = true/);
  assert.match(runnerProgram, /GC\.GetTotalAllocatedBytes\(precise: true\)/);
  assert.doesNotMatch(runnerProgram, /GetAllocatedBytesForCurrentThread/);
  assert.match(runnerProgram, /BenchmarkProfile\.Log\.Begin\(i\)/);
  assert.match(runnerProgram, /BenchmarkProfile\.Log\.End\(i\)/);
});
