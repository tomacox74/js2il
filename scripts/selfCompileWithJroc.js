"use strict";

const fs = require("node:fs");
const path = require("node:path");
const cp = require("node:child_process");

function maybeRunCompiledWithJroc({
  scriptPath,
  outputDirectory,
  assemblyName,
  bootstrapEnvironmentVariable,
  nodeOnlyEnvironmentVariable,
}) {
  if (!scriptPath || !outputDirectory || !assemblyName) {
    throw new Error("Self-compilation requires a script path, output directory, and assembly name.");
  }

  if (process.env[bootstrapEnvironmentVariable] === "1") {
    return;
  }

  if (process.env[nodeOnlyEnvironmentVariable] === "1" || process.argv.includes("--node-only")) {
    return;
  }

  const argv0 = path.basename((process.argv[0] || "").toLowerCase());
  const runningViaNode = argv0 === "node" || argv0 === "node.exe";
  if (!runningViaNode) {
    return;
  }

  fs.rmSync(outputDirectory, { recursive: true, force: true });
  fs.mkdirSync(outputDirectory, { recursive: true });

  const repositoryRoot = path.resolve(__dirname, "..");
  const compile = cp.spawnSync("jroc", [scriptPath, outputDirectory], {
    stdio: "inherit",
    cwd: repositoryRoot,
  });
  if (compile.error) {
    throw compile.error;
  }
  if (compile.status !== 0) {
    process.exit(compile.status ?? 1);
  }

  const dllPath = path.join(outputDirectory, `${assemblyName}.dll`);
  if (!fs.existsSync(dllPath)) {
    throw new Error(`Compiled assembly not found: ${dllPath}`);
  }

  const runCompiled = cp.spawnSync("dotnet", [dllPath, ...process.argv.slice(2)], {
    stdio: "inherit",
    cwd: repositoryRoot,
    env: { ...process.env, [bootstrapEnvironmentVariable]: "1" },
  });
  if (runCompiled.error) {
    throw runCompiled.error;
  }

  process.exit(runCompiled.status ?? 1);
}

module.exports = { maybeRunCompiledWithJroc };
