---
name: investigating-benchmark-performance
description: Investigate JROC benchmark performance with local artifacts, GitHub workflow provenance, and Supabase perf_results data.
tier: standard
applyTo: 'tests/performance/**,.github/workflows/benchmarkdotnet-suite.yml,.github/workflows/mitata-suite.yml,.github/workflows/performance-comparison.yml'
---

# Investigating Benchmark Performance

Use this skill when investigating a performance regression, improvement, or
cross-runtime comparison. Start with the generated IL and benchmark artifact
for the exact scenario before drawing conclusions from aggregate results.

## Data Sources

GitHub Actions ingests benchmark results into the Supabase `perf_results`
table. The shared ingestion implementation is
`tests/performance/ingestPerfToSupabase.js`.

| Workflow | Source | Input |
| --- | --- | --- |
| `.github/workflows/benchmarkdotnet-suite.yml` | `benchmarkdotnet` | `tests/performance/Benchmarks/BenchmarkDotNet.Artifacts/results/*.json` |
| `.github/workflows/mitata-suite.yml` | `mitata` | `tests/performance/mitata/results.json` |
| `.github/workflows/performance-comparison.yml` | `prime-script` | `tests/performance/results.json` |

The BenchmarkDotNet and Mitata workflows run for published releases and can
also be dispatched manually. The Prime comparison workflow is manually
dispatchable. Workflow ingestion is best-effort (`continue-on-error`), so
verify that a workflow produced its artifact before treating a missing table
row as a benchmark failure.

Download the exact raw artifact before relying on ingested rows:

```bash
gh run download <run-id> \
  --repo tomacox74/js2il \
  --name <artifact-name> \
  --dir <temporary-directory>
```

For BenchmarkDotNet, inspect `HostEnvironmentInfo` and each matching row's
`Method`, `Parameters`, `Statistics` (including `N`), `Memory`, and `Metrics`.
Do not reconstruct sample count or runtime identity from the Supabase row when
the raw report is available.

## Local Supabase Access

Development desktops can use these environment variables for read-only
analysis:

```bash
export SUPABASE_URL="https://<project>.supabase.co"
export SUPABASE_PUBLISHABLE_KEY="<publishable-key>"
```

Neither Bash nor Copilot automatically loads a project `.env` file. If a
developer stores these values in an ignored local `.env`, they must load it
explicitly in a trusted shell before running analysis commands.

Never print either value, commit it, or place it in generated reports. Do not
use the CI-only `SUPABASE_SERVICE_ROLE_KEY` for desktop analysis. The workflow
ingester requires the service role key because it upserts rows; the publishable
key is appropriate only when the database's row-level security policy permits
the desired reads.

Example query for recent rows:

```bash
curl --fail-with-body --silent --show-error \
  "$SUPABASE_URL/rest/v1/perf_results?select=run_at,source,scenario,runtime,metric,value,unit,branch,sha&order=run_at.desc&limit=100" \
  -H "apikey: $SUPABASE_PUBLISHABLE_KEY" \
  -H "Authorization: Bearer $SUPABASE_PUBLISHABLE_KEY" |
  jq .
```

Filter at the database whenever possible. For example, compare the latest
`mean_ns` rows for a scenario:

```bash
curl --fail-with-body --silent --show-error \
  "$SUPABASE_URL/rest/v1/perf_results?source=eq.benchmarkdotnet&scenario=eq.prime-javascript&metric=eq.mean_ns&order=run_at.desc&limit=100" \
  -H "apikey: $SUPABASE_PUBLISHABLE_KEY" \
  -H "Authorization: Bearer $SUPABASE_PUBLISHABLE_KEY" |
  jq .
```

Use URL encoding for filter values that contain spaces or punctuation. When
reading a large result set, constrain `source`, `scenario`, `metric`, and a
recent time range; do not fetch the full table by default.

## `perf_results` Ingestion Schema

The checked-in ingester is the authoritative write contract. It writes one row
per metric, not one row per benchmark case. The conflict identity is:

```text
(run_id, run_attempt, source, scenario, runtime, metric)
```

Core columns written by every row:

| Column | Value |
| --- | --- |
| `run_id` | GitHub Actions run ID, or a local timestamp-derived ID |
| `run_attempt` | GitHub run attempt; `1` locally |
| `workflow` | GitHub workflow name, or `local` |
| `repo` | `owner/repository`, or `local/local` |
| `branch` | Git ref name; may be null |
| `sha` | Commit SHA, or `local` |
| `source` | `benchmarkdotnet`, `mitata`, or `prime-script` |
| `scenario` | Lowercase slug for the benchmark/script scenario |
| `runtime` | Normalized runtime identity |
| `runtime_version` | Version matched to the normalized runtime when known; may be null |
| `metric` | Measurement name |
| `value` | Numeric measurement |
| `unit` | Unit for `value` |
| `run_at` | Source timestamp or ingestion timestamp |
| `meta` | Source-specific JSON metadata |

The current schema may also have these structured host columns:

```text
os_type, os_platform, os_release, os_version, os_arch,
cpu_model, cpu_logical_cores, total_memory_bytes,
runner_os, runner_arch, github_image_os, github_image_version
```

They are optional for backward compatibility: if the deployed table does not
yet have them, the ingester retries with these columns and `runtime_version`
removed, retaining the data in `meta`. Do not assume a structured host column
exists without checking the deployed schema or an actual returned row.

The table can have database-managed columns (for example an ID or creation
timestamp) that are not supplied by the ingester. Obtain exact SQL types,
constraints, indexes, and row-level-security policies from the Supabase
dashboard or the database migration/schema source; do not infer them from the
REST payload.

## Metric Catalog

| Source | Metrics |
| --- | --- |
| `benchmarkdotnet` | `mean_ns`, `median_ns`, `stddev_ns`, `allocated_bytes`, `allocated_native_memory_bytes`, `gen0_collections_per_1000_ops`, `gen1_collections_per_1000_ops`, `gen2_collections_per_1000_ops` |
| `mitata` | `avg_ns`, `total_ns`, `iterations`, `error` |
| `prime-script` | `passes`, `passes_per_second`, `compile_duration_ms` |

Interpret lower values as better for duration/allocation/GC metrics and higher
values as better for `passes_per_second`. Never compare values across different
metrics or units.

Runtime names are normalized by the ingester. Relevant examples include
`jroc`, `jroc-compile`, `jroc-execute`, `jroc-total`, `node`, `jint`,
`jint-prepare`, `jint-execute`, `jint-execute-prepared`, `clearscript`,
`yantrajs`, and `yantrajs-execute`.

Do not assume a runtime label from a C# method name or helper-script label.
For example, a method named `Jint_ExecutePrepared` can be ingested as
`jint-execute` when its BenchmarkDotNet description omits "prepared". Check
the raw artifact and `normalizeRuntime()` before constructing a filter.

Group comparable Supabase rows by at least:

```text
(run_id, run_attempt, source, scenario)
```

Rows for the same run can have slightly different `run_at` timestamps. Do not
group by timestamp equality. Compare commits only when source, scenario,
runtime, metric/unit, benchmark version, and host are compatible.

## Understand the Timed Boundary

An "execute-only" benchmark can still include substantial setup below the
benchmark method. Read the benchmark implementation before interpreting the
result. For example, a timed `JsEngine.LoadModule(...)` includes runtime/module
initialization, JavaScript function materialization, and any workload executed
at module load; it is not only steady-state generated JavaScript instructions.

When two equivalent fixtures diverge:

1. Preserve the end-to-end benchmark as the user-visible result.
2. Create a temporary controlled variant that removes only the workload while
   retaining declarations/module initialization.
3. Compare that definitions-only phase with the full phase.
4. If needed, create a load-once/workload-only variant as a second control.

Record exactly what changed in the control. Never present a modified-fixture
result as the original benchmark.

Use BenchmarkDotNet `MemoryDiagnoser` allocation for claims. In a controlled
runner, `GC.GetAllocatedBytesForCurrentThread()` is invalid when JROC executes
on a runtime thread; use `GC.GetTotalAllocatedBytes(precise: true)` only as a
phase-isolation diagnostic.

## Generated Code and Runtime Profiling

Whole-assembly IL counts are screening signals, not root-cause evidence. They
mix module initialization, cold callables, and hot code. Compare corresponding
executed methods and separately inspect module-main materialization:

- method IL/native size and instruction count;
- boxing/coercion and generic operator calls;
- fixed-arity argument arrays and closure adapters;
- scope/function/array/object construction;
- direct versus runtime-dispatched calls.

For arrow-heavy variants, inspect generated `Closure.BindArrow` sites and the
current `Closure.CreateBoundDelegate` implementation. Per-instance expression
tree compilation or dynamic-method creation can dominate repeated module loads
even when the arrow bodies themselves have better IL.

EventPipe/dotnet-trace reports include runtime, finalizer, and blocked
background threads. Treat full call paths and phase-specific differences as
evidence; do not interpret top-N percentages as hardware CPU percentages.

These environment switches are useful causal diagnostics, not benchmark
configurations:

```bash
COMPlus_JitNoInline=1
COMPlus_TieredCompilation=0
```

If a variant gap changes materially, investigate JIT/inlining/tiering, but
retain default-runtime BenchmarkDotNet results for performance claims.

## Dromaeo Scenario Matching

Inspect the actual fixture diff before assigning meaning to a filename suffix.
Confirm whether the difference is `var` versus `let`/`const`, declarations
versus arrows, constructors versus literals, or a real algorithm change.

Scenario names can be prefixes of other scenarios. `dromaeo-3d-cube` also
prefix-matches `dromaeo-3d-cube-modern`, so a wildcard BenchmarkDotNet filter
alone is not exact. Use the catalog's exact selector:

```bash
dotnet run -c Release \
  --project tests/performance/Benchmarks/Benchmarks.csproj -- \
  --dromaeo \
  --filter "*DromaeoExecutionBenchmarks*" \
  --scenario dromaeo-3d-cube
```

For the paired cube investigation and IL counters, use:

```bash
node scripts/runCubePhasedGuardrails.js --dry
node scripts/runCubePhasedGuardrails.js --dry --il-smells
```

## Investigation Workflow

1. Identify the exact scenario, source, metric, runtime, commit SHA, and
   runner/host. Do not compare unrelated benchmark types or machines.
2. Retrieve the matching workflow artifact and inspect the raw JSON before
   querying aggregate table results.
3. Query recent comparable rows using the same `source`, `scenario`, `metric`,
   runtime, and host metadata. Compare several runs to account for noise.
4. Read the timed benchmark method and identify its actual phase boundary.
5. Inspect corresponding generated methods plus module initialization. For
   compiler changes, explain which call, allocation, coercion, construction,
   or branch changed in an executed path.
6. Reproduce locally only with the documented single-scenario benchmark
   command. For Dromaeo use:

   ```bash
   node scripts/runPhasedBenchmarkScenario.js <scenario>
   ```

7. Report the result with provenance: workflow run, SHA, source, scenario,
   metric/unit, runtime version, host, and sample size. State uncertainty when
   data is sparse or runners differ.

## Guardrails

- Do not claim a compiler optimization caused a benchmark change without an IL
  or runtime-path explanation.
- Treat release and manually dispatched runs separately unless versions, SHA,
  runtime, host, and benchmark parameters match.
- Preserve JavaScript semantics over a benchmark-only optimization. Dynamic
  property overrides, aliases, and escape paths must retain their fallback
  behavior.
- Do not write to Supabase from a development desktop unless the task
  explicitly requires ingestion and the correct credentials and policy are
  available.
