# Issue triage snapshot (2026-03-06)

This file captures a point-in-time prioritized list of open issues/PRs and the current active item being addressed.

Synced to:
- Repo: `master` @ `9c2ce6cc` (merge of PR #821 ordinary object invariants for issue #776)
- GitHub: open issues/PRs state as of 2026-03-08 (refreshed in-place)

## Current active item
**Issue #777** (OPEN; active work is now on `copilot/gh-777-object-integrity`, with draft PR #822 open for review):
- https://github.com/tomacox74/js2il/issues/777
- Draft PR: https://github.com/tomacox74/js2il/pull/822

Rationale:
- #776 is now closed via merged PR #821, so #777 becomes the next unfinished ranked medium-priority item from this list.
- The current branch tightens strict-mode integrity enforcement in `JavaScriptRuntime.Object` for `preventExtensions` / `seal` / `freeze` interactions, including write/delete/add failures and inherited prototype descriptor restrictions.
- Focused integrity and broader Object execution/generator coverage are green, and the draft PR is open but not yet marked ready for review.
- This keeps the Object meta-programming surface aligned with Node for common ordinary-object integrity transitions before moving on to the next constructor/runtime gaps.

## Recommended next item after #777
**Issue #778** (OPEN):
- https://github.com/tomacox74/js2il/issues/778

Rationale:
- It is now the next unfinished ranked medium-priority spec item once the Object integrity audit lands.
- It is adjacent compiler/runtime work but does not block the current Object surface completion pass.

## Recently completed since the previous snapshot
- **#776** (priority:medium) ECMA-262: Ordinary object internal method invariants (DefineOwnProperty/GetOwnProperty, keys ordering) — CLOSED via PR #821 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/776
  - https://github.com/tomacox74/js2il/pull/821
- **#583** (priority:medium) Quality: add real-world canary corpus smoke tests (bounded) — CLOSED via PR #820 on 2026-03-08
  - https://github.com/tomacox74/js2il/issues/583
  - https://github.com/tomacox74/js2il/pull/820
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

1. #777 (priority:medium) ECMA-262: Object integrity APIs semantics audit (freeze/seal/preventExtensions/is*)
   - https://github.com/tomacox74/js2il/issues/777
   - Current branch status: `copilot/gh-777-object-integrity` now has draft PR #822 open with runtime, test, changelog, and ECMA-262 tracking updates.
2. #778 (priority:medium) ECMA-262: Support Function constructor (new Function / CreateDynamicFunction)
   - https://github.com/tomacox74/js2il/issues/778
3. #779 (priority:medium) ECMA-262: Symbol ecosystem completeness audit (well-known symbols + symbol-key introspection semantics)
   - https://github.com/tomacox74/js2il/issues/779
4. #780 (priority:medium) ECMA-262: Expose Array iterator methods (entries/keys/values/[Symbol.iterator])
   - https://github.com/tomacox74/js2il/issues/780
5. #787 (priority:medium) node: expand util essentials (format, inspect parity, util.types breadth)
   - https://github.com/tomacox74/js2il/issues/787
6. #788 (priority:medium) node: expand child_process beyond sync (spawn/exec/execFile, stdio pipes)
   - https://github.com/tomacox74/js2il/issues/788
7. #789 (priority:medium) node: implement url/querystring baseline (URL, URLSearchParams, parse/stringify)
   - https://github.com/tomacox74/js2il/issues/789
8. #790 (priority:medium) node: implement crypto minimum practical subset (createHash, randomBytes, webcrypto bridge)
   - https://github.com/tomacox74/js2il/issues/790
9. #781 (priority:low) ECMA-262: Implement WeakRef + FinalizationRegistry host cleanup model
   - https://github.com/tomacox74/js2il/issues/781
10. #791 (priority:low) node: add ESM interop baseline (import.meta.url + Node-style ESM resolution plan)
   - https://github.com/tomacox74/js2il/issues/791

## Remaining open issues
11. #792 (priority:low) node: add http/https/net/tls baseline plan (client/server skeleton)
     - https://github.com/tomacox74/js2il/issues/792
12. #419 Hosting: support mutable CommonJS exports (Node-like)
     - https://github.com/tomacox74/js2il/issues/419
13. #439 Hosting: publish referenceable library/build NuGet package
     - https://github.com/tomacox74/js2il/issues/439
14. #451 perf(il): expand typed temps/locals to reduce casts/boxing
     - https://github.com/tomacox74/js2il/issues/451
15. #727 Function length/name should be descriptor-backed own properties
     - https://github.com/tomacox74/js2il/issues/727
16. #728 Complete bound function semantics for constructor/new-target and metadata
     - https://github.com/tomacox74/js2il/issues/728
17. #737 perf: callsite-based typed parameter specialization for non-exported functions
     - https://github.com/tomacox74/js2il/issues/737
18. #738 perf(prime): close PrimeJavaScript gap with spec-safe hot-path optimizations
     - https://github.com/tomacox74/js2il/issues/738
19. #740 perf(prime): keep sieve loop math in typed locals with fallback
     - https://github.com/tomacox74/js2il/issues/740
20. #742 perf(prime): trim timing/config coercion overhead in main path
     - https://github.com/tomacox74/js2il/issues/742
21. #743 perf(prime): add Prime perf acceptance gate and reporting
     - https://github.com/tomacox74/js2il/issues/743
22. #746 perf: make dromaeo-object-regexp faster than Jint prepared
     - https://github.com/tomacox74/js2il/issues/746
23. #747 perf(regexp): cache Regex instances by source+flags
     - https://github.com/tomacox74/js2il/issues/747
24. #748 perf(dispatch): add RegExp fast paths in Object.CallMember1/2
     - https://github.com/tomacox74/js2il/issues/748
25. #768 Perf: devirtualize calls to const/arrow function bindings (dromaeo-object-regexp-modern)
     - https://github.com/tomacox74/js2il/issues/768

## Open PRs (for context)
- #822 (DRAFT): https://github.com/tomacox74/js2il/pull/822 — object integrity API semantics audit for issue #777.

## Label/metadata gaps (as of this snapshot)
- Open issues: 27
- Open PRs: 1
- Several open issues still lack `priority:*` labels.
- `lane:*` coverage is still incomplete (at least 14 open issues are missing a lane label).
