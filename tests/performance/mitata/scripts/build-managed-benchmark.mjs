import { build } from "esbuild";
import { mkdirSync, readFileSync, writeFileSync } from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url));
const benchmarkDirectory = path.resolve(scriptDirectory, "..");
const benchmarkName = process.argv[2] ?? "string-width";
const inputPath = path.join(benchmarkDirectory, "snippets", `${benchmarkName}.mjs`);
const outputDirectory = path.join(benchmarkDirectory, ".managed");
const outputPath = path.join(outputDirectory, `${benchmarkName}.js`);
const modulePath = path.join(outputDirectory, `${benchmarkName}.mjs`);

let source = readFileSync(inputPath, "utf8")
  .replace(/from\s+["']\.\.\/runner\.mjs["']/g, 'from "../runner-simple.mjs"')
  .replace("await run();", "void run();");

if (benchmarkName === "string-width") {
  const upstreamSetup =
    "const maxInputLength = Math.max(...inputs.map(([input]) => input.repeat(Math.max(...repeatCounts)).length));";
  const managedSetup = `let maxInputLength = 0;
for (const [input] of inputs) {
  const repeatedInputLength = input.repeat(5000).length;
  if (repeatedInputLength > maxInputLength) {
    maxInputLength = repeatedInputLength;
  }
}`;

  if (!source.includes(upstreamSetup)) {
    throw new Error("Could not find the string-width maxInputLength setup to rewrite.");
  }

  source = source.replace(upstreamSetup, managedSetup);
}

mkdirSync(outputDirectory, { recursive: true });
writeFileSync(modulePath, source, "utf8");

await build({
  stdin: {
    contents: source,
    resolveDir: path.dirname(inputPath),
    sourcefile: path.basename(inputPath),
    loader: "js",
  },
  bundle: true,
  format: "iife",
  platform: "neutral",
  target: "es2022",
  outfile: outputPath,
});

console.log(outputPath);
