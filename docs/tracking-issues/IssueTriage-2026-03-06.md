# Issue triage snapshot (2026-03-06)

This file captures a point-in-time prioritized list of open issues/PRs and the current active item being addressed.

Synced to:
- Repo: `master` @ `739561b4` (merge of PR #818 typed-array broadening for issue #774)
- GitHub: open issues/PRs state as of 2026-03-08 (refreshed in-place)

## Current active item
**Issue #775** (OPEN; highest-priority remaining item from this list and currently being addressed on the `copilot/gh-775-regexp-sticky` branch via draft PR #819):
- https://github.com/tomacox74/js2il/issues/775
- https://github.com/tomacox74/js2il/pull/819

Rationale:
- #774 is now closed via merged PR #818, so #775 is the first unfinished `priority:high` item in the ranking below.
- The current branch now broadens PR #819 beyond the initial sticky slice: sticky `/y` exec/test/lastIndex behavior, `s` / `u` / `d` flag parsing and reflection, limited unicode rewriting for common `/u` cases, minimal `.indices` support for `/d`, explicit `v` rejection, and well-known symbol dispatch for `match` / `replace` / `search` / `split`.
- Focused RegExp/String execution + generator validation is green (`80` tests passed).
- Docs now reflect the supported `g`, `i`, `m`, `s`, `u`, `d`, and `y` surface plus the remaining limitations (`v` rejection, `matchAll` and fuller unicode/exotic semantics still pending).
- Draft PR #819 now appears sufficient to close issue #775 on merge.

## Recommended next item after #775
**Issue #583** (OPEN):
- https://github.com/tomacox74/js2il/issues/583

Rationale:
- It is now the next highest-ranked unfinished item once #775 moves out of the active slot.
- A bounded real-world canary lane would complement the recent stopwatch and typed-array work by giving earlier warning on ecosystem-level breakage.

## Recently completed since the previous snapshot
- **#774** (priority:high) ECMA-262: Complete %TypedArray% constructors + prototype methods — CLOSED via PR #818 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/774
  - https://github.com/tomacox74/js2il/pull/818
- **#773** (priority:high) ECMA-262: Implement ArrayBuffer + DataView primitives (foundation for TypedArray semantics) — CLOSED on 2026-03-07
  - https://github.com/tomacox74/js2il/issues/773
- **#581** (priority:high) Quality: bounded differential testing (Node vs JS2IL) for semantic regressions — CLOSED on 2026-03-06
  - https://github.com/tomacox74/js2il/issues/581
- **#584** (priority:high) Quality: add compiler shape-coverage micro-tests (joins/back-edges/materialization) — CLOSED on 2026-03-06
  - https://github.com/tomacox74/js2il/issues/584
- **#812** perf(abi): introduce callable scope ABI attribute and variants — merged via PR #813 on 2026-03-07
  - https://github.com/tomacox74/js2il/issues/812
  - https://github.com/tomacox74/js2il/pull/813

## Triage note (hygiene)
**Issue #772 is still OPEN but appears implemented in `master`** (commit `78edaae0` and later master updates). It should be closed or explicitly re-scoped:
- https://github.com/tomacox74/js2il/issues/772
- https://github.com/tomacox74/js2il/commit/78edaae01b2b3da4560068b12314005a4c40387a

## Top 10 open issues (excluding #772, which still looks like hygiene work)
(Heuristic ranking using `priority:*` labels + module/spec/perf keywords in titles/labels; recommendation-only.)

1. #775 (priority:high) ECMA-262: RegExp modern flags and symbol methods parity (u/y/d/v, @@match, etc.)
   - https://github.com/tomacox74/js2il/issues/775
   - Current branch status: draft PR #819 now covers sticky `/y`, minimal `/d` indices, limited `/u` + `/s`, explicit `v` rejection, and well-known symbol dispatch for `match` / `replace` / `search` / `split`; it appears sufficient to close the issue after review/merge.
2. #583 (priority:medium) Quality: add real-world canary corpus smoke tests (bounded) for ecosystem stability
   - https://github.com/tomacox74/js2il/issues/583
3. #776 (priority:medium) ECMA-262: Ordinary object internal method invariants (DefineOwnProperty/GetOwnProperty, keys ordering)
   - https://github.com/tomacox74/js2il/issues/776
4. #777 (priority:medium) ECMA-262: Object integrity APIs semantics audit (freeze/seal/preventExtensions/is*)
   - https://github.com/tomacox74/js2il/issues/777
5. #778 (priority:medium) ECMA-262: Support Function constructor (new Function / CreateDynamicFunction)
   - https://github.com/tomacox74/js2il/issues/778
6. #779 (priority:medium) ECMA-262: Symbol ecosystem completeness audit (well-known symbols + symbol-key introspection semantics)
   - https://github.com/tomacox74/js2il/issues/779
7. #780 (priority:medium) ECMA-262: Expose Array iterator methods (entries/keys/values/[Symbol.iterator])
   - https://github.com/tomacox74/js2il/issues/780
8. #787 (priority:medium) node: expand util essentials (format, inspect parity, util.types breadth)
   - https://github.com/tomacox74/js2il/issues/787
9. #788 (priority:medium) node: expand child_process beyond sync (spawn/exec/execFile, stdio pipes)
   - https://github.com/tomacox74/js2il/issues/788
10. #789 (priority:medium) node: implement url/querystring baseline (URL, URLSearchParams, parse/stringify)
    - https://github.com/tomacox74/js2il/issues/789

## Remaining open issues
11. #790 (priority:medium) node: implement crypto minimum practical subset (createHash, randomBytes, webcrypto bridge)
    - https://github.com/tomacox74/js2il/issues/790
12. #781 (priority:low) ECMA-262: Implement WeakRef + FinalizationRegistry host cleanup model
    - https://github.com/tomacox74/js2il/issues/781
13. #791 (priority:low) node: add ESM interop baseline (import.meta.url + Node-style ESM resolution plan)
    - https://github.com/tomacox74/js2il/issues/791
14. #792 (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton)
    - https://github.com/tomacox74/js2il/issues/792
15. #419 Hosting: support mutable CommonJS exports (Node-like)
    - https://github.com/tomacox74/js2il/issues/419
16. #439 Hosting: publish referenceable library/build NuGet package
    - https://github.com/tomacox74/js2il/issues/439
17. #451 perf(il): expand typed temps/locals to reduce casts/boxing
    - https://github.com/tomacox74/js2il/issues/451
18. #727 Function length/name should be descriptor-backed own properties
    - https://github.com/tomacox74/js2il/issues/727
19. #728 Complete bound function semantics for constructor/new-target and metadata
    - https://github.com/tomacox74/js2il/issues/728
20. #737 perf: callsite-based typed parameter specialization for non-exported functions
    - https://github.com/tomacox74/js2il/issues/737
21. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
    - https://github.com/tomacox74/js2il/issues/738
22. #740 perf(prime): keep sieve loop math in typed locals with fallback
    - https://github.com/tomacox74/js2il/issues/740
23. #742 perf(prime): trim timing/config coercion overhead in main path
    - https://github.com/tomacox74/js2il/issues/742
24. #743 perf(prime): add Prime perf acceptance gate and reporting
    - https://github.com/tomacox74/js2il/issues/743
25. #746 perf: make dromaeo-object-regexp faster than Jint prepared
    - https://github.com/tomacox74/js2il/issues/746
26. #747 perf(regexp): cache Regex instances by source+flags
    - https://github.com/tomacox74/js2il/issues/747
27. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748
28. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
    - https://github.com/tomacox74/js2il/issues/768

## Open PRs (for context)
- #704 (OPEN): https://github.com/tomacox74/js2il/pull/704
- #819 (OPEN, draft): https://github.com/tomacox74/js2il/pull/819 — broadened RegExp issue #775 coverage (`y` / `s` / `u` / `d`, `v` rejection, and symbol dispatch for `match` / `replace` / `search` / `split`)

## Label/metadata gaps (as of this snapshot)
- Open issues: 29
- Missing `priority:*` label: 14
- Missing `lane:*` label: 29
