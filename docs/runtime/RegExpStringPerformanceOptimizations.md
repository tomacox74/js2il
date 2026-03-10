# RegExp and String Hot-Path Performance Optimizations

This document summarizes the runtime performance work shipped in PR #834 for issue #833.
The goal was to recover the severe `dromaeo-object-regexp` regression that appeared after the broader RegExp compatibility work in `v0.8.29`.

## Regression summary

- User-reported phased benchmark regression:
  - `v0.8.28`: about `78.5 ms`
  - `v0.8.29`: about `401 ms`
- Local reproduction before the fix on the issue #833 branch:
  - non-modern `js2il execute (pre-compiled)`: about `671.7 ms`
  - non-modern `js2il compile`: about `347.3 ms`

The first investigation pointed at the new generic well-known-symbol dispatch used by:

- `String.prototype.match`
- `String.prototype.search`
- `String.prototype.replace`
- `String.prototype.split`

That was real overhead, but deeper profiling showed that the non-modern benchmark was ultimately dominated by `split(/(?:)/)` result construction.

## Root cause

The regression came from the interaction of two costs:

1. **Generic RegExp symbol dispatch became hot.**
   Every `match/search/replace/split` call had to perform symbol-key conversion, property lookup, and generic invocation even for ordinary built-in `RegExp` receivers.
2. **Empty-RegExp split was much more expensive than it needed to be.**
   The `dromaeo-object-regexp` scenario spends a large amount of time in `tmp[i].split(/(?:)/)`, and the generic path paid unnecessary regex/runtime allocation costs for a case with stable JavaScript semantics.

## Optimizations added in this PR

### 1. Cheaper well-known symbol lookup and invocation

The first step was reducing generic dispatch overhead in the common case:

- `JavaScriptRuntime.Symbol` now caches `DebugId`
- `JavaScriptRuntime.String` caches property-key strings for:
  - `Symbol.match`
  - `Symbol.search`
  - `Symbol.replace`
  - `Symbol.split`
- `TryInvokeWellKnownSymbol(...)` now uses arity-specific fast paths instead of always paying a more generic invocation cost

This cut a meaningful amount of overhead before deeper hotspot work started.

### 2. Intrinsic RegExp dispatch with explicit invalidation

Built-in `JavaScriptRuntime.RegExp` receivers now use an intrinsic path first, but only while it is safe to do so.

Key pieces:

- `JavaScriptRuntime.RegExp` exposes intrinsic `@@match`, `@@search`, `@@replace`, and `@@split` delegates
- `JavaScriptRuntime.String` tries those intrinsic delegates before falling back to generic symbol dispatch
- The runtime invalidates those fast paths when user code can observably override behavior through:
  - instance symbol assignment
  - prototype symbol assignment
  - `Object.defineProperty(...)`
  - `Object.setPrototypeOf(...)`
  - `__proto__`
  - RegExp prototype observation paths

This preserves JavaScript override semantics while keeping the built-in path lean.

### 3. Shared RegExp prototype surface

The PR moved intrinsic well-known-symbol methods from per-instance setup to a shared static `RegExp.Prototype`.

Benefits:

- avoids defining the same symbol methods on every `RegExp` instance
- makes the runtime surface more consistent with other intrinsic types such as `Array`
- centralizes prototype invalidation logic for RegExp symbol overrides

### 4. Lower-allocation match and search helpers

Several supporting optimizations reduce object creation on common RegExp flows:

- `Regex.EnumerateMatches(...)` / match-bounds helpers for cheaper index/length-based operations
- conservative simple-literal RegExp detection so common literal cases can use string operations directly
- cached prepared `Regex` instances keyed by pattern and options

These were secondary wins compared to the split fix, but they improved the broader runtime path and helped the benchmark before the final hotspot was addressed.

### 5. JavaScript-correct empty-RegExp split fast path

The biggest remaining win came from specializing empty-RegExp split.

`JavaScriptRuntime.String.SplitWithRegExp(...)` now routes empty patterns through a dedicated JavaScript-aware implementation instead of the generic regex split path.

The new path preserves the important semantics that matter for JavaScript compatibility:

- no extra leading or trailing empty strings
- `limit` is honored
- default behavior splits by UTF-16 code unit
- `/u` respects surrogate pairs when advancing the split index

It also reduces per-element overhead by:

- using pre-sized dense result storage
- avoiding unnecessary regex engine work
- caching Latin-1 single-character strings for the hottest split outputs

### 6. Correctness fixes required by the performance work

The performance changes exposed a broader correctness gap:

- `JavaScriptRuntime.Object.SetProperty(...)` could previously fail to persist new own properties on some CLR-backed runtime objects
- that broke cases such as `re[Symbol.match] = ...`, which must create an observable own property

This PR fixes that fallback and adds focused coverage so the performance fast paths remain semantics-safe.

## Benchmark impact

### Local phased benchmark

| Measurement | Before | After |
| --- | ---: | ---: |
| non-modern `js2il execute (pre-compiled)` | `671.7 ms` | `239.6 ms` |
| non-modern `js2il compile` | `347.3 ms` | `248.7 ms` |

### Targeted hotspot probe

| Measurement | Before | After |
| --- | ---: | ---: |
| `split(/(?:)/)` probe | about `571 ms` | about `252 ms` |

The final local execute-lane result comfortably beats the user-reported regressed value of about `401 ms`.

## Validation added in this PR

Focused behavior coverage was added for:

- RegExp instance well-known-symbol overrides
- RegExp prototype well-known-symbol overrides
- empty-RegExp split semantics, including `/u` surrogate-pair behavior

Validation commands used during the work:

```powershell
dotnet test .\Js2IL.Tests\Js2IL.Tests.csproj -c Release --filter "FullyQualifiedName~Js2IL.Tests.String.ExecutionTests|FullyQualifiedName~Js2IL.Tests.String.GeneratorTests|FullyQualifiedName~Js2IL.Tests.IntrinsicCallables.ExecutionTests.IntrinsicCallables_RegExp|FullyQualifiedName~Js2IL.Tests.IntrinsicCallables.GeneratorTests.IntrinsicCallables_RegExp" --nologo
```

```powershell
node .\scripts\runPhasedBenchmarkScenario.js dromaeo-object-regexp
```

## Key files

- `JavaScriptRuntime\String.cs`
- `JavaScriptRuntime\RegExp.cs`
- `JavaScriptRuntime\Object.cs`
- `JavaScriptRuntime\Symbol.cs`
- `Js2IL.Tests\String\JavaScript\String_RegExp_SymbolDispatch_RegExpOverride.js`
- `Js2IL.Tests\String\JavaScript\String_RegExp_SymbolDispatch_RegExpPrototypeOverride.js`
- `Js2IL.Tests\String\JavaScript\String_Split_Regex_Empty.js`

## Takeaways

- Generic well-known-symbol dispatch can be a major cost in RegExp-heavy benchmark scenarios.
- Empty-RegExp split is worth treating as a dedicated runtime path because its semantics are stable and its hot-path cost is unusually high.
- In JS2IL, performance fast paths must always be paired with explicit invalidation and tests for instance/prototype mutation so JavaScript behavior stays correct.
