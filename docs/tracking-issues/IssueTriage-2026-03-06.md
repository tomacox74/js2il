# Issue triage snapshot (2026-03-06)

This file captures a point-in-time prioritized list of open issues/PRs and the current active item being addressed.

Synced to:
- Repo: `master` @ `e73f7e94` (state when branch `copilot/gh-419-mutable-commonjs-exports` was created)
- Active branch: `copilot/gh-419-mutable-commonjs-exports` @ `2ea6b8c6` (open PR #851 for issue #419)
- GitHub: open issues/PRs state as of 2026-03-13 (refreshed after PR #844 merged, issue #439 was decomposed into subissues #845-#850, and PR #851 opened)

## Current active item
**Issue #419** (OPEN; active work is now on `copilot/gh-419-mutable-commonjs-exports`, PR #851 is open):
- https://github.com/tomacox74/js2il/issues/419
- https://github.com/tomacox74/js2il/pull/851

Rationale:
- PR #844 for issue #728 has merged, clearing the previous Function follow-up item.
- Issue #419 is now implemented and in review as PR #851, covering mutable `module.exports` access through both typed and dynamic hosting proxies while preserving script-thread marshalling.
- With the current hosting mutable-exports work in review, the next likely fresh implementation slice shifts to the packaging migration work now decomposed under issue #439.

## Recommended next item after #419
**Issue #845** (OPEN):
- https://github.com/tomacox74/js2il/issues/845

Rationale:
- Issue #439 is now an umbrella tracker rather than a single executable slice; the new concrete starting point is subissue #845.
- #845 extracts `Js2IL.Core` / `Js2IL.Compiler.dll`, which is the first dependency in the now-broken-down hosting/package migration.
- The remaining higher-priority open item #772 still looks like hygiene or scope-clarification follow-up rather than a fresh implementation slice.

## Recently completed since the previous snapshot
- **#728** (priority:medium) Complete bound function semantics for constructor/new-target and metadata — CLOSED via PR #844 on 2026-03-12
  - https://github.com/tomacox74/js2il/issues/728
  - https://github.com/tomacox74/js2il/pull/844
- **#727** (priority:medium) Function length/name should be descriptor-backed own properties — CLOSED via PR #843 on 2026-03-12
  - https://github.com/tomacox74/js2il/issues/727
  - https://github.com/tomacox74/js2il/pull/843
- **#792** (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton) — CLOSED via PR #840 on 2026-03-12
  - https://github.com/tomacox74/js2il/issues/792
  - https://github.com/tomacox74/js2il/pull/840
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
**Issue #439 is now an umbrella packaging migration tracker** and should generally be executed through its subissues (`#845`-`#850`) rather than as a single large PR:
- https://github.com/tomacox74/js2il/issues/439
- https://github.com/tomacox74/js2il/issues/845
- https://github.com/tomacox74/js2il/issues/846
- https://github.com/tomacox74/js2il/issues/847
- https://github.com/tomacox74/js2il/issues/848
- https://github.com/tomacox74/js2il/issues/849
- https://github.com/tomacox74/js2il/issues/850

**Issue #772 is still OPEN but appears mostly implemented in `master`** (commit `78edaae0` and later master updates). It should be closed or explicitly re-scoped:
- https://github.com/tomacox74/js2il/issues/772
- https://github.com/tomacox74/js2il/commit/78edaae01b2b3da4560068b12314005a4c40387a

## Top 10 open issues after the current active item (excluding #772, which currently looks like hygiene follow-up, and treating #439 as an umbrella tracker rather than the next direct execution slice)
(Heuristic ranking using `priority:*` labels + module/spec/perf keywords in titles/labels; recommendation-only.)

1. #845 Hosting/NuGet: create Js2IL.Core and extract Js2IL.Compiler.dll
   - https://github.com/tomacox74/js2il/issues/845
2. #451 perf(il): expand typed temps/locals to reduce casts/boxing
   - https://github.com/tomacox74/js2il/issues/451
3. #737 perf: callsite-based typed parameter specialization for non-exported functions
   - https://github.com/tomacox74/js2il/issues/737
4. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
   - https://github.com/tomacox74/js2il/issues/738
5. #740 perf(prime): keep sieve loop math in typed locals with fallback
   - https://github.com/tomacox74/js2il/issues/740
6. #742 perf(prime): trim timing/config coercion overhead in main path
   - https://github.com/tomacox74/js2il/issues/742
7. #743 perf(prime): add Prime perf acceptance gate and reporting
   - https://github.com/tomacox74/js2il/issues/743
8. #746 perf: make dromaeo-object-regexp faster than Jint prepared
   - https://github.com/tomacox74/js2il/issues/746
9. #747 perf(regexp): cache Regex instances by source+flags
   - https://github.com/tomacox74/js2il/issues/747
10. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
    - https://github.com/tomacox74/js2il/issues/748

## Remaining open issues
11. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
    - https://github.com/tomacox74/js2il/issues/768
12. #439 Hosting/NuGet: migrate compiler packaging to js2il + Js2IL.Core + Js2IL.SDK (umbrella tracker)
    - https://github.com/tomacox74/js2il/issues/439
13. #846 Hosting/NuGet: create Js2IL.SDK MSBuild task package
    - https://github.com/tomacox74/js2il/issues/846
14. #847 Hosting/NuGet: migrate hosting samples and docs to Js2IL.SDK
    - https://github.com/tomacox74/js2il/issues/847
15. #848 Hosting/NuGet: publish js2il, Js2IL.Core, and Js2IL.SDK in release builds
    - https://github.com/tomacox74/js2il/issues/848
16. #849 Hosting/NuGet: configure nuget.org pages for Js2IL.Core and Js2IL.SDK
    - https://github.com/tomacox74/js2il/issues/849
17. #850 Hosting/NuGet: add Core/SDK restore and post-publish smoke validation
    - https://github.com/tomacox74/js2il/issues/850

## Open PRs (for context)
- **#851** `fix(hosting): support mutable CommonJS exports` — OPEN
  - https://github.com/tomacox74/js2il/pull/851

## Label/metadata gaps (as of this snapshot)
- Open issues: 23
- Open PRs: 1
- 21 open issues are missing `priority:*` labels.
- No current open issues carry `lane:*` labels (23 missing).
