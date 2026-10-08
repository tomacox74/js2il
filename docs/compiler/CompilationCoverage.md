# Compilation-mode coverage

Compilation-mode coverage measures the compiler's static implementation choices,
independently of JavaScript correctness. It is not executed-code coverage, an
execution profile, a performance prediction, or a percentage of emitted IL bytes.

## CLI

```shell
jroc coverage input.js
jroc coverage input.js --json
jroc coverage input.js --coverage-report coverage.json
jroc input.js out --coverage-report coverage.json
```

`coverage` (also available as `--coverage`) compiles in memory without loading,
executing, or persisting the resulting assembly. It accepts additional inputs
with `-a`, module IDs, and the usual compilation settings. An output assembly
directory is not accepted in analysis-only mode. `--json` writes one JSON report
to stdout; diagnostics remain on stderr and verbose/unused-analysis stdout
diagnostics cannot be combined with it. `--coverage-report` writes a JSON file,
including during normal
compilation. Rejected compilation produces a partial report and a nonzero exit
status. Report-writing failures also return a nonzero status.

Text reports include category counts, percentages, and the file, one-based
line/column, and reason for every non-direct site.

## Measurement and classification

Schema version **1** uses **`statement-source-sites-v1`**:

- Each unique, non-hidden statement sequence-point span observed in final
  normalized LIR counts once per compilation. Dependencies, callable bodies,
  and independent entry modules are included.
- Instructions inherit the most recent non-hidden sequence point. Multiple
  lowering operations or repeated compilations of the same span do not add
  sites. Labels, sequence-point markers, and instructions without an active
  source span do not count. Synthetic tail operations can share their preceding
  statement's span; this is not expression-perfect attribution.
- Known unsupported validation uses the offending AST diagnostic span instead
  (for example the `eval` identifier). A lowering failure can identify an entire
  callable. Ordinary parser/early errors are diagnostics, not evidence that a
  valid JavaScript feature is unsupported.
- Each site receives its **worst observed mode**, in the order
  `directIl < runtimeIntrinsic < runtimeDispatch < unsupported`. Only reasons for
  that mode are retained. Guarded specializations with generic fallbacks count
  as dynamic even if the fast path would usually execute.
- Counts sum to the number of observed sites; each percentage divides its count
  by that total. Empty reports have zero percentages. Failed compilations are
  partial: uncompiled source is never invented as direct or unsupported.

| JSON mode | Meaning |
|---|---|
| `directIl` | Explicitly classified IL-level arithmetic, constants, control flow, scope/argument access, boxing, and statically bound generated callable operations. |
| `runtimeIntrinsic` | Explicitly classified typed operations, fixed runtime/BCL/intrinsic targets, and runtime support such as allocation, scope construction, or TDZ checks. |
| `runtimeDispatch` | Generic JavaScript operators/coercion, runtime-selected property/call/constructor targets, guarded fallback paths, import/require, generic runtime services, and unclassified operations. |
| `unsupported` | A compiler-observed unsupported feature or lowering failure with a source span and reason. |

The classifier describes **LIR semantic operations**, not every scaffolding
instruction inside an IL emitter or runtime helper. A direct generated call can
still need invocation-frame/closure ABI support; a bound intrinsic can internally
perform JavaScript coercion or invoke callbacks. Neither category promises
allocation-free execution. All classification changes that affect comparability
must update the measurement version; compare reports only with matching
measurement, compiler provenance, options, and runner/source scope.

The collector is compilation-scoped and opt-in. Disabled compilation does not
instantiate it, scan instructions, or allocate site/reason collections. Existing
sequence points are reused without enabling PDBs or changing emitted IL.

## JSON and SDK

The compiler report contains `schemaVersion`, `measurement`,
`compilationSucceeded`, `complete`, four-category `counts` (plus `total`),
`percentages`, `sites`, and `diagnostics`. Sites include `file`, `line`, `column`,
`endLine`, `endColumn`, `mode`, and sorted `reasons`. Counts and sites are
deterministically ordered; paths are the compiler's actual source identities.

```csharp
var analysis = JrocInMemoryCompiler.AnalyzeCompilationCoverage(request);
Console.WriteLine(analysis.Report.ToText());
File.WriteAllText("coverage.json", analysis.Report.ToJson());
if (analysis.Artifact is null)
{
    // Recognized compilation rejection; inspect the partial report.
}
```

Both single-entry and multi-entry analysis overloads compile once. Unexpected
compiler bugs still propagate. Alternatively, set `CollectCompilationCoverage`
on a compile request and read `artifact.CompilationCoverage`. Ordinary `Compile`
retains its exception-on-failure contract.

## Test262 and retained evidence

```shell
node scripts/test262/runMvp.js --suite pr --compilation-coverage --output artifacts/test262/pr
python3 -m scripts.test262.compilationCoverage --summary artifacts/test262/pr/summary.json \
  --output artifacts/test262/pr/compilation-coverage.json
```

The MVP runner embeds each report in `summary.json` and records compiler binary
hashes. The native screening host accepts `"compilation_coverage": true` in its
plan and embeds the report from the **same** native-harness compilation, including
recognized compilation failures. Reports are omitted when collection is disabled
or the fixture cannot reach compilation.

Native source locations refer to prepared fixture source, including injected
strict directives, and use native virtual-file identities. MVP locations refer
to composite source containing JavaScript harness files and strict preparation.
Keep these scopes separate; MVP harness operations can dominate its totals.
Fixture paths and strict/non-strict/module variants remain separate report
identities rather than being inferred from compiler source paths.

Central preparation accepts `--compilation-coverage`, records it in compiler
provenance, and propagates it through the existing networkless fixture boundary.
No database schema, authority contract, or credential allowlist changes are
needed. Aggregation reads retained acknowledged **and** unacknowledged outbox
payloads without database credentials:

```shell
python3 -m scripts.test262.compilationCoverage --outbox artifacts/test262/central-outbox.sqlite \
  --context artifacts/test262/central-context.json \
  --output artifacts/test262/central-compilation-coverage.json
```

Aggregates keep correctness outcome counts separate from
`passingCompilationModes`. Only passing fixture variants with complete,
successful compilation reports contribute source-site counts; runtime assertion
failures, partial reports, matched parse rejections, skipped tests, and harness
gaps retain their individual evidence but do not inflate that denominator.
`missingReports` is explicit, not treated as direct IL or unsupported syntax.
Percentages are calculated from summed site counts, not averaged test percentages.

Outbox aggregation validates payload digests and exact run/provenance/scope,
deduplicates replayed observation IDs, and selects the latest lease generation
per fixture variant (then finish time and observation ID). It does not select the
best passing retry. Incompatible report schemas, measurement units, counts, or
percentages fail explicitly. `observedAttempts` retains the retry population.
This artifact is local run evidence, not a replacement for authoritative
Test262 conformance or invalidation-aware central catalogue queries.

PR and nightly MVP CI collect bounded coverage and retain separate JSON artifacts
for 90 days. The central workflow provides an opt-in `compilation_coverage`
input and retains both the aggregate and the existing recovery outbox for 90 days.
Historical dashboards and regression thresholds are deliberately not imposed.
