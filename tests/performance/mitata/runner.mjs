import * as Mitata from "mitata";
import process from "node:process";

const useSimpleRunner = !!process?.env?.JROC_SIMPLE_BENCH_RUNNER;
const simpleBenchmarks = [];

function createSimpleBenchHandle() {
  const handle = {
    gc() { return handle; },
    args() { return handle; },
    highlight() { return handle; },
    compact() { return handle; },
  };

  return handle;
}

function simpleBench(name, fn) {
  if (typeof name === "function") {
    fn = name;
    name = fn.name || "anonymous";
  }

  simpleBenchmarks.push({ name, fn });
  return createSimpleBenchHandle();
}

function simpleGroup(name, fn) {
  if (typeof name === "function") {
    fn = name;
  }

  return fn();
}

function formatDurationNs(ns) {
  const value = Number(ns);

  if (!Number.isFinite(value)) {
    return String(ns);
  }

  const roundToHundredths = current => `${Math.round(current * 100) / 100}`;

  if (value < 1e3) {
    return `${roundToHundredths(value)} ns`;
  }

  if (value < 1e6) {
    return `${roundToHundredths(value / 1e3)} us`;
  }

  if (value < 1e9) {
    return `${roundToHundredths(value / 1e6)} ms`;
  }

  return `${roundToHundredths(value / 1e9)} s`;
}

async function simpleRun() {
  const now = globalThis.performance?.now
    ? () => globalThis.performance.now() * 1e6
    : () => Date.now() * 1e6;
  const iterations = Number.parseInt(process?.env?.JROC_SIMPLE_BENCH_ITERATIONS ?? "", 10);
  const runsPerBenchmark = Number.isFinite(iterations) && iterations > 0 ? iterations : 1;
  const results = [];

  for (const benchmark of simpleBenchmarks) {
    let error = null;
    let totalNs = 0;

    try {
      console.log(`running ${benchmark.name}`);
      for (let i = 0; i < runsPerBenchmark; i++) {
        const start = now();
        const result = benchmark.fn();
        if (result instanceof Promise) {
          await result;
        }

        totalNs += now() - start;
      }
    }
    catch (err) {
      error = err;
    }

    results.push({
      name: benchmark.name,
      iterations: runsPerBenchmark,
      avgNs: error ? null : totalNs / runsPerBenchmark,
      totalNs: error ? null : totalNs,
      error,
    });
  }

  if (process?.env?.BENCHMARK_RUNNER) {
    console.log(JSON.stringify({
      runtime: "jroc",
      iterations: runsPerBenchmark,
      benchmarks: results.map(result => ({
        ...result,
        error: result.error ? { message: result.error.message ?? String(result.error) } : null,
      })),
    }));

    return { benchmarks: results };
  }

  const nameWidth = Math.max("benchmark".length, ...results.map(result => result.name.length));
  console.log(`${"benchmark".padEnd(nameWidth)}  avg`);
  console.log(`${"-".repeat(nameWidth)}  ${"-".repeat(12)}`);

  for (const result of results) {
    if (result.error) {
      console.log(`${result.name.padEnd(nameWidth)}  error: ${result.error.message ?? String(result.error)}`);
      continue;
    }

    console.log(`${result.name.padEnd(nameWidth)}  ${formatDurationNs(result.avgNs)}`);
  }

  return { benchmarks: results };
}

export const bench = useSimpleRunner ? simpleBench : Mitata.bench;
export const group = useSimpleRunner ? simpleGroup : Mitata.group;
export const summary = useSimpleRunner ? simpleGroup : Mitata.summary;

/** @param {Parameters<typeof Mitata["run"]>["0"]} opts */
export function run(opts = {}) {
  if (useSimpleRunner) {
    return simpleRun();
  }

  if (process?.env?.BENCHMARK_RUNNER) {
    opts.format = "json";
  }

  return Mitata.run(opts);
}
