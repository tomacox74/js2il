## PR: Class Method Scope Generation & Validator Updates

### Summary
Implements robust method‑scope handling for ES class methods and modernizes validation:
* Generates scope types for all scopes (including empty class methods) so complex bodies (loops, nested blocks, lexical bindings) no longer throw missing scope errors.
* Heuristic `ShouldCreateMethodScopeInstance` decides when to materialize a method scope local (loops, let/const, nested functions, for/of, etc.).
* Replaces ad‑hoc local allocation with `CreateScopeInstance` (maps to local 0) ensuring consistent variable resolution.
* Removes obsolete experimental warnings for classes & arrow functions.
* Adds validator error for unsupported `require("module")` calls by reflecting over runtime node modules (via `NodeModuleAttribute`).

### Key Changes
| Area | Change |
|------|--------|
| IL Gen | `ClassesGenerator` now calls heuristic and passes flag to `ILMethodGenerator` |
| IL Gen | `ILMethodGenerator` uses `CreateScopeInstance` for function/method scope (local 0) |
| Types  | `TypeGenerator` registers every scope type (even with zero vars) in `VariableRegistry` |
| Vars   | `VariableRegistry` tracks `_allScopeTypes` for empty scopes & exposes `EnsureScopeType` |
| Validation | Removed class/arrow experimental warnings; added dynamic module support detection & unsupported module error |
| Validation | Reflection over `JavaScriptRuntime` assembly to collect supported modules automatically |
| Tests | Added execution + generator tests for: method calling method, method with for loop calling method |
| Tests | Added generator snapshots for two‑param Add/Subtract constructor methods |
| Tests | Added validator tests for supported / unsupported `require()` |

### Added / Updated Tests
* Classes Execution: `Classes_ClassMethod_CallsAnotherMethod`, `Classes_ClassMethod_ForLoop_CallsAnotherMethod`.
* Classes Generator: counterparts for the above plus Add/Subtract constructor snapshots.
* Validator: multi‑issue test updated; added supported/unsupported `require` tests.

### Validation Behavior Changes
* Class declarations: now fully allowed (no warning/error).
* Arrow functions: allowed (no warning).
* `require()` validation: errors only when module name not among reflected `NodeModuleAttribute` types.

### Rationale
Previous runtime failure: complex class methods (loops + method calls) triggered "Scope '<name>' not found" because method scopes without locals were not materialized. Ensuring all scope types exist and conditionally instantiating a scope local resolves this while avoiding unnecessary locals (prevents `InvalidProgramException`).

### Risk & Mitigation
* Heuristic false positives: only cost is an extra (benign) scope object. No functional regression observed in added tests.
* Reflection failure (e.g., trimming): guarded by try/catch; fallback to empty set yields explicit unsupported error—failing fast.

### Follow Ups (Not in this PR)
* Extend heuristic or make configurable.
* Add snapshot formatter to reduce diff verbosity.
* Broaden supported Node module list as intrinsic implementations land.

### Manual Verification
Targeted validator test run and class execution/generator tests pass (see local run logs). Full suite recommended before merge.

### Checklist
- [x] Method scope generation heuristic
- [x] Variable registry updates for empty scopes
- [x] Remove experimental warnings
- [x] Reflection‑based module support in validator
- [x] New execution & generator tests
- [x] Validator tests for `require()`

### Merge Notes
No public API breaking changes expected; consumer behavior improves (classes & arrow functions no longer produce warnings).

---
Please review focusing on:
1. Heuristic correctness (`ShouldCreateMethodScopeInstance`).
2. Validator reflection safety / performance.
3. Snapshot diffs (IL) for unintended instruction changes.

Thanks.