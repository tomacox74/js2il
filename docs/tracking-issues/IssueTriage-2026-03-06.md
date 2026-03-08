# Issue triage snapshot (2026-03-08)

This file captures a point-in-time prioritized list of open issues/PRs and the current active item being addressed.

Synced to:
- Repo: `master` @ `5ed128dc` (merge of PR #819 RegExp broadening for issue #775)
- GitHub: open issues/PRs state as of 2026-03-08 (refreshed in-place)

## Current active item
**Issue #583** (OPEN; active work is now on `copilot/gh-583-canary-smoke`, with local validation complete and a new draft PR still to be opened):
- https://github.com/tomacox74/js2il/issues/583
- Existing older PR for the same issue (still open, but not merged): https://github.com/tomacox74/js2il/pull/704

Rationale:
- #775 is now closed via merged PR #819, so #583 is the next unfinished ranked item from this list.
- The current branch adds a bounded canary harness (`scripts/differential-test/runCanarySuites.js`), a dedicated CI workflow (`.github/workflows/canary-smoke.yml`), and documented PR/nightly suites under `scripts/differential-test/corpus/canary/`.
- The PR gate currently uses `dromaeo-object-array-modern` and `dromaeo-object-regexp`; the nightly-expanded lane adds `array-stress` and `stopwatch-modern`.
- Local validation is green: `npm run diff:test:canary`, `npm run diff:test:canary:nightly`, and `dotnet build .\Js2IL\Js2IL.csproj -c Release --nologo`.
- The originally suggested ECMA-262 HTML→Markdown converter remains a good future canary, but the direct `turndown`/`domino` dependency path still exposes unsupported transitive surfaces during JS2IL compilation, so the initial lane is seeded with already-supported real benchmark scenarios.

## Recommended next item after #583
**Issue #776** (OPEN):
- https://github.com/tomacox74/js2il/issues/776

Rationale:
- It becomes the next highest-ranked unfinished medium-priority spec item once the canary lane lands.
- Ordinary object internal-method invariants are a good foundation for the immediately adjacent integrity/property-semantics work in #777.

## Recently completed since the previous snapshot
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

## Triage note (hygiene)
**Issue #772 is still OPEN but appears implemented in `master`** (commit `78edaae0` and later master updates). It should be closed or explicitly re-scoped:
- https://github.com/tomacox74/js2il/issues/772
- https://github.com/tomacox74/js2il/commit/78edaae01b2b3da4560068b12314005a4c40387a

## Top 10 open issues (excluding #772, which still looks like hygiene work)
(Heuristic ranking using `priority:*` labels + module/spec/perf keywords in titles/labels; recommendation-only.)

1. #583 (priority:medium) Quality: add real-world canary corpus smoke tests (bounded) for ecosystem stability
   - https://github.com/tomacox74/js2il/issues/583
   - Current branch status: `copilot/gh-583-canary-smoke` now has a bounded PR/nightly canary lane with documented scenario selection and green local validation; draft PR creation is the remaining packaging step.
2. #776 (priority:medium) ECMA-262: Ordinary object internal method invariants (DefineOwnProperty/GetOwnProperty, keys ordering)
   - https://github.com/tomacox74/js2il/issues/776
3. #777 (priority:medium) ECMA-262: Object integrity APIs semantics audit (freeze/seal/preventExtensions/is*)
   - https://github.com/tomacox74/js2il/issues/777
4. #778 (priority:medium) ECMA-262: Support Function constructor (new Function / CreateDynamicFunction)
   - https://github.com/tomacox74/js2il/issues/778
5. #779 (priority:medium) ECMA-262: Symbol ecosystem completeness audit (well-known symbols + symbol-key introspection semantics)
   - https://github.com/tomacox74/js2il/issues/779
6. #780 (priority:medium) ECMA-262: Expose Array iterator methods (entries/keys/values/[Symbol.iterator])
   - https://github.com/tomacox74/js2il/issues/780
7. #787 (priority:medium) node: expand util essentials (format, inspect parity, util.types breadth)
   - https://github.com/tomacox74/js2il/issues/787
8. #788 (priority:medium) node: expand child_process beyond sync (spawn/exec/execFile, stdio pipes)
   - https://github.com/tomacox74/js2il/issues/788
9. #789 (priority:medium) node: implement url/querystring baseline (URL, URLSearchParams, parse/stringify)
   - https://github.com/tomacox74/js2il/issues/789
10. #790 (priority:medium) node: implement crypto minimum practical subset (createHash, randomBytes, webcrypto bridge)
    - https://github.com/tomacox74/js2il/issues/790

## Remaining open issues
11. #781 (priority:low) ECMA-262: Implement WeakRef + FinalizationRegistry host cleanup model
    - https://github.com/tomacox74/js2il/issues/781
12. #791 (priority:low) node: add ESM interop baseline (import.meta.url + Node-style ESM resolution plan)
    - https://github.com/tomacox74/js2il/issues/791
13. #792 (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton)
    - https://github.com/tomacox74/js2il/issues/792
14. #419 Hosting: support mutable CommonJS exports (Node-like)
    - https://github.com/tomacox74/js2il/issues/419
15. #439 Hosting: publish referenceable library/build NuGet package
    - https://github.com/tomacox74/js2il/issues/439
16. #451 perf(il): expand typed temps/locals to reduce casts/boxing
    - https://github.com/tomacox74/js2il/issues/451
17. #727 Function length/name should be descriptor-backed own properties
    - https://github.com/tomacox74/js2il/issues/727
18. #728 Complete bound function semantics for constructor/new-target and metadata
    - https://github.com/tomacox74/js2il/issues/728
19. #737 perf: callsite-based typed parameter specialization for non-exported functions
    - https://github.com/tomacox74/js2il/issues/737
20. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
    - https://github.com/tomacox74/js2il/issues/738
21. #740 perf(prime): keep sieve loop math in typed locals with fallback
    - https://github.com/tomacox74/js2il/issues/740
22. #742 perf(prime): trim timing/config coercion overhead in main path
    - https://github.com/tomacox74/js2il/issues/742
23. #743 perf(prime): add Prime perf acceptance gate and reporting
    - https://github.com/tomacox74/js2il/issues/743
24. #746 perf: make dromaeo-object-regexp faster than Jint prepared
    - https://github.com/tomacox74/js2il/issues/746
25. #747 perf(regexp): cache Regex instances by source+flags
    - https://github.com/tomacox74/js2il/issues/747
26. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748
27. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
    - https://github.com/tomacox74/js2il/issues/768

## Open PRs (for context)
- #704 (OPEN): https://github.com/tomacox74/js2il/pull/704 — earlier attempt at issue #583 canary smoke coverage; still open but superseded by the current `copilot/gh-583-canary-smoke` work-in-progress branch.

## Label/metadata gaps (as of this snapshot)
- Open issues: 28
- Missing `priority:*` label: 14
- Missing `lane:*` label: 28
