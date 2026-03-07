# Issue triage snapshot (2026-03-07)

This file captures a point-in-time prioritized list of open issues/PRs and the current active item being addressed.

Synced to:
- Repo: `master` @ `b2d6b1fb` (merge of PR #813 / GH #812 callable scope ABI metadata)
- GitHub: open issues/PRs state as of 2026-03-07

## Current active item
**Issue #773** (OPEN; highest-priority remaining item from this list and addressed on the `copilot/gh-773-arraybuffer-dataview` branch):
- https://github.com/tomacox74/js2il/issues/773

Rationale:
- #581 and #584 are now closed, so #773 is the first unfinished issue in the prioritized list below.
- It unblocks #774 by providing the ArrayBuffer/DataView foundation needed for ArrayBuffer-backed TypedArray work.

## Recommended next item after #773
**Issue #774** (OPEN):
- https://github.com/tomacox74/js2il/issues/774

Rationale:
- It is the direct follow-on to #773 and depends on the ArrayBuffer/DataView primitives landing first.
- Completing it would turn the existing minimal typed-array surface into broader, real-world TypedArray coverage.

## Recently completed since the previous snapshot
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

1. #773 (priority:high) ECMA-262: Implement ArrayBuffer + DataView primitives (foundation for TypedArray semantics)
   - https://github.com/tomacox74/js2il/issues/773
2. #774 (priority:high) ECMA-262: Complete %TypedArray% constructors + prototype methods
   - https://github.com/tomacox74/js2il/issues/774
3. #775 (priority:high) ECMA-262: RegExp modern flags and symbol methods parity (u/y/d/v, @@match, etc.)
   - https://github.com/tomacox74/js2il/issues/775
4. #583 (priority:medium) Quality: add real-world canary corpus smoke tests (bounded) for ecosystem stability
   - https://github.com/tomacox74/js2il/issues/583
5. #776 (priority:medium) ECMA-262: Ordinary object internal method invariants (DefineOwnProperty/GetOwnProperty, keys ordering)
   - https://github.com/tomacox74/js2il/issues/776
6. #777 (priority:medium) ECMA-262: Object integrity APIs semantics audit (freeze/seal/preventExtensions/is*)
   - https://github.com/tomacox74/js2il/issues/777
7. #778 (priority:medium) ECMA-262: Support Function constructor (new Function / CreateDynamicFunction)
   - https://github.com/tomacox74/js2il/issues/778
8. #779 (priority:medium) ECMA-262: Symbol ecosystem completeness audit (well-known symbols + symbol-key introspection semantics)
   - https://github.com/tomacox74/js2il/issues/779
9. #780 (priority:medium) ECMA-262: Expose Array iterator methods (entries/keys/values/[Symbol.iterator])
   - https://github.com/tomacox74/js2il/issues/780
10. #787 (priority:medium) node: expand util essentials (format, inspect parity, util.types breadth)
    - https://github.com/tomacox74/js2il/issues/787

## Remaining open issues
11. #788 (priority:medium) node: expand child_process beyond sync (spawn/exec/execFile, stdio pipes)
    - https://github.com/tomacox74/js2il/issues/788
12. #789 (priority:medium) node: implement url/querystring baseline (URL, URLSearchParams, parse/stringify)
    - https://github.com/tomacox74/js2il/issues/789
13. #790 (priority:medium) node: implement crypto minimum practical subset (createHash, randomBytes, webcrypto bridge)
    - https://github.com/tomacox74/js2il/issues/790
14. #781 (priority:low) ECMA-262: Implement WeakRef + FinalizationRegistry host cleanup model
    - https://github.com/tomacox74/js2il/issues/781
15. #791 (priority:low) node: add ESM interop baseline (import.meta.url + Node-style ESM resolution plan)
    - https://github.com/tomacox74/js2il/issues/791
16. #792 (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton)
    - https://github.com/tomacox74/js2il/issues/792
17. #419 Hosting: support mutable CommonJS exports (Node-like)
    - https://github.com/tomacox74/js2il/issues/419
18. #439 Hosting: publish referenceable library/build NuGet package
    - https://github.com/tomacox74/js2il/issues/439
19. #451 perf(il): expand typed temps/locals to reduce casts/boxing
    - https://github.com/tomacox74/js2il/issues/451
20. #727 Function length/name should be descriptor-backed own properties
    - https://github.com/tomacox74/js2il/issues/727
21. #728 Complete bound function semantics for constructor/new-target and metadata
    - https://github.com/tomacox74/js2il/issues/728
22. #737 perf: callsite-based typed parameter specialization for non-exported functions
    - https://github.com/tomacox74/js2il/issues/737
23. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
    - https://github.com/tomacox74/js2il/issues/738
24. #740 perf(prime): keep sieve loop math in typed locals with fallback
    - https://github.com/tomacox74/js2il/issues/740
25. #742 perf(prime): trim timing/config coercion overhead in main path
    - https://github.com/tomacox74/js2il/issues/742
26. #743 perf(prime): add Prime perf acceptance gate and reporting
    - https://github.com/tomacox74/js2il/issues/743
27. #746 perf: make dromaeo-object-regexp faster than Jint prepared
    - https://github.com/tomacox74/js2il/issues/746
28. #747 perf(regexp): cache Regex instances by source+flags
    - https://github.com/tomacox74/js2il/issues/747
29. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748
30. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
    - https://github.com/tomacox74/js2il/issues/768

## Open PRs (for context)
- #704 (OPEN): https://github.com/tomacox74/js2il/pull/704

## Label/metadata gaps (as of this snapshot)
- Open issues: 31
- Missing `priority:*` label: 14
- Missing `lane:*` label: 31
