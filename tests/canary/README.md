# Canary Smoke Tests

Real-world scripts that compile, execute, and produce deterministic output – used as
an early-warning system for ecosystem-level breakage.

## How it works

Each canary goes through three phases:

1. **Compile** – `js2il <script.js> -o <outdir>`
2. **Execute** – `dotnet <outdir>/<name>.dll` with a strict wall-clock timeout
3. **Validate** – stdout must contain a `CANARY:<name>:ok` marker

Any failure causes the run to exit non-zero and preserves artefacts for investigation.

## Running locally

```powershell
# Run a single canary (auto-discovers built Js2IL.dll):
pwsh scripts/run-canary.ps1 -CanaryScript tests/canary/corpus/hello-world.js

# Run with an explicit path to the compiled js2il tool:
pwsh scripts/run-canary.ps1 \
  -CanaryScript tests/canary/corpus/closures.js \
  -Js2ILPath Js2IL/bin/Release/net10.0/Js2IL.dll

# Keep artefacts after a successful run (useful for debugging):
pwsh scripts/run-canary.ps1 \
  -CanaryScript tests/canary/corpus/prime-mini.js \
  -KeepArtifacts
```

You can also run all canaries sequentially with a simple loop:

```powershell
foreach ($s in (Get-ChildItem tests/canary/corpus/*.js)) {
  pwsh scripts/run-canary.ps1 -CanaryScript $s.FullName
}
```

## CI integration

| Workflow | Trigger | Scripts |
|---|---|---|
| `canary-pr` | push / pull_request → master | hello-world, closures, classes (fast set) |
| `canary-nightly` | daily at 02:00 UTC + manual | all scripts in corpus/ |

## Corpus

| Script | Features exercised |
|---|---|
| `hello-world.js` | Basic function calls, string concatenation |
| `closures.js` | Closure semantics, mutable captured variables |
| `classes.js` | ES6 class declaration, inheritance, `instanceof` |
| `array-methods.js` | `Array.map`, `filter`, `reduce`, `join` |
| `prime-mini.js` | Algorithmic loops, arrays, deterministic sieve |

## Adding a new canary

1. Create `tests/canary/corpus/<name>.js` (must use `'use strict';`).
2. Make sure the script prints `CANARY:<name>:ok` to stdout on success.
3. Verify it runs correctly under Node.js first: `node tests/canary/corpus/<name>.js`.
4. Verify it compiles and runs with js2il:
   ```powershell
   pwsh scripts/run-canary.ps1 -CanaryScript tests/canary/corpus/<name>.js
   ```
5. Add a step to `.github/workflows/canary-nightly.yml` (and optionally `canary-pr.yml` for fast canaries).

### Canary script checklist

- [ ] `'use strict';` prologue
- [ ] Prints `CANARY:<name>:ok` on the last line (deterministic)
- [ ] No external module dependencies (unless the nightly set explicitly handles them)
- [ ] Exits with code 0 on success
- [ ] Runs in < 5 seconds on a typical CI machine
