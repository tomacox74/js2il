# Issue triage snapshot (2026-03-06)

This file captures a point-in-time prioritized list of open issues/PRs and the current active item being addressed.

Synced to:
- Repo: `master` @ `e86a73c5` (post-merge state through PR #826 / issue #787)
- GitHub: open issues/PRs state as of 2026-03-09 (refreshed in-place)

## Current active item
**Issue #788** (OPEN; active work is now on `copilot/gh-788-child-process-async`):
- https://github.com/tomacox74/js2il/issues/788

Rationale:
- #787 is now closed via merged PR #826, so the next ranked unfinished issue after the active #780 review work becomes #788 from the triage ordering.
- The current branch adds the minimal async `child_process` slice requested by the issue: `spawn`, `exec`, `execFile`, piped stdout/stderr capture, callback completion, and a basic child handle/event surface.
- The branch builds directly on the existing process, events, stream, and scheduler infrastructure already in `master`.

## Recommended next item after #788
**Issue #789** (OPEN):
- https://github.com/tomacox74/js2il/issues/789

Rationale:
- Once the async child_process baseline lands, #789 becomes the next unfinished medium-priority Node compatibility item in the overall ranked list.
- URL and querystring coverage remains a high-leverage ecosystem gap and is the next ranked module-level item after child_process.

## Recently completed since the previous snapshot
- **#787** (priority:medium) node: expand util essentials (format, inspect parity, util.types breadth) — CLOSED via PR #826 on 2026-03-09
  - https://github.com/tomacox74/js2il/issues/787
  - https://github.com/tomacox74/js2il/pull/826
- **#779** (priority:medium) ECMA-262: Symbol ecosystem completeness audit (well-known symbols + symbol-key introspection semantics) — CLOSED via PR #824 on 2026-03-09
  - https://github.com/tomacox74/js2il/issues/779
  - https://github.com/tomacox74/js2il/pull/824
- **#778** (priority:medium) ECMA-262: Support Function constructor (new Function / CreateDynamicFunction) — CLOSED via PR #823 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/778
  - https://github.com/tomacox74/js2il/pull/823
- **#777** (priority:medium) ECMA-262: Object integrity APIs semantics audit (freeze/seal/preventExtensions/is*) — CLOSED via PR #822 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/777
  - https://github.com/tomacox74/js2il/pull/822
- **#776** (priority:medium) ECMA-262: Ordinary object internal method invariants (DefineOwnProperty/GetOwnProperty, keys ordering) — CLOSED via PR #821 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/776
  - https://github.com/tomacox74/js2il/pull/821
- **#775** (priority:high) ECMA-262: RegExp modern flags and symbol methods parity — CLOSED via PR #819 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/775
  - https://github.com/tomacox74/js2il/pull/819
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

## Triage notes
**Issue #772 is still OPEN but appears mostly implemented in `master`** (commit `78edaae0` and later master updates). It should be closed or explicitly re-scoped:
- https://github.com/tomacox74/js2il/issues/772
- https://github.com/tomacox74/js2il/commit/78edaae01b2b3da4560068b12314005a4c40387a

**Issue #583 is still OPEN even though PR #820 merged an initial bounded canary slice.** Treat it as follow-up expansion/hardening work rather than a completed item:
- https://github.com/tomacox74/js2il/issues/583
- https://github.com/tomacox74/js2il/pull/820

## Top 10 open issues after the current active item (excluding #772, which still looks like hygiene work)
(Heuristic ranking using `priority:*` labels + module/spec/perf keywords in titles/labels; recommendation-only.)

1. #789 (priority:medium) node: implement url/querystring baseline (URL, URLSearchParams, parse/stringify)
   - https://github.com/tomacox74/js2il/issues/789
2. #790 (priority:medium) node: implement crypto minimum practical subset (createHash, randomBytes, webcrypto bridge)
   - https://github.com/tomacox74/js2il/issues/790
3. #583 (priority:medium) Quality: add real-world canary corpus smoke tests (bounded) for ecosystem stability
   - https://github.com/tomacox74/js2il/issues/583
   - Current signal: PR #820 landed an initial bounded smoke gate, but the issue remains open for broader corpus/artifact follow-up.
4. #781 (priority:low) ECMA-262: Implement WeakRef + FinalizationRegistry host cleanup model
   - https://github.com/tomacox74/js2il/issues/781
5. #791 (priority:low) node: add ESM interop baseline (import.meta.url + Node-style ESM resolution plan)
   - https://github.com/tomacox74/js2il/issues/791
6. #792 (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton)
   - https://github.com/tomacox74/js2il/issues/792
7. #727 Function length/name should be descriptor-backed own properties
   - https://github.com/tomacox74/js2il/issues/727
8. #728 Complete bound function semantics for constructor/new-target and metadata
   - https://github.com/tomacox74/js2il/issues/728
9. #419 Hosting: support mutable CommonJS exports (Node-like)
   - https://github.com/tomacox74/js2il/issues/419
10. #439 Hosting: publish referenceable library/build NuGet package
    - https://github.com/tomacox74/js2il/issues/439

## Remaining open issues
11. #451 perf(il): expand typed temps/locals to reduce casts/boxing
    - https://github.com/tomacox74/js2il/issues/451
12. #737 perf: callsite-based typed parameter specialization for non-exported functions
    - https://github.com/tomacox74/js2il/issues/737
13. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
    - https://github.com/tomacox74/js2il/issues/738
14. #740 perf(prime): keep sieve loop math in typed locals with fallback
    - https://github.com/tomacox74/js2il/issues/740
15. #742 perf(prime): trim timing/config coercion overhead in main path
    - https://github.com/tomacox74/js2il/issues/742
16. #743 perf(prime): add Prime perf acceptance gate and reporting
    - https://github.com/tomacox74/js2il/issues/743
17. #746 perf: make dromaeo-object-regexp faster than Jint prepared
    - https://github.com/tomacox74/js2il/issues/746
18. #747 perf(regexp): cache Regex instances by source+flags
    - https://github.com/tomacox74/js2il/issues/747
19. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748
20. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
    - https://github.com/tomacox74/js2il/issues/768

## Open PRs (for context)
- #825: https://github.com/tomacox74/js2il/pull/825 — Array iterator surface coverage for issue #780.

## Label/metadata gaps (as of this snapshot)
- Open issues: 24
- Open PRs: 1
- 14 open issues are missing `priority:*` labels.
- No current open issues carry `lane:*` labels (24 missing).
