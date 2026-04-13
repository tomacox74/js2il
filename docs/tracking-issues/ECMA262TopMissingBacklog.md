# ECMA-262 Top Missing Features Backlog

> **Last Updated**: 2026-04-13
> Purpose: capture the highest-impact ECMA-262 gaps still marked missing or materially incomplete in `docs\ECMA262`.
> Source basis: current generated `docs\ECMA262\**\Section*.md` / JSON tracking, cross-checked against live GitHub issue state.

## What changed since the previous pass

- The previous top tracked issue [#859](https://github.com/tomacox74/js2il/issues/859) is now **closed**, so the remaining highest-value ECMA gaps are mostly **issue-creation candidates**, not already-open GitHub issues.
- The best remaining backlog items are now concentrated in TypedArray fidelity, the new Uint8Array base64/hex surface, Promise follow-ons, and missing constructor/prototype intrinsics for generator-family function objects.

## How this pass was filtered

- Included items that are still marked `Not Yet Supported`, plus materially incomplete user-visible surfaces where the docs describe a large missing feature family.
- Collapsed closely related clauses into a single backlog item when the docs describe one missing surface.
- De-prioritized low-value or intentionally unsupported items (`with`, `debugger`) and metadata-only gaps whose underlying runtime behavior is already broadly usable.

## Current ranked backlog (recommended order)

| Rank | GitHub issue | Missing feature | Key tracked clauses | Why it is still top-ranked |
|---:|---|---|---|---|
| 1 | No dedicated issue yet | TypedArray constructor / prototype fidelity and remaining methods | [`23.2 TypedArray Objects`](../ECMA262/23/Section23_2.md), [`10.4.5 TypedArray Exotic Objects`](../ECMA262/10/Section10_4.md) | The current docs show a meaningful supported slice for `Int32Array`, `Uint8Array`, and `Float64Array`, but `%TypedArray%` constructor/prototype fidelity, species hooks, metadata surfaces, and many remaining methods are still incomplete. |
| 2 | No dedicated issue yet | Uint8Array base64 / hex extensions | [`23.3 Uint8Array Objects`](../ECMA262/23/Section23_3.md) | This entire ES2025-facing surface is still `Not Yet Supported`, making it one of the clearest user-visible missing built-in families in the docs. |
| 3 | No dedicated issue yet | Promise follow-up surface and prototype metadata | [`27.2 Promise Objects`](../ECMA262/27/Section27_2.md) | Core Promise behavior is in good shape, but the docs still call out missing `HostPromiseRejectionTracker`, `Promise.try`, `%Symbol.species%`, and prototype metadata/toStringTag details. |
| 4 | No dedicated issue yet | Generator / async / async-generator constructor and prototype intrinsics | [`27.3`](../ECMA262/27/Section27_3.md), [`27.4`](../ECMA262/27/Section27_4.md), [`27.7`](../ECMA262/27/Section27_7.md) | Syntax support is mostly there, but the spec-shaped `GeneratorFunction`, `AsyncGeneratorFunction`, and `AsyncFunction` constructor/prototype surfaces remain unimplemented. |
| 5 | No dedicated issue yet | Remaining class/runtime edge-case fidelity after [#859](https://github.com/tomacox74/js2il/issues/859) | [`15.7 Class Definitions`](../ECMA262/15/Section15_7.md) | The major class-element tranche landed, but the docs still show limitations around arbitrary runtime-computed class method names and at least one not-yet-supported static-block-related clause (`15.7.12`). |

## Notable next-tier gaps

- **ArrayBuffer-backed binary fidelity around the TypedArray slice**: much of the remaining pain is clustered around the same TypedArray / ArrayBuffer family, so future issue scoping should probably split user-visible methods from lower-level buffer/spec-invariant work.
- **Promise host integration semantics**: if runtime-host parity becomes more important, the Promise follow-up should probably split host rejection tracking from the remaining constructor/prototype metadata.
- **Function-object intrinsic parity**: the generator-family constructor/prototype follow-on is primarily about reflective and meta-programming compatibility rather than syntax support.

## Intentionally de-prioritized missing items

- [`14.11 The with Statement`](../ECMA262/14/Section14_11.md)
- [`14.16 The debugger Statement`](../ECMA262/14/Section14_16.md)

These are still marked unsupported, but they remain low-value or intentionally deferred compared with the backlog above.
