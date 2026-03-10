# Issue triage snapshot (2026-03-06)

This file captures a point-in-time prioritized list of open issues/PRs and the current active item being addressed.

Synced to:
- Repo: `master` @ `6cb37ca3` (post-merge state through PR #830 / issue #583 landing)
- Active branch: `copilot/gh-781-weakref-finalizationregistry-baseline` @ `34d76c09` (open PR #831 for issue #781)
- GitHub: open issues/PRs state as of 2026-03-09 (refreshed after PR #831 opened)

## Current active item
**Issue #781** (OPEN; active work is now on `copilot/gh-781-weakref-finalizationregistry-baseline`, PR #831 is open):
- https://github.com/tomacox74/js2il/issues/781
- https://github.com/tomacox74/js2il/pull/831

Rationale:
- #583 landed via PR #830, so #781 is now the next ranked unfinished item.
- PR #831 already carries the WeakRef / FinalizationRegistry baseline, so the remaining work is review/merge plus any review-driven follow-up.
- This is now the highest-ranked open issue in the current triage list once the canary follow-up closed.

## Recommended next item after #781
**Issue #791** (OPEN):
- https://github.com/tomacox74/js2il/issues/791

Rationale:
- Once PR #831 lands, #791 becomes the next ranked unfinished item in the list.
- It is the next concrete Node compatibility item ahead of the broader networking plan in #792.
- It remains a scoped follow-up after the more runtime-heavy WeakRef landing.

## Recently completed since the previous snapshot
- **#583** (priority:medium) Quality: add real-world canary corpus smoke tests (bounded) for ecosystem stability — CLOSED via PR #830 on 2026-03-09
  - https://github.com/tomacox74/js2il/issues/583
  - https://github.com/tomacox74/js2il/pull/830
- **#790** (priority:medium) node: implement crypto minimum practical subset (createHash, randomBytes, webcrypto bridge) — code landed via PR #829 on 2026-03-09
  - https://github.com/tomacox74/js2il/issues/790
  - https://github.com/tomacox74/js2il/pull/829
- **#789** (priority:medium) node: implement url/querystring baseline (URL, URLSearchParams, parse/stringify) — CLOSED via PR #828 on 2026-03-09
  - https://github.com/tomacox74/js2il/issues/789
  - https://github.com/tomacox74/js2il/pull/828
- **#788** (priority:medium) node: expand child_process beyond sync (spawn/exec/execFile, stdio pipes) — CLOSED via PR #827 on 2026-03-09
  - https://github.com/tomacox74/js2il/issues/788
  - https://github.com/tomacox74/js2il/pull/827
- **#787** (priority:medium) node: expand util essentials (format, inspect parity, util.types breadth) — CLOSED via PR #826 on 2026-03-09
  - https://github.com/tomacox74/js2il/issues/787
  - https://github.com/tomacox74/js2il/pull/826
- **#780** (priority:medium) ECMA-262: Expose Array iterator methods (entries/keys/values/[Symbol.iterator]) — CLOSED via PR #825 on 2026-03-09
  - https://github.com/tomacox74/js2il/issues/780
  - https://github.com/tomacox74/js2il/pull/825
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

**Issue #790 is still OPEN even though PR #829 merged the crypto baseline into `master`.** It should be closed or explicitly re-scoped:
- https://github.com/tomacox74/js2il/issues/790
- https://github.com/tomacox74/js2il/pull/829

## Top 10 open issues after the current active item (excluding #772 and #790, which currently look like hygiene follow-up)
(Heuristic ranking using `priority:*` labels + module/spec/perf keywords in titles/labels; recommendation-only.)

1. #791 (priority:low) node: add ESM interop baseline (import.meta.url + Node-style ESM resolution plan)
   - https://github.com/tomacox74/js2il/issues/791
2. #792 (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton)
   - https://github.com/tomacox74/js2il/issues/792
3. #727 Function length/name should be descriptor-backed own properties
   - https://github.com/tomacox74/js2il/issues/727
4. #728 Complete bound function semantics for constructor/new-target and metadata
   - https://github.com/tomacox74/js2il/issues/728
5. #419 Hosting: support mutable CommonJS exports (Node-like)
   - https://github.com/tomacox74/js2il/issues/419
6. #439 Hosting: publish referenceable library/build NuGet package
   - https://github.com/tomacox74/js2il/issues/439
7. #451 perf(il): expand typed temps/locals to reduce casts/boxing
      - https://github.com/tomacox74/js2il/issues/451
8. #737 perf: callsite-based typed parameter specialization for non-exported functions
      - https://github.com/tomacox74/js2il/issues/737
9. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
      - https://github.com/tomacox74/js2il/issues/738
10. #740 perf(prime): keep sieve loop math in typed locals with fallback
    - https://github.com/tomacox74/js2il/issues/740

## Remaining open issues
11. #742 perf(prime): trim timing/config coercion overhead in main path
    - https://github.com/tomacox74/js2il/issues/742
12. #743 perf(prime): add Prime perf acceptance gate and reporting
    - https://github.com/tomacox74/js2il/issues/743
13. #746 perf: make dromaeo-object-regexp faster than Jint prepared
    - https://github.com/tomacox74/js2il/issues/746
14. #747 perf(regexp): cache Regex instances by source+flags
    - https://github.com/tomacox74/js2il/issues/747
15. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748
16. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
    - https://github.com/tomacox74/js2il/issues/768

## Open PRs (for context)
- **#831** `feat(ecma262): add WeakRef cleanup baseline` — OPEN
  - https://github.com/tomacox74/js2il/pull/831

## Label/metadata gaps (as of this snapshot)
- Open issues: 19
- Open PRs: 1
- 14 open issues are missing `priority:*` labels.
- No current open issues carry `lane:*` labels (19 missing).
