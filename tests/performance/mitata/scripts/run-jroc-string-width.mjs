import { spawnSync } from "node:child_process";
import { rmSync, mkdirSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url));
const benchmarkDirectory = path.resolve(scriptDirectory, "..");
const repoRoot = path.resolve(benchmarkDirectory, "..", "..", "..");
const inputFile = process.env.JROC_BENCHMARK_INPUT
  ? path.resolve(benchmarkDirectory, process.env.JROC_BENCHMARK_INPUT)
  : path.join(benchmarkDirectory, "snippets", "string-width.mjs");
const benchmarkName = path.basename(inputFile, path.extname(inputFile));
const outputDirectory = path.join(benchmarkDirectory, ".jroc", benchmarkName);
const outputAssembly = path.join(outputDirectory, `${benchmarkName}.dll`);

function run(command, args, options = {}) {
  const result = spawnSync(command, args, {
    cwd: benchmarkDirectory,
    env: {
      ...process.env,
      JROC_SIMPLE_BENCH_RUNNER: "1",
    },
    stdio: "inherit",
    ...options,
  });

  if (result.error) {
    throw result.error;
  }

  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
}

function compileWithJroc() {
  const jroc = process.env.JROC;

  if (jroc) {
    if (jroc.endsWith(".dll")) {
      run("dotnet", [jroc, inputFile, outputDirectory]);
      return;
    }

    run(jroc, [inputFile, outputDirectory]);
    return;
  }

  run("dotnet", ["run", "--project", path.join(repoRoot, "src", "Cli"), "--", inputFile, outputDirectory], {
    cwd: repoRoot,
  });
}

rmSync(outputDirectory, { recursive: true, force: true });
mkdirSync(outputDirectory, { recursive: true });

compileWithJroc();
run("dotnet", [outputAssembly]);
