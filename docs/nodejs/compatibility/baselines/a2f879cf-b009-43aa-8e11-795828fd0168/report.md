# Node public-API first-slice evidence

As of 2026-10-23T22:35:52.220Z; JROC checkout `adf3d0fe2ada282fc6cd0cfba15f596c1dd286f5`.
Corpus `node-public-api-v1`, executable digest `2ec36077bbf4eceb73565a1fdc3c7972058372de35bcaf7ee2c4d4fd11d795f7`.

**Local-unverified observations, not an overall Node compatibility score or trusted catalogue acceptance.**

19/19 original scenarios selected; 38 retained attempts.
Counts are per scenario, never per target mapping or attempt. Non-pass outcomes remain visible.

| Area | Scenarios | Pass | Mismatch | Compile failure | Other |
| --- | ---: | ---: | ---: | ---: | ---: |
| path | 4 | 2 | 2 | 0 | 0 |
| buffer | 4 | 2 | 2 | 0 | 0 |
| events | 4 | 0 | 4 | 0 | 0 |
| loader | 7 | 4 | 2 | 1 | 0 |

## Scope and limitations

32 pinned upstream discovery leads: 0 runnable, 26 adaptation-required, 1 platform-specific, 5 excluded. 26 have unknown whole-file applicability.
No upstream test file is claimed as executed. Original fixture assertions are independently written; see the retained corpus metadata for exact URLs, revisions, hashes, license provenance and triage reasons.
30/3124 API candidates have selected scenario mappings; 3094 are not selected/tested by this slice. 8 loader behavior registrations have mappings.
Mappings indicate selected assertion intent, not whole-API support or proof every mapped operation was reached. Unmapped APIs are untested by this slice; compile/runtime failures can prevent mapped operations from executing.
Recorded profile: Node 24.22.1, linux x64, .NET SDK 10.0.113, installed .NET runtimes 10.0.13. Exact timings and unconstrained scheduling are not compared. There is no web-platform API in this slice, so no WPT case is claimed.
Upstream private-internal/flag-dependent and oversized tests are triaged separately, never counted as JROC successes or failures. This is not a package-workflow scorecard or a CI regression baseline.
Full bundle retained in the named local run directory, not published central evidence. This report retains lossless stdout/stderr/observable snapshots; compiler binaries remain in the full bundle.

## Scenario outcomes

| Scenario | Outcome | Phase(s) |
| --- | --- | --- |
| path/posix | pass | comparison |
| path/win32 | pass | comparison |
| path/shape-and-boundaries | mismatch | comparison |
| path/invalid-inputs | mismatch | comparison |
| buffer/encodings | pass | comparison |
| buffer/aliasing-and-boundaries | pass | comparison |
| buffer/invalid-inputs | mismatch | comparison |
| buffer/export-shape | mismatch | comparison |
| events/synchronous-order | mismatch | comparison |
| events/once-and-removal | mismatch | comparison |
| events/error-semantics | mismatch | comparison |
| events/export-shape | mismatch | comparison |
| loader/commonjs-relative-and-cache | pass | comparison |
| loader/esm-relative-and-package-type | pass | comparison |
| loader/exports-require | pass | comparison |
| loader/exports-import | compile-failure | compile |
| loader/package-imports | pass | comparison |
| loader/transitive-dependency | mismatch | comparison |
| loader/resolution-errors | mismatch | comparison |

The paired diagnostic bytes, target mappings, attempt history and raw-artifact hashes are retained in the sibling JSON report. Read stdout/stderr `base64` fields losslessly; `Buffer.from(value, "base64").toString("utf8")` is useful for text diagnostics.
A compiler rejection can prevent a scenario from reaching the intended API assertion; inspect diagnostics before attributing the failure to a particular operation. Missing runtime APIs can appear as mismatches rather than the compiler-only `unsupported` classification.
