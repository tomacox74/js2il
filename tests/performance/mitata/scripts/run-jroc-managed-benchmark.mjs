import { spawnSync } from "node:child_process";
import path from "node:path";
import { fileURLToPath } from "node:url";

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url));
const benchmarkDirectory = path.resolve(scriptDirectory, "..");
const benchmarkName = process.argv[2] ?? "runtime-baseline";

function run(command, args, env = process.env) {
  const result = spawnSync(command, args, {
    cwd: benchmarkDirectory,
    env,
    stdio: "inherit",
  });

  if (result.error) {
    throw result.error;
  }
  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
}

run("node", [path.join(scriptDirectory, "build-managed-benchmark.mjs"), benchmarkName]);
run("node", [path.join(scriptDirectory, "run-jroc-string-width.mjs")], {
  ...process.env,
  JROC_BENCHMARK_INPUT: `.managed/${benchmarkName}.js`,
});
