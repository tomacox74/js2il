# Real-world canary corpus

This corpus complements the differential corpus with bounded smoke cases that validate committed, user-facing outputs.

## Suites

| Suite | Purpose | Entry point |
| --- | --- | --- |
| `pr` | Small PR gate with fast ecosystem/tooling canaries | `npm run diff:test:canary` |
| `expanded` | Additional nightly coverage layered on top of the PR gate | `npm run diff:test:canary:nightly` |

## Cases

| Suite | Case | Why it matters |
| --- | --- | --- |
| `pr` | `dromaeo-object-array-modern` | Exercises a real benchmark script with modern array-heavy object/loop behavior. |
| `pr` | `dromaeo-object-regexp` | Exercises a real benchmark script with regexp-heavy string processing. |
| `expanded` | `array-stress` | Exercises a bounded array stress benchmark with repeated append/index patterns. |
| `expanded` | `stopwatch-modern` | Exercises the constructor-function stopwatch benchmark end-to-end. |
| `expanded` | `dromaeo-object-string-modern` | Broadens nightly coverage with real benchmark string/object manipulation hot paths. |
| `expanded` | `dromaeo-3d-cube-modern` | Exercises math-heavy object allocation and numeric update flows in a bounded loop. |
| `expanded` | `dromaeo-core-eval-modern` | Covers a representative eval-driven benchmark path without widening the fast PR gate. |
| `expanded` | `dromaeo-object-regexp-modern` | Adds the modern regexp-heavy benchmark variant alongside the legacy PR-gate sample. |

## Adding a new canary

1. Add a `.js` entry script to the appropriate suite directory.
2. Add a sibling `.expected.txt` file with the exact expected stdout.
3. Keep the script bounded and deterministic so it is safe for CI.
4. Validate locally with the matching `npm run diff:test:canary*` command.
