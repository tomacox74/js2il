const simpleBenchmarks = [];
const globals = typeof globalThis === "object" ? globalThis : {};

function createHandle() {
  const handle = {
    gc() { return handle; },
    args() { return handle; },
    highlight() { return handle; },
    compact() { return handle; },
  };

  return handle;
}

export function bench(name, fn) {
  if (typeof name === "function") {
    fn = name;
    name = fn.name || "anonymous";
  }

  simpleBenchmarks.push({ name, fn });
  return createHandle();
}

export function group(name, fn) {
  return (typeof name === "function" ? name : fn)();
}

export function summary(name, fn) {
  return (typeof name === "function" ? name : fn)();
}

export function run() {
  const iterations = Number(globals.__BENCHMARK_ITERATIONS ?? 1);
  const runsPerBenchmark = Number.isFinite(iterations) && iterations > 0 ? Math.floor(iterations) : 1;
  const now = globals.performance?.now
    ? () => globals.performance.now() * 1e6
    : () => Date.now() * 1e6;
  const benchmarks = [];

  for (const benchmark of simpleBenchmarks) {
    let totalNs = 0;
    let error = null;

    try {
      for (let iteration = 0; iteration < runsPerBenchmark; iteration++) {
        const start = now();
        const result = benchmark.fn();
        if (result instanceof Promise) {
          throw new Error("The managed benchmark runner does not support asynchronous benchmarks.");
        }
        totalNs += now() - start;
      }
    } catch (caught) {
      error = caught;
    }

    benchmarks.push({
      name: benchmark.name,
      iterations: runsPerBenchmark,
      avgNs: error ? null : totalNs / runsPerBenchmark,
      totalNs: error ? null : totalNs,
      error: error ? { message: error.message ?? String(error) } : null,
    });
  }

  console.log(JSON.stringify({
    runtime: String(globals.__BENCHMARK_RUNTIME ?? "managed"),
    iterations: runsPerBenchmark,
    benchmarks,
  }));
}
