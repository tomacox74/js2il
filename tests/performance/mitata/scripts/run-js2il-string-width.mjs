import { spawnSync } from "node:child_process";
import { rmSync, mkdirSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url));
const benchmarkDirectory = path.resolve(scriptDirectory, "..");
const repoRoot = path.resolve(benchmarkDirectory, "..", "..", "..");
const inputFile = path.join(benchmarkDirectory, "snippets", "string-width.mjs");
const outputDirectory = path.join(benchmarkDirectory, ".js2il", "string-width");
const outputAssembly = path.join(outputDirectory, "string-width.dll");

function run(command, args, options = {}) {
  const result = spawnSync(command, args, {
    cwd: benchmarkDirectory,
    env: {
      ...process.env,
      JS2IL_SIMPLE_BENCH_RUNNER: "1",
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

function compileWithJs2IL() {
  const js2il = process.env.JS2IL;

  if (js2il) {
    if (js2il.endsWith(".dll")) {
      run("dotnet", [js2il, inputFile, outputDirectory]);
      return;
    }

    run(js2il, [inputFile, outputDirectory]);
    return;
  }

  run("dotnet", ["run", "--project", path.join(repoRoot, "src", "Cli"), "--", inputFile, outputDirectory], {
    cwd: repoRoot,
  });
}

rmSync(outputDirectory, { recursive: true, force: true });
mkdirSync(outputDirectory, { recursive: true });

compileWithJs2IL();
run("dotnet", [outputAssembly]);
