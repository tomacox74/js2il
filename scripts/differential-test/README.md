# Bounded Differential Test Harness

Detects semantic regressions by running every JS program under **both Node.js
and JS2IL** and comparing the observable output (stdout, exit code).  Any
mismatch is a potential compiler bug.

## Quick start

```sh
# Run fixed corpus only (fast, suitable for CI)
npm run diff:test

# Run corpus + 50 generated programs (weekly-style)
npm run diff:test:generate

# Custom options
node scripts/differential-test/run.js \
  --corpus  scripts/differential-test/corpus \
  --timeout 15 \
  --generate 100 \
  --seed 1234 \
  --verbose
```

## Options

| Flag | Default | Description |
|------|---------|-------------|
| `--corpus <dir>` | `./corpus` | Directory of `.js` seed programs |
| `--timeout <secs>` | `10` | Per-execution wall-clock limit |
| `--compile-timeout <secs>` | `2× --timeout` | Compilation wall-clock limit |
| `--generate <n>` | `0` | Also run `n` generated programs |
| `--seed <n>` | `42` | RNG seed for generated programs (deterministic) |
| `--output <dir>` | OS temp | Scratch directory for compiled DLLs |
| `--js2il <path>` | auto-detected | Path to `Js2IL.dll` or `js2il` executable |
| `--verbose` | off | Print full diff for every test (including passes) |

## Corpus

| File | Risk area |
|------|-----------|
| `cf-ternary.js` | Control-flow joins via `?:` |
| `cf-logical.js` | `&&` / `\|\|` value semantics & short-circuit |
| `loop-for.js` | `for`-loop back-edges and accumulator patterns |
| `loop-while.js` | `while`-loop with conditional variable updates |
| `numeric-int.js` | Integer `+ - * / % **` |
| `numeric-float.js` | Float / NaN / Infinity / boxing boundary |
| `array-index.js` | `Array.length` and index arithmetic |
| `array-mixed.js` | `map / filter / reduce / indexOf` |

## Generated programs

`generate.js` produces deterministic JS programs from five templates that
cover the same four risk areas.  Pass `--generate N` to add N programs on top
of the corpus.  The same `--seed` value always yields the same programs.

```sh
# Write 20 programs to /tmp/gen without running them
node scripts/differential-test/generate.js --seed 7 --count 20 --output /tmp/gen
```

## CI integration

`differential.yml` wires this harness into GitHub Actions:

* **PR gate** – runs the fixed corpus (fast, ~1 min).
* **Weekly** – runs corpus + 50 generated programs (scheduled Monday at 02:00 UTC).
* **Manual** – `workflow_dispatch` accepts custom `seed` / `generate` inputs.
* If the scheduled weekly run fails, the workflow opens a GitHub issue automatically.

## Output normalisation

Stdout lines are trimmed of trailing whitespace and compared verbatim.  When
both processes exit non-zero the JS-level error type+message is extracted and
compared so that Node's V8 stack trace and the .NET runtime trace do not
produce spurious mismatches.

## Reproducing a failure

Every mismatch is printed with the **file path** (corpus) or **full program
source** path (generated) so you can reproduce it locally:

```sh
# Re-run a single failing program
node   scripts/differential-test/corpus/loop-for.js
dotnet Js2IL/bin/Release/net10.0/Js2IL.dll scripts/differential-test/corpus/loop-for.js -o /tmp/out
dotnet /tmp/out/loop-for.dll
```
