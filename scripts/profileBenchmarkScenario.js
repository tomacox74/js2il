#!/usr/bin/env node
"use strict";

/**
 * Profiles a benchmark scenario with dotnet-trace and opens the result in PerfView.
 *
 * Steps automated:
 *   1. Resolves the scenario script(s) under tests/performance/Benchmarks/Scenarios.
 *   2. Generates and builds a small Release runner that compiles the scenario
 *      to an assembly on disk (with a portable PDB for symbols), loads it with
 *      Assembly.LoadFrom, and executes it via JsEngine.LoadModule.
 *   3. Runs the runner under `dotnet-trace collect` (CPU sampling).
 *   4. Downloads PerfView (cached) if needed and opens the .nettrace file in it.
 *
 * Usage:
 *   node scripts/profileBenchmarkScenario.js <scenario-name> [--iterations N] [--no-open]
 *
 * Examples:
 *   node scripts/profileBenchmarkScenario.js ai-astar
 *   node scripts/profileBenchmarkScenario.js dromaeo-3d-cube --iterations 3
 *
 * Windows-only (PerfView). Requires the dotnet SDK; installs dotnet-trace
 * as a global tool if it is missing.
 */

const childProcess = require("node:child_process");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");

const repoRoot = path.resolve(__dirname, "..");
const scenariosDir = path.join(repoRoot, "tests", "performance", "Benchmarks", "Scenarios");
const krackenDir = path.join(scenariosDir, "kracken-1.1");
const workRoot = path.join(os.tmpdir(), "jroc-profile");
const perfViewExe = path.join(workRoot, "PerfView.exe");
const perfViewUrl = "https://github.com/microsoft/perfview/releases/latest/download/PerfView.exe";

function printUsage() {
  console.log("Usage: node scripts/profileBenchmarkScenario.js <scenario-name> [--iterations N] [--no-open] [--no-inline]");
  console.log("");
  console.log("Options:");
  console.log("  --iterations N   Number of timed iterations (default 2).");
  console.log("  --no-open        Skip opening the trace in PerfView.");
  console.log("  --no-inline      Run with DOTNET_JitNoInline=1 so every runtime helper");
  console.log("                   appears as a real frame in the trace (timings will skew).");
  console.log("");
  console.log("Examples:");
  console.log("  node scripts/profileBenchmarkScenario.js ai-astar");
  console.log("  node scripts/profileBenchmarkScenario.js dromaeo-3d-cube --iterations 3");
  console.log("");
  console.log("Available scenarios:");
  for (const name of listScenarios()) {
    console.log(`  ${name}`);
  }
}

function listScenarios() {
  const names = new Set();
  if (fs.existsSync(scenariosDir)) {
    for (const file of fs.readdirSync(scenariosDir)) {
      if (file.endsWith(".js")) names.add(file.replace(/\.js$/i, ""));
    }
  }
  if (fs.existsSync(krackenDir)) {
    for (const file of fs.readdirSync(krackenDir)) {
      if (file.endsWith(".js") && !file.endsWith("-data.js")) names.add(file.replace(/\.js$/i, ""));
    }
  }
  return [...names].sort();
}

function normalizeScenarioName(input) {
  const trimmed = String(input ?? "").trim();
  if (!trimmed) return "";
  return path.basename(trimmed).replace(/\.js$/i, "");
}

/**
 * Resolves the scenario to a single composed script and an execution mode.
 * - Kracken scenarios (kracken-1.1/<name>.js) are concatenated with their
 *   optional <name>-data.js and wrapped with an exported runTest() that calls
 *   go(), matching KrackenBenchmarks.cs. Mode "calltest": load once, time
 *   repeated runTest() calls.
 * - Plain scenarios (Scenarios/<name>.js) run their workload at module load.
 *   Mode "reload": compile once, time each LoadModule.
 */
function resolveScenario(name) {
  const krackenScript = path.join(krackenDir, `${name}.js`);
  if (fs.existsSync(krackenScript)) {
    const dataScript = path.join(krackenDir, `${name}-data.js`);
    const parts = [];
    if (fs.existsSync(dataScript)) parts.push(fs.readFileSync(dataScript, "utf8"));
    parts.push(fs.readFileSync(krackenScript, "utf8"));
    parts.push("export function runTest() { go(); return 'done'; }");
    return { source: parts.join("\n"), mode: "calltest" };
  }

  const plainScript = path.join(scenariosDir, `${name}.js`);
  if (fs.existsSync(plainScript)) {
    return { source: fs.readFileSync(plainScript, "utf8"), mode: "reload" };
  }

  return null;
}

const runnerCsproj = `<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="${path.join(repoRoot, "src", "Jroc.Core", "Jroc.Core.csproj")}" />
    <ProjectReference Include="${path.join(repoRoot, "src", "JavaScriptRuntime", "JavaScriptRuntime.csproj")}" />
  </ItemGroup>
</Project>
`;

const runnerProgram = `using System.Diagnostics;
using System.Reflection;
using Jroc;
using Jroc.Runtime;

// args: <scriptPath> <mode:calltest|reload> <iterations>
var scriptPath = args[0];
var mode = args[1];
var iterations = args.Length > 2 ? int.Parse(args[2]) : 2;

// Compile the scenario to an assembly on disk (with symbols) so PerfView can
// resolve the generated methods back to the scenario source.
var outDir = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(scriptPath))!, "out");
Directory.CreateDirectory(outDir);

var compileWatch = Stopwatch.StartNew();
var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(Path.GetFullPath(scriptPath))
{
    EmitPdb = true
});
var dllPath = Path.Combine(outDir, artifact.AssemblyName + ".dll");
File.WriteAllBytes(dllPath, artifact.PeBytes);
if (artifact.PdbBytes is { Length: > 0 } pdbBytes)
{
    File.WriteAllBytes(Path.Combine(outDir, artifact.AssemblyName + ".pdb"), pdbBytes);
}
compileWatch.Stop();
Console.WriteLine($"compile: {compileWatch.Elapsed.TotalSeconds:F1} s -> {dllPath}");

var assembly = Assembly.LoadFrom(dllPath);
var moduleId = artifact.ModuleIds[0];

if (mode == "calltest")
{
    using var exports = (IDisposable)JsEngine.LoadModule(assembly, moduleId);
    dynamic d = exports;
    for (int i = 0; i < iterations; i++)
    {
        var sw = Stopwatch.StartNew();
        var result = d.runTest();
        sw.Stop();
        Console.WriteLine($"run {i}: {sw.Elapsed.TotalSeconds:F2} s ({result})");
    }
}
else
{
    for (int i = 0; i < iterations; i++)
    {
        var sw = Stopwatch.StartNew();
        using (var exports = (IDisposable)JsEngine.LoadModule(assembly, moduleId))
        {
        }
        sw.Stop();
        Console.WriteLine($"run {i}: {sw.Elapsed.TotalSeconds:F2} s");
    }
}
`;

function run(command, commandArgs, options) {
  const result = childProcess.spawnSync(command, commandArgs, {
    stdio: "inherit",
    shell: false,
    ...options,
  });
  if (result.error) throw result.error;
  return result.status ?? 1;
}

function ensureDotnetTrace() {
  const probe = childProcess.spawnSync("dotnet-trace", ["--version"], { stdio: "ignore", shell: false });
  if (probe.status === 0) return;
  console.log("dotnet-trace not found; installing as a global dotnet tool...");
  if (run("dotnet", ["tool", "install", "-g", "dotnet-trace"]) !== 0) {
    throw new Error("Failed to install dotnet-trace.");
  }
}

async function ensurePerfView() {
  if (fs.existsSync(perfViewExe)) return;
  console.log(`Downloading PerfView to ${perfViewExe} ...`);
  const response = await fetch(perfViewUrl, { redirect: "follow" });
  if (!response.ok) {
    throw new Error(`Failed to download PerfView: HTTP ${response.status}`);
  }
  const buffer = Buffer.from(await response.arrayBuffer());
  fs.writeFileSync(perfViewExe, buffer);
}

async function main() {
  const argv = process.argv.slice(2);
  if (argv.length === 0 || argv[0] === "--help" || argv[0] === "-h") {
    printUsage();
    process.exit(argv[0] ? 0 : 1);
  }

  if (process.platform !== "win32") {
    console.error("This script is Windows-only (PerfView).");
    process.exit(1);
  }

  const scenarioName = normalizeScenarioName(argv[0]);
  let iterations = 2;
  let open = true;
  let noInline = false;
  for (let i = 1; i < argv.length; i++) {
    if (argv[i] === "--iterations" && argv[i + 1]) {
      iterations = Number.parseInt(argv[++i], 10);
    } else if (argv[i] === "--no-open") {
      open = false;
    } else if (argv[i] === "--no-inline") {
      noInline = true;
    } else {
      console.error(`Unknown option: ${argv[i]}`);
      printUsage();
      process.exit(1);
    }
  }

  if (!scenarioName || !Number.isInteger(iterations) || iterations < 1) {
    printUsage();
    process.exit(1);
  }

  const scenario = resolveScenario(scenarioName);
  if (!scenario) {
    console.error(`Scenario '${scenarioName}' not found.`);
    printUsage();
    process.exit(1);
  }

  const workDir = path.join(workRoot, scenarioName);
  const runnerDir = path.join(workDir, "runner");
  fs.mkdirSync(runnerDir, { recursive: true });

  const composedScript = path.join(workDir, `${scenarioName}.js`);
  fs.writeFileSync(composedScript, scenario.source);
  fs.writeFileSync(path.join(runnerDir, "runner.csproj"), runnerCsproj);
  fs.writeFileSync(path.join(runnerDir, "Program.cs"), runnerProgram);

  console.log(`Scenario:   ${scenarioName} (mode: ${scenario.mode}, iterations: ${iterations})`);
  console.log(`Work dir:   ${workDir}`);
  if (noInline) {
    console.log("Inlining:   disabled (DOTNET_JitNoInline=1) - call trees are complete, timings skew");
  }

  console.log("Building profiling runner (Release)...");
  if (run("dotnet", ["build", "-c", "Release", "--nologo", "-v", "q"], { cwd: runnerDir }) !== 0) {
    throw new Error("Runner build failed.");
  }

  ensureDotnetTrace();

  const runnerExe = path.join(runnerDir, "bin", "Release", "net10.0", "runner.exe");
  const timestamp = new Date().toISOString().replace(/[:.]/g, "-").slice(0, 19);
  const tracePath = path.join(workDir, `${scenarioName}-${timestamp}.nettrace`);

  console.log("Collecting trace (this runs the full scenario; may take a while)...");
  // Env vars set on dotnet-trace propagate to the child process it launches.
  const traceEnv = noInline ? { ...process.env, DOTNET_JitNoInline: "1" } : undefined;
  const traceStatus = run("dotnet-trace", [
    "collect",
    "-o", tracePath,
    "--",
    runnerExe, composedScript, scenario.mode, String(iterations),
  ], traceEnv ? { env: traceEnv } : undefined);
  if (traceStatus !== 0 || !fs.existsSync(tracePath)) {
    throw new Error("Trace collection failed.");
  }
  console.log(`Trace:      ${tracePath}`);

  if (open) {
    await ensurePerfView();
    console.log("Opening trace in PerfView...");
    childProcess.spawn(perfViewExe, [tracePath], { detached: true, stdio: "ignore" }).unref();
  }
}

main().catch((error) => {
  console.error(error.message ?? error);
  process.exit(1);
});
