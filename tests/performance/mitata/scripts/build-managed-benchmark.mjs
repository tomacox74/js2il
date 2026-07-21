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

const source = readFileSync(inputPath, "utf8")
  .replace(/from\s+["']\.\.\/runner\.mjs["']/g, 'from "../runner-simple.mjs"')
  .replace("await run();", "void run();");

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
