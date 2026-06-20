import { spawnSync } from "node:child_process";
import { mkdirSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url));
const benchmarkDirectory = path.resolve(scriptDirectory, "..");

function parseArgs(argv) {
  const parsed = {
    bench: "string-width",
    runtimes: [],
    output: "results.json",
  };
  const positional = [];

  for (let i = 0; i < argv.length; i++) {
    const token = argv[i];
    if (token === "--bench") {
      parsed.bench = argv[++i] ?? parsed.bench;
      continue;
    }
    if (token === "--runtime") {
      parsed.runtimes.push(argv[++i] ?? "");
      continue;
    }
    if (token === "--output") {
      parsed.output = argv[++i] ?? parsed.output;
      continue;
    }
    if (token.startsWith("--")) {
      throw new Error(`Unknown argument: ${token}`);
    }

    positional.push(token);
  }

  if (positional.length > 3) {
    throw new Error(`Unknown argument: ${positional[3]}`);
  }

  if (positional[0] && parsed.bench === "string-width") {
    parsed.bench = positional[0];
  }
  if (positional[1] && parsed.runtimes.length === 0) {
    parsed.runtimes.push(positional[1]);
  }
  if (positional[2] && parsed.output === "results.json") {
    parsed.output = positional[2];
  }

  return parsed;
}

function extractRunnerJson(stdout, stderr) {
  const lines = `${stdout ?? ""}\n${stderr ?? ""}`
    .split(/\r?\n/)
    .map(line => line.trim())
    .filter(line => line.length > 0);

  for (let i = lines.length - 1; i >= 0; i--) {
    const candidate = lines[i];
    if (!candidate.startsWith("{") || !candidate.includes("\"benchmarks\"")) {
      continue;
    }

    try {
      return JSON.parse(candidate);
    } catch {
      // Keep scanning if there are non-JSON lines that start with '{'.
    }
  }

  throw new Error("Could not find mitata JSON output in command logs.");
}

function runNpmScript(scriptName, runtimeLabel) {
  console.log(`Running ${scriptName} (${runtimeLabel})...`);
  const isWindows = process.platform === "win32";
  const command = isWindows ? (process.env.ComSpec || "cmd.exe") : "npm";
  const commandArgs = isWindows
    ? ["/d", "/s", "/c", `npm run ${scriptName}`]
    : ["run", scriptName];

  const env = {
    ...process.env,
    BENCHMARK_RUNNER: "1",
    JROC_SIMPLE_BENCH_RUNNER: "1",
    BENCHMARK_RUNTIME: runtimeLabel,
  };

  if (
    runtimeLabel === "jroc" &&
    !env.JROC &&
    String(env.USE_PUBLISHED_JROC_TOOL || "").toLowerCase() === "true"
  ) {
    // In release mode, prefer the globally installed jroc tool.
    env.JROC = "jroc";
  }

  const result = spawnSync(command, commandArgs, {
    cwd: benchmarkDirectory,
    env,
    encoding: "utf8",
    stdio: "pipe",
  });

  if (result.stdout) {
    process.stdout.write(result.stdout);
  }
  if (result.stderr) {
    process.stderr.write(result.stderr);
  }

  if (result.error) {
    throw result.error;
  }

  if (result.status !== 0) {
    throw new Error(`npm run ${scriptName} failed with exit code ${result.status ?? "unknown"}.`);
  }

  const payload = extractRunnerJson(result.stdout, result.stderr);
  payload.runtime = runtimeLabel;
  payload.script = scriptName;
  return payload;
}

function resolveScriptName(benchName, runtimeLabel) {
  if (runtimeLabel === "node") {
    return `bench:${benchName}`;
  }
  if (runtimeLabel === "jroc") {
    return `bench:${benchName}:jroc`;
  }

  throw new Error(`Unsupported runtime '${runtimeLabel}'. Supported runtimes: node, jroc.`);
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  const runtimes = args.runtimes.length > 0 ? args.runtimes : ["node", "jroc"];
  const outputPath = path.resolve(benchmarkDirectory, args.output);
  const runAt = new Date().toISOString();

  const runs = runtimes.map(runtimeLabel => {
    const scriptName = resolveScriptName(args.bench, runtimeLabel);
    return runNpmScript(scriptName, runtimeLabel);
  });

  const payload = {
    timestamp: runAt,
    benchmark: args.bench,
    runs,
  };

  mkdirSync(path.dirname(outputPath), { recursive: true });
  writeFileSync(outputPath, `${JSON.stringify(payload, null, 2)}\n`, "utf8");
  console.log(`Mitata results written to ${outputPath}`);
}

main();
